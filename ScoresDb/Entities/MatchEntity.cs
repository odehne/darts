namespace ScoresDb.Entities
{

	public class MatchEntity : DartEntity
	{
		public int WhoStarts { get; set; }
		public Guid GameWinner { get; set; }
		
		public List<SetEntity> Sets { get; set; }
		public int BestOfSets { get; set; }
		public int BestOfLegs { get; set; }
		public int StartValue { get; set; }
		public DateTime PlayedAt { get; set; }

		public List<PlayerEntity> Players { get; set; }

		public MatchEntity()
		{
			Players = new List<PlayerEntity>();
		}
	}

	
}