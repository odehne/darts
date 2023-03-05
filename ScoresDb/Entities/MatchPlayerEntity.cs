namespace ScoresDb.Entities
{
	public class MatchPlayerEntity : DartEntity
	{
		public Guid MatchId { get; set; }
		public Guid PlayerId { get; set; }
	}
}