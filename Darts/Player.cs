using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Darts
{
	public class Player
	{
	 	public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; }
		public int HighestCheckOut { get;  set; }
		public int HighestScore { get; set; }
		public double TotalAvg { get; set; }
		public double GameAvg { get; set; }

		public Player()
		{
		}

		public Player(string name)
		{
			Name = name;
		}
	}

	
}