namespace ScoresDb.Entities
{
	public class LegEntity : DartEntity
    {
		public AllThrowsEntity ThrowsOfPlayers { get; set; }
		public int LegIndex { get; set; }
		public int StartValue { get; set; }
		public Guid LegWinner {get; set;}
		public Guid SetId { get; set; }
		public Guid WhoStarts { get; set; }

		public LegEntity()
		{
			ThrowsOfPlayers = new AllThrowsEntity();
		}

		public List<ThrowEntity> GetThrows(Guid playerId)
		{
            return ThrowsOfPlayers.Where(x => x.PlayerId == playerId).ToList();
        }
	}

}