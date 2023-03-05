namespace Darts
{
	public class Leg
	{
		public event LegFinishedEventHandler LegFinished;
		public delegate void LegFinishedEventHandler(int whatPlayer, int whoStarts, int finishValue);

		public event ThrowAppliedEventHandler ThrowApplied;
		public delegate void ThrowAppliedEventHandler(int whatPlayer, int thrownValue, int newRest, int whoStarted);

		public int LegWinner = 0;
		public int Index;
		public int WhoStarts { get; set; }

		public List<Guid> PlayerIds { get; set; }

		public List<AllThrows> ThrowsOfPlayers { get; set; }

		public Guid Id { get; set; }
		
		public int StartValue { get; set; }

		protected virtual void OnLegFinished(int whatPlayer, int finishValue)
		{
			LegWinner = whatPlayer;

			if (WhoStarts == 1)
				WhoStarts = 2;
			else 
				WhoStarts = 1;

			var handler = LegFinished;
			handler?.Invoke(whatPlayer, WhoStarts, finishValue);
		}

		protected virtual void OnThrowApplied(int whatPlayer, int thrownValue, int newBalance)
		{
			var handler = ThrowApplied;
			handler?.Invoke(whatPlayer, thrownValue, newBalance, WhoStarts);
		}

	

		public int GetAvgPerDart(int whatPlayer)
		{
			return ThrowsOfPlayers[whatPlayer - 1].Last().Balance / 3;
		}

		
		public Leg(int startValue, int whoStarts, List<Guid> playerIds)
		{
			ThrowsOfPlayers = new List<AllThrows>();
			Id = Guid.NewGuid();
			WhoStarts = whoStarts;
			StartValue = startValue;
			PlayerIds= playerIds;

			foreach (var playerId in PlayerIds)
			{
				ThrowsOfPlayers.Add(new AllThrows(playerId) { new SingleThrow(startValue, 0, 0) });
			}
		}

		public int GetBalance(int whatPlayer)
		{
			return ThrowsOfPlayers[whatPlayer - 1].Last().Balance;
		}

		public List<string> GetThrowHistory(int whatPlayer)
		{
			var lst = new List<string>();

			foreach (var thro in ThrowsOfPlayers[whatPlayer-1])
			{
				lst.Add(thro.ToString());
			}

			return lst;
		}

		public Tuple<int, int> UpdateValue(int newValue, int whatPlayer)
		{
			ThrowsOfPlayers[whatPlayer - 1].Last().Balance += ThrowsOfPlayers[whatPlayer - 1].Last().Throw;
			ThrowsOfPlayers[whatPlayer - 1].Last().Throw = newValue;
			ThrowsOfPlayers[whatPlayer - 1].Last().Balance -= ThrowsOfPlayers[whatPlayer - 1].Last().Throw;
		
			//Hat jemand gewonnen?
			var newBalance = GetBalance(whatPlayer);
			
			if (newBalance == 0)
			{
				OnLegFinished(whatPlayer, newValue);
			}
			else
			{
				return new Tuple<int, int>(newValue, newBalance);
			}

			return null;
		}

		public void DecreaseValue(int value, int whatPlayer)
		{
			var currentBalance = GetBalance(whatPlayer);

			// result of 1 or less than 0 is illegal
			if (currentBalance - value == 1 || currentBalance - value < 0)
				return;
			
			// value greater than 180 is illegal
			if (value > 180)
				return;

			ThrowsOfPlayers[whatPlayer - 1].AddThrow(currentBalance, value, ThrowsOfPlayers[whatPlayer - 1].Count);
			

			//Hat jemand gewonnen?
			var newBalance = GetBalance(whatPlayer);
			OnThrowApplied(whatPlayer, value, newBalance);

			if (newBalance == 0)
				OnLegFinished(whatPlayer, value);


		}
	}
}