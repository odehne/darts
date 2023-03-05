namespace Darts
{
	public class Set
	{

		public Guid Id { get; set; }
		public int Index { get; set; }
		public Legs Legs { get; set; }
	
		public int BestOfLegs { get; set; }
		public int StartValue { get; set; }
		public int SetWinner { get; set; }

		public List<Guid> PlayerIds { get; set; }




		public int WhoStarts { get; set; }
	
		public event CurrentSetFinishedEventHandler CurrentSetFinished;
		public delegate void CurrentSetFinishedEventHandler(int whatPlayer, int whoStarts);

		public event CurrentLegFinishedEventHandler CurrentLegFinished;
		public delegate void CurrentLegFinishedEventHandler(int whatPlayer, int whoStarts, int finishValue);

		public event CurrentThrowCompletedEventHandler CurrentThrowCompleted;
		public delegate void CurrentThrowCompletedEventHandler(int whatPlayer, int thrownValue, int newRest, int whoStarted);

		protected virtual void OnCurrentThrowCompleted(int whatPlayer, int thrownValue, int newRest, int whoStarted)
		{
			var handler = CurrentThrowCompleted;
			handler?.Invoke(whatPlayer, thrownValue, newRest, whoStarted);
		}

		public Set(int maxLegs, int startValue, int startingPlayer, List<Guid> playerIds)
		{
			Id = Guid.NewGuid();
			PlayerIds = playerIds;
			StartValue = startValue;
			BestOfLegs = maxLegs;
			WhoStarts = startingPlayer;
			Legs = new Legs(BestOfLegs, StartValue, WhoStarts, PlayerIds);
			Legs.SetFinished += Legs_SetFinished;
			Legs.CurrentLegFinished += Legs_CurrentLegFinished;
			Legs.CurrentThrowCompleted += Legs_CurrentThrowCompleted;
			Legs.AddLeg();
		}

		public string GetLegs(int whatPlayer)
		{
			var ret = "";
			foreach (var leg in Legs)
			{
				var x = "";
				if (leg.LegWinner != 0)
				{
					x = leg.LegWinner == whatPlayer ? x = "X" : x = "0";
					ret += x;
				}
			}
			return ret;
		}

		private void Legs_CurrentThrowCompleted(int whatPlayer, int thrownValue, int newRest, int whoStarted)
		{
			OnCurrentThrowCompleted(whatPlayer, thrownValue, newRest, whoStarted);	
		}

		protected virtual void OnCurrentLegFinished(int whatPlayer, int finishValue)
		{
			var handler = CurrentLegFinished;
			handler?.Invoke(whatPlayer, WhoStarts, finishValue);
		}

		private void Legs_CurrentLegFinished(int whatPlayer, int whoStarts, int finishValue)
		{
			WhoStarts = whoStarts;
			Legs.AddLeg();
			OnCurrentLegFinished(whatPlayer, finishValue);
		}

		private void Legs_SetFinished(int whatPlayer, int whoStarts)
		{
			WhoStarts= whoStarts;
			OnCurrentSetFinished(whatPlayer, WhoStarts);
		}

		protected virtual void OnCurrentSetFinished(int whatPlayer, int whoStarts)
		{
			WhoStarts = whoStarts;
			SetWinner = whatPlayer;
			var handler = CurrentSetFinished;
			handler?.Invoke(whatPlayer, WhoStarts);
		}
	}
}