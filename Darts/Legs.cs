namespace Darts
{
	public class Legs : List<Leg> 
	{
		public int BestOfLegs { get; set; }
		public int StartValue { get; set; }
		public int WhoStarts { get; set; }

		public List<Guid> PlayerIds { get; set; }

		public event CurrentLegFinishedEventHandler CurrentLegFinished;
		public delegate void CurrentLegFinishedEventHandler(int whatPlayer, int whoStarts, int finishValue);

		public event SetFinishedEventHandler SetFinished;
		public delegate void SetFinishedEventHandler(int whatPlayer, int whoStarts);

		public event CurrentThrowCompletedEventHandler CurrentThrowCompleted;
		public delegate void CurrentThrowCompletedEventHandler(int whatPlayer, int thrownValue, int newRest, int whoStarted);

		protected virtual void OnCurrentThrowCompleted(int whatPlayer, int thrownValue, int newRest, int whoStarted)
		{
			var handler = CurrentThrowCompleted;
			handler?.Invoke(whatPlayer, thrownValue, newRest, whoStarted);
		}

		protected virtual void OnCurrentLegFinished(int whatPlayer, int finishValue)
		{
			var handler = CurrentLegFinished;
			handler?.Invoke(whatPlayer, WhoStarts, finishValue);
		}

		protected virtual void OnSetFinished(int whatPlayer)
		{
			var handler = SetFinished;
			handler?.Invoke(whatPlayer, WhoStarts);
		}

		public void AddLeg()
		{
			var leg = new Leg(StartValue, WhoStarts, PlayerIds);
			leg.Index = Count;
			leg.LegFinished += LegFinished;
			leg.ThrowApplied += Leg_ThrowApplied;
			Add(leg);
		}

		private void Leg_ThrowApplied(int whatPlayer, int thrownValue, int newRest, int whoStarted)
		{
			OnCurrentThrowCompleted(whatPlayer, thrownValue, newRest, whoStarted);	
		}

		private void LegFinished(int whatPlayer, int whoStarts, int finishValue)
		{
			WhoStarts = whoStarts;
			if (!IsTheSetFinished(whatPlayer))
				OnCurrentLegFinished(whatPlayer, finishValue);
		}
		
		public Legs(int bestOfLegs, int startValue, int whoStarts, List<Guid> playerIds)
		{
			PlayerIds = playerIds;
			BestOfLegs = bestOfLegs;
			StartValue = startValue;
			WhoStarts = whoStarts;
		}

		public bool IsTheSetFinished(int playerWhoWonTheLeg)
		{
			var legsToFinishSet = (BestOfLegs + 1) / 2;
			var finishedLegs = this.Where(w=>w.LegWinner == playerWhoWonTheLeg).Count();
			if(finishedLegs >= legsToFinishSet)
			{
				OnSetFinished(playerWhoWonTheLeg);
				return true;
			}
			return false;	
		}
	}
}