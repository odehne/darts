namespace ScoresDb.Entities
{
	public class PlayerEntity : DartEntity
    {
		public string Name { get; set; }
		public double TotalAvg { get; set; }
		public double GameAvg { get; set; }
		public int HighestScore { get; set; }
		public int HighestCheckOut { get; set; }
	}
}