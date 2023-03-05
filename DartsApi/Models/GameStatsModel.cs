namespace DartsApi.Models
{
	public class GameStatsModel
	{
		public string Id { get; set; }
		public bool GameWon { get; set; }
		public int HighestScore_Game { get; set; }
		public int HighestScore_Set { get; set; }
		public int HighestScore_Leg { get; set; }
		public int HighestCheckout_Game { get; set; }
		public int HighestCheckout_Set { get; set; }
		public int HighestCheckout_Leg { get; set; }
		public double Avg_Game { get; set; }
		public double Avg_Set { get; set; }
		public double Avg_Leg { get; set; }
		public double AvgBeforeCheckout { get; set; }

		public int BestOfSets { get; set; }
		public int BestOfLegs { get; set; }
		public int StartValue { get; set; }
		public DateTime PlayedAt { get; set; }
	}

}
