namespace ScoresDb.Models
{

	public class SideBarData
	{
		public List<PlayerModel> Players { get; set; }

        public SideBarData()
        {
			Players = new List<PlayerModel>();
        }
    }

	public class PlayerModel
    {
		public string Id { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public string Path { get; set; }
	}

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
