namespace Darts
{

	public class Sets : List<Set> 
	{
		public int BestOfSets { get; set; }
		public int StartValue { get; set; }
		public int BestOfLegs { get; set; }
		public int WhoStarts { get; set; }
		public Guid GameId { get; set; }

		public List<Guid> PlayerIds { get; set; }
		
		public event GameFinishedEventHandler GameFinished;
		public delegate void GameFinishedEventHandler(int whatPlayer);

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

		public Sets(int bestofSets, int bestOfLegs, int startValue, int whoStarts, List<Guid> playerIds)
		{
			BestOfSets = bestofSets;
			StartValue = startValue;
			BestOfLegs = bestOfLegs;
			WhoStarts = whoStarts;
			PlayerIds = playerIds;
		}

		public void AddSet()
		{
			
			var set = new Set(BestOfLegs, StartValue, WhoStarts, PlayerIds);
			set.CurrentSetFinished += Set_SetFinished;
			set.CurrentLegFinished += Set_CurrentLegFinished;
			set.CurrentThrowCompleted += Set_CurrentThrowCompleted;
			set.Index = Count;
			Add(set);
		}

		private void Set_CurrentThrowCompleted(int whatPlayer, int thrownValue, int newRest, int whoStarted)
		{
			OnCurrentThrowCompleted(whatPlayer, thrownValue, newRest, whoStarted);
		}

		private void Set_CurrentLegFinished(int whatPlayer, int whoStarts, int finishValue)
		{
			WhoStarts= whoStarts;
			OnCurrentLegFinished(whatPlayer, finishValue);
		}

		protected virtual void OnGameFinished(int whatPlayer)
		{
			var handler = GameFinished;
			handler?.Invoke(whatPlayer);
		}

		protected virtual void OnCurrentSetFinished(int whatPlayer)
		{
			var handler = CurrentSetFinished;
			handler?.Invoke(whatPlayer, WhoStarts);
		}

		protected virtual void OnCurrentLegFinished(int whatPlayer, int finishValue)
		{
			var handler = CurrentLegFinished;
			handler?.Invoke(whatPlayer, WhoStarts, finishValue);
		}

		private void Set_SetFinished(int whatPlayer, int whoStarts)
		{
			WhoStarts = whoStarts;
			IsTheGameFinished(whatPlayer);
		}

		public void IsTheGameFinished(int playerWhoWonThisSet)
		{
			var setsToFinishGame = (BestOfSets + 1) / 2;
			var finishedSets = 0;

			finishedSets = this.Where(w => w.SetWinner == playerWhoWonThisSet).Count();
			if (finishedSets >= setsToFinishGame)
			{
				OnGameFinished(playerWhoWonThisSet);
			}
			else
			{
				AddSet();
				OnCurrentSetFinished(playerWhoWonThisSet);
			}
		}
	}
}