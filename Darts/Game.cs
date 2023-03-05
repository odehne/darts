using Newtonsoft.Json.Linq;
using ScoresDb.Repositories;
using System.Reflection;

namespace Darts
{
	public enum Bestofs
	{
		BestOfThree = 3,
		BestOfFive = 5,
		BestOfSeven = 7,
		BestOfNine = 9,
		BestOfEleven = 11,
		BestOfThirteen = 13
	}

	public class Game
	{

		public event GameCompletedEventHandler GameCompleted;
		public delegate void GameCompletedEventHandler(int whatPlayer);

		public event SetCompletedEventHandler SetCompleted;
		public delegate void SetCompletedEventHandler(int whatPlayer, int whoStarts);

		public event LegCompletedEventHandler LegCompleted;
		public delegate void LegCompletedEventHandler(int whatPlayer, int whoStarts);

		public event CurrentThrowCompletedEventHandler CurrentThrowCompleted;
		public delegate void CurrentThrowCompletedEventHandler(int whatPlayer, int thrownValue, int newRest, int whoStarted);

		public Guid Id { get; set; } = Guid.NewGuid();
		public int WhoStarts { get; set; }
		public Guid WinnerId { get; set; }
		public List<Player> Players { get; set; }
		public Sets Sets { get; set; }
		public int BestOfSets { get; set; }
		public int BestOfLegs { get; set; }
		public int StartValue { get; set; }
		public string DbPath { get; set; }

		public int WhatPlayer { get; set; }

		public MatchRepository GameRepo { get; set; }

		private Checkouts PossibleCheckouts { get; set; }

		public string GetDartOut(int currentValue)
		{
			return PossibleCheckouts.GetDartOut(currentValue);
		}

		//public Player GetPlayer(int index)
		//{
		//	if (Players.Count > index)
		//		return Players[index];
		//	return null;
		//}

		public Game(NewGameModel model, string dbPath)
		{
			Players = new List<Player>();
			DbPath = dbPath;
			WhoStarts = 1;
			WhatPlayer = WhoStarts;
			PossibleCheckouts = new Checkouts();
			BestOfSets = (int)model.SetsBestOf;
			BestOfLegs = (int)model.LegsBestOf;
			StartValue = model.StartValue;

			GameRepo = new MatchRepository("Data Source=" + DbPath);

			CreateTables();

			foreach (var p in model.Players)
			{
				var p1 = GameRepo.AllPlayers.GetByName(p.Name).Result;
				p1 ??= GameRepo.AllPlayers.Create(Mapping.DbEntityMapper.ToPlayerEntiy(p)).Result;
				Players.Add(Mapping.DbEntityMapper.FromPlayerEntiy(p1));
			}
			
			Sets = new Sets(BestOfSets, BestOfLegs, StartValue, WhoStarts, Players.Select(x=>x.Id).ToList());
			Sets.GameFinished += Sets_GameFinished;
			Sets.CurrentSetFinished += Sets_CurrentSetFinished;
			Sets.CurrentLegFinished += Sets_CurrentLegFinished;
			Sets.CurrentThrowCompleted += Sets_CurrentThrowCompleted;

			StartNewSet();
			DbPath = dbPath;
		}
		public void CreateTables()
		{
			_ = GameRepo.CreateTable().Result;
			_ = GameRepo.AllPlayers.CreateTable().Result;
			_ = GameRepo.AllSets.CreateTable().Result;
			_ = GameRepo.AllLegs.CreateTable().Result;
			_ = GameRepo.AllThrows.CreateTable().Result;
			_ = GameRepo.AllLegsAndPlayers.CreateTable().Result;
			_ = GameRepo.AllMatchPlayers.CreateTable().Result;
		}

		public async Task<bool> SaveGame()
		{
			try
			{
				var g = Mapping.DbEntityMapper.ToGameEntity(this);
				var result = await GameRepo.SaveGame(g);

				for (int i = 0; i < result.Sets.Count(); i++)
				{
					Sets[i].Id = result.Sets[i].Id;

					for (int j = 0; j < result.Sets[i].Legs.Count(); j++)
					{
						Sets[i].Legs[j].Id = result.Sets[i].Legs[j].Id;
					}
				}

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public string GetSets(int whatPlayer)
		{
			var ret = "";
			foreach (var set in Sets) 
			{
				var x = "";
				if (set.SetWinner!=0)
				{
					x = set.SetWinner == whatPlayer ? x = "X" : x = "0";
					ret+= x;
				}
			}
			return ret;
		}

		public void DecreaseValue(int newValue)
		{
			Sets.Last().Legs.Last().DecreaseValue(newValue, WhatPlayer);
		}

		protected virtual void OnCurrentThrowCompleted(int whatPlayer, int thrownValue, int newRest, int whoStarted)
		{
			WhatPlayer = GetNextPlayer(whatPlayer);

			var handler = CurrentThrowCompleted;
			handler?.Invoke(whatPlayer, thrownValue, newRest, whoStarted);
			
		}

		private int GetNextPlayer(int whatPlayer)
		{
			if (whatPlayer < Players.Count)
				return whatPlayer + 1;
			else
				return 1;
		}

		private void Sets_CurrentThrowCompleted(int whatPlayer, int thrownValue, int newRest, int whoStarted)
		{
			if (thrownValue > Players[whatPlayer - 1].HighestScore)
				Players[whatPlayer - 1].HighestScore = thrownValue;

			OnCurrentThrowCompleted(whatPlayer, thrownValue, newRest, whoStarted);
		}

		private void Sets_CurrentLegFinished(int whatPlayer, int whoStarts, int finishValue)
		{
			WhoStarts = whoStarts;

				if(finishValue > Players[whatPlayer-1].HighestCheckOut) 
					Players[whatPlayer - 1].HighestCheckOut = finishValue;

			OnLegCompleted(whatPlayer, finishValue);
		}

		private void Sets_CurrentSetFinished(int whatPlayer, int whoStarts)
		{
			WhoStarts = whoStarts;
			OnSetCompleted(whatPlayer);
		}

		private void Sets_GameFinished(int whatPlayer)
		{
			OnGameCompleted(whatPlayer);
		}

		protected virtual void OnGameCompleted(int whatPlayer)
		{
			WinnerId = Players[whatPlayer - 1].Id;

			var handler = GameCompleted;
			handler?.Invoke(whatPlayer);
		}

		protected virtual void OnSetCompleted(int whatPlayer)
		{
			var handler = SetCompleted;
			handler?.Invoke(whatPlayer, WhoStarts);
		}

		protected virtual void OnLegCompleted(int whatPlayer, int finishValue)
		{
			var handler = LegCompleted;
			handler?.Invoke(whatPlayer, WhoStarts);
		}

		public List<string> GetThrowHistory(int whatPlayer)
		{
			return Sets.Last().Legs.Last().GetThrowHistory(whatPlayer);
		}

		public int GetBalance(int whatPlayer)
		{
			return Sets.Last().Legs.Last().ThrowsOfPlayers[whatPlayer-1].Last().Balance;
		}

		public int GetHighestScore(int whatPlayer)
		{
			return Sets.Last().Legs.Last().ThrowsOfPlayers[whatPlayer - 1].Max(x=>x.Throw);
		}

		public double GetAvg(int whatPlayer)
		{
			if (Sets.Last().Legs.Last().ThrowsOfPlayers[whatPlayer - 1].Count > 1)
				return Sets.Last().Legs.Last().ThrowsOfPlayers[whatPlayer - 1].Sum(x => x.Throw) / (Sets.Last().Legs.Last().ThrowsOfPlayers[whatPlayer - 1].Count -1);
			return 0;
		}

		public Tuple<int, int> GetLastThrownValue()
		{
			var pcount = Sets.Last().Legs.Last().ThrowsOfPlayers.Count;

			if (pcount == 1)
				return new Tuple<int, int>(1, Sets.Last().Legs.Last().ThrowsOfPlayers[0].Last().Throw);

			if (WhatPlayer == 1)
				return new Tuple<int, int>(pcount, Sets.Last().Legs.Last().ThrowsOfPlayers[pcount - 1].Last().Throw);

			return new Tuple<int, int>(WhatPlayer-1, Sets.Last().Legs.Last().ThrowsOfPlayers[WhatPlayer - 2].Last().Throw);
		}

		public void StartNewSet()
		{
			Sets.AddSet();
		}

		public void Restart()
		{
			Sets.Clear();
		}
	}
}
