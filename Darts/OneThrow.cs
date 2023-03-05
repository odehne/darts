namespace Darts
{
	public class SingleThrow
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public int Index { get; set; }
		public int Throw { get; set; }
		public int Balance { get; set; }
	
		public SingleThrow(int balance, int playersThrow, int index)
		{
			Throw = playersThrow;
			Balance = balance - playersThrow;
			Index = index;
		}

		public override string? ToString()
		{
			return $"{Balance}   [{Throw}]";
		}
	}

	public class AllThrows : List<SingleThrow>
	{
		public Guid PlayerId { get; set; }
	
		public AllThrows(Guid playerId)
		{
			PlayerId = playerId;
		}

		public void AddThrow(int balance, int playersThrow, int index)
		{
			Add(new SingleThrow(balance, playersThrow, index));
		}
	}
}