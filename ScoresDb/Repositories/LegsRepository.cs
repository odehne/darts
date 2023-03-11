using ScoresDb.Entities;
using System.Data.SQLite;

namespace ScoresDb.Repositories
{
	public interface ILegsRepository : IRepository<LegEntity>
    {
    	Task<List<LegEntity>> GetLegs(Guid setId);
		Task<List<LegEntity>> GetAllWonLegs(Guid playerId);
		Task<List<LegEntity>> GetLegsByPlayerId(Guid playerId);
	}

	public class LegsRepository : ILegsRepository
	{
		public IThrowsRepository _throwRepo;
		public ILegPlayersRepository _legsAndPlayersRepo;
		public string TableName => "Legs";

		public List<LegEntity> Items { get; set; }
		public string ConnectionString { get; set; }

		public LegsRepository(string connectionString, IThrowsRepository throwRepo, ILegPlayersRepository legsAndPlayersRepo)
		{
			ConnectionString = connectionString;
			_throwRepo = throwRepo;
			_legsAndPlayersRepo = legsAndPlayersRepo;
		}

		public async Task<LegEntity> Create(LegEntity item)
		{
			using (var connection = new SQLiteConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					// Erstellen der Tabelle, sofern diese noch nicht existiert.
					command.CommandText = $"INSERT OR REPLACE INTO {TableName} (id, LegIndex, StartValue, LegWinner, SetId, WhoStarts) VALUES (@id, @LegIndex, @StartValue, @LegWinner, @SetId, @WhoStarts);";
					command.Parameters.AddWithValue("@id", item.Id.ToString());
					command.Parameters.AddWithValue("@LegIndex", item.LegIndex);
					command.Parameters.AddWithValue("@StartValue", item.StartValue);
					command.Parameters.AddWithValue("@LegWinner", item.LegWinner.ToString());
					command.Parameters.AddWithValue("@SetId", item.SetId.ToString());
					command.Parameters.AddWithValue("@WhoStarts", item.WhoStarts.ToString());
					command.ExecuteNonQuery();
					return await GetById(item.Id);
				}
			}
		}

		public async Task<string> CreateTable()
		{
			try
			{
				var sql = $"CREATE TABLE \"{TableName}\" (\"Id\" TEXT, \"LegIndex\" INTEGER, \"StartValue\" INTEGER, \"LegWinner\" TEXT, \"SetId\" TEXT, \"WhoStarts\" TEXT, PRIMARY KEY(\"Id\"));";
				using var connection = new SQLiteConnection(ConnectionString);
				connection.Open();
				using var command = new SQLiteCommand(connection);
				command.CommandText = sql;
				command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				return $"Failed to create table [{TableName}], Error: [{e.Message}].";
			}
			return "OK";
		}

		public async Task<bool> Delete(LegEntity item)
		{
			var query = $"DELETE FROM [{TableName}] WHERE Id = @Id";
			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			var cmd = new SQLiteCommand(query, conn);
			cmd.Parameters.AddWithValue("@Id", item.Id);
			cmd.ExecuteNonQuery();
			await ReadAll();
			return true;
		}

		public async Task<LegEntity> GetById(Guid id)
		{
			if (Items == null || Items.Count == 0)
			{
				var b = await ReadAll();
				if (!b)
					return null;
			}
			var itm = Items.FirstOrDefault(x => x.Id == id);
			if (itm == null)
			{
				if (await ReadAll())
					itm = Items.FirstOrDefault(x => x.Id == id);
			}
			return itm;
		}

		public async Task<LegEntity> GetByName(string name)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<LegEntity>> GetItems()
		{
			Items = new List<LegEntity>();
			await ReadAll();
			return Items;
		}

		public async Task<AllThrowsEntity> GetThrows(Guid legId)
		{
			return await _throwRepo.GetThrowsByLegId(legId);
		}

		public async Task<bool> ReadAll()
		{
			Items = new List<LegEntity>();
			if (string.IsNullOrEmpty(ConnectionString))
				throw new ArgumentException("ConnectionString not set.");

			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			using var cmd = new SQLiteCommand($"SELECT id, LegIndex, StartValue, LegWinner, SetId, WhoStarts FROM {TableName} ORDER BY LegIndex", conn);
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Items.Add(await ReadRecord(reader));
			}

			return true;
		}

		private async Task<LegEntity> ReadRecord(SQLiteDataReader reader)
		{
			var leg = new LegEntity
			{
				Id = Guid.Parse(reader.GetString(reader.GetOrdinal("Id"))),
				LegIndex = reader.GetInt32(reader.GetOrdinal("LegIndex")),
				StartValue = reader.GetInt32(reader.GetOrdinal("StartValue")),
				LegWinner = Guid.Parse(reader.GetString(reader.GetOrdinal("LegWinner"))),
				SetId = Guid.Parse(reader.GetString(reader.GetOrdinal("SetId"))),
				WhoStarts = Guid.Parse(reader.GetString(reader.GetOrdinal("WhoStarts")))
			};

			leg.ThrowsOfPlayers = await _throwRepo.GetThrowsByLegId(leg.Id);

			return leg;

		}


		public async Task<LegEntity> Update(LegEntity item)
		{
			return await Create(item);
		}

		public async Task<List<LegEntity>> GetLegs(Guid setId)
		{
			if (Items == null || Items.Count == 0)
			{
				var b = await ReadAll();
				if (!b)
					return null;
			}
			return Items.Where(x=> x.SetId == setId).ToList();

		}

		public async Task<List<LegEntity>> GetLegsByPlayerId(Guid playerId)
		{
			var ret = new List<LegEntity>();
			var ids = await _legsAndPlayersRepo.GetLegsByPlayerId(playerId);
			if (Items == null || Items.Count == 0)
			{
				var b = await ReadAll();
				if (!b)
					return null;
			}
			foreach (var id in ids)
			{
				ret.Add(await GetById(id));
			}
			return ret;
		}

		public async Task<List<LegEntity>> GetAllWonLegs(Guid playerId)
		{
			if (Items == null || Items.Count == 0)
			{
				var b = await ReadAll();
				if (!b)
					return null;
			}
			return Items.Where(x => x.LegWinner == playerId).ToList();
		}
	}
}