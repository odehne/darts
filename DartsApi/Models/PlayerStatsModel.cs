using ScoresDb.Entities;

namespace DartsApi.Models
{
	public class PlayerStatsModel
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public int HighestScore { get; set; }
		public int HighestCheckout{ get; set; }
		public int LegWinsTotal { get; set; }
		public int SetWinsTotal { get; set; }
		public double AvgTotal { get; set; }
		public double AvgBeforeCheckoutTotal { get; set; }

		public List<GameStatsModel> GameStats { get; set; }

		public List<LegStatsModel> LegStats { get; set; }

		public int GameWinsTotal { get; set; }
		public int LossesTotal { get; set; }

		public PlayerStatsModel()
		{
			GameStats = new List<GameStatsModel>();
		}

		public PlayerStatsModel(List<MatchEntity> theGames, PlayerEntity thePlayer)
		{
			
			Id = thePlayer.Id.ToString();
			Name = thePlayer.Name;
			LegStats = new List<LegStatsModel>();

			GameStats = new List<GameStatsModel>();
			foreach (var game in theGames)
			{
				var gs = new GameStatsModel
				{
					StartValue = game.StartValue,
					BestOfLegs = game.BestOfLegs,
					BestOfSets = game.BestOfSets,
					PlayedAt = game.PlayedAt
				};


				foreach (var set in game.Sets)
				{
					foreach (var leg in set.Legs)
					{
						var legStats = new LegStatsModel { Id = leg.Id.ToString() };

						var thrs = leg.ThrowsOfPlayers.Where(x => x.PlayerId == thePlayer.Id).ToList();
						legStats.Avg = thrs.Sum(x=>x.Throw) / thrs.Count;
						legStats.AvgScoreBeforeCheckout = thrs.Where(x => x.Balance > 100).Sum(x => x.Throw) / thrs.Count(x => x.Balance > 100);
						if(thrs.Last().Throw>HighestCheckout)
							HighestCheckout = thrs.Last().Throw;
						if (leg.LegWinner == thePlayer.Id)
						{
							legStats.Won = true;
							LegWinsTotal++;
						}
						LegStats.Add(legStats);
					}
					SetWinsTotal++;
				}

				gs.Avg_Leg = LegStats.Sum(x=>x.Avg) / LegStats.Count();

				AvgTotal = gs.Avg_Leg;

				if (game.GameWinner == thePlayer.Id)
					GameWinsTotal++;

				GameStats.Add(gs);
			}
		}

	}
}
