namespace ScoresDb.Entities
{

	public class ThrowEntity : DartEntity
	{
		public int Throw { get; set; }
		public int ThrowIndex { get; set; }
		public int Balance { get; set; }
		public Guid PlayerId { get; set; }
		public Guid LegId { get; set; }

	}
}