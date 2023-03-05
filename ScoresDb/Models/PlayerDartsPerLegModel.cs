namespace ScoresDb.Models
{
	public class PlayerDartsPerLegModel
	{
		public string PlayerName { get; set; }
		public string PlayerId { get; set; }
		public string[] labels { get; set; }
		public List<DataSetModel> Datasets { get; set; }

		public PlayerDartsPerLegModel()
		{
			Datasets = new List<DataSetModel>();
		}
	}

}
