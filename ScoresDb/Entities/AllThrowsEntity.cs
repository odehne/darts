namespace ScoresDb.Entities
{
	public class AllThrowsEntity : List<ThrowEntity>
	{
		public Guid PlayerId { get; set; }
		public Guid LegId { get; set; }
		public AllThrowsEntity()
		{
		}
		public AllThrowsEntity(Guid playerId, Guid legId)
		{
			PlayerId = playerId;
			LegId = legId;
		}
	}
}