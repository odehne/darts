namespace ScoresDb.Entities
{
	public class DartEntity
	{
		public Guid Id { get; set; }

		public DartEntity()
		{
			Id = Guid.NewGuid();
		}
	}
}