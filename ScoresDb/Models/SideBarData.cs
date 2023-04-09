namespace ScoresDb.Models
{
    public class SideBarData
	{
		public List<PlayerModel> Players { get; set; }
		public bool sidebar { get; set; } = false;
        public SideBarData()
        {
			Players = new List<PlayerModel>();
        }
    }

}
