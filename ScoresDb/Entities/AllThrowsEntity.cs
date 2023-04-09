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

		public string[] GetLabels()
		{
            var lbls = new List<string>();
			var i = 0;
            foreach (var thr in this)
            {
                lbls.Add(i.ToString());
				i++;
            }
            return lbls.ToArray();
        }

		public int[] GetThrows()
		{
			var throws = new List<int>();

			foreach (var thr in this)
			{
				throws.Add(thr.Throw);
			}
			return throws.ToArray();
		}
	}
}