namespace ScoresDb.Models
{
	public class DataSetModel
	{

		public int id { get; set; }
		public string label { get; set; }
		public int[] data { get; set; }

		public string borderColor { get; set; }
		public string backgroundColor { get; set; }


		public DataSetModel()
		{
		}

		public DataSetModel(int id, string label, int[] data, string borderColor, string backgroundColor)
		{
			this.id = id;
			this.label = label;
			this.data = data;
			this.borderColor = borderColor;
			this.backgroundColor = backgroundColor;
		}
	}

}
