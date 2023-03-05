namespace ScoresDb.Entities
{
	public class LegPlayerEntity : DartEntity
	{
		public Guid LegId { get; set; }
		public Guid PlayerId { get; set; }
	}
}