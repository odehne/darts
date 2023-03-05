namespace Darts
{
	public class NewGameModel
	{
		public List<Player> Players{ get; set; }
		public Bestofs SetsBestOf { get; set; }
		public Bestofs LegsBestOf { get; set; }
		public int StartValue { get; set; }
		public double TotalAvg { get; set; }
		public double GameAvg { get; set; }
		public int HighestScore { get; set; }
		public int HighestCheckOut { get; set; }

		public NewGameModel()
		{
			Players	= new List<Player>();
		}
	}
}