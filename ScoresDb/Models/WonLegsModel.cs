namespace ScoresDb.Models
{
	public class WonLegsModel
	{
		public string[] labels { get; set; }
		public List<DataSetModel> Datasets { get; set; }

		public WonLegsModel()
		{
			Datasets = new List<DataSetModel>();
		}
	}

}
