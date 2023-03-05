namespace ScoresDb.Entities
{
	public class LegEntity : DartEntity
    {
		public List<AllThrowsEntity> ThrowsOfPlayers { get; set; }
		public int LegIndex { get; set; }
		public int StartValue { get; set; }
		public Guid LegWinner {get; set;}
		public Guid SetId { get; set; }
		public Guid WhoStarts { get; set; }

		public LegEntity()
		{
			ThrowsOfPlayers = new List<AllThrowsEntity>();
		}

		public AllThrowsEntity GetThrows(Guid playerId)
		{
			return ThrowsOfPlayers.FirstOrDefault(x=>x.PlayerId == playerId);
		}
	}

}