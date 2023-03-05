namespace ScoresDb.Entities
{
	public class SetEntity : DartEntity
	{
		public Guid GameId { get; set; }
		public List<LegEntity> Legs { get; set; }
		public int SetIndex { get; set; }

		public int BestOfLegs { get; set; }
		public int StartValue { get; set; }
		public Guid WhoStarts { get; set; }
		public Guid SetWinner { get; set; }

		public SetEntity()
		{
			Legs = new List<LegEntity>();
		}
	}
}