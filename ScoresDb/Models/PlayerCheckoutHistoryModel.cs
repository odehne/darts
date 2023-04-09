namespace ScoresDb.Models
{

    public class PlayerCheckoutHistoryModel
	{

		public string PlayerName { get; set; }
		public string PlayerId { get; set; }

		public string[] labels { get; set; }

		public List<int> WonCheckouts { get; set; }
		public List<int> AllCheckouts { get; set; }

		public List<DataSetModel> Datasets { get; set; }

		public PlayerCheckoutHistoryModel()
		{
			WonCheckouts = new List<int>();
			AllCheckouts = new List<int>();
			Datasets = new List<DataSetModel> { };
		}

	}

}
