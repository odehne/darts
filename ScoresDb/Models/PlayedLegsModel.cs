namespace ScoresDb.Models
{
	public class PlayedLegsModel
	{
		public string PlayerName { get; set; }
		public string PlayerId { get; set; }
		public string[] labels { get; set; }
		public List<DataSetModel> Datasets { get; set; }

		public PlayedLegsModel()
		{
			Datasets= new List<DataSetModel>();
		}
	}

}
