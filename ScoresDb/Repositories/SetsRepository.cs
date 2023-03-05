using ScoresDb.Entities;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace ScoresDb.Repositories
{
    public interface ISetsRepository : IRepository<SetEntity>
	{
		Task<List<SetEntity>> GetSets(Guid gameId);
	}
	public class SetsRepository : ISetsRepository
	{
		public string TableName => "Sets";

		public ILegsRepository _legRepo;

		public List<SetEntity> Items { get; set; }
		public string ConnectionString { get; set; }

		public SetsRepository(string connectionString, ILegsRepository legRepo)
		{
			ConnectionString = connectionString;
			_legRepo = legRepo;
		}


		public async Task<SetEntity> Create(SetEntity item)
		{
			using (var connection = new SQLiteConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					// Erstellen der Tabelle, sofern diese noch nicht existiert.
					command.CommandText = $"INSERT OR REPLACE INTO {TableName} (id, SetIndex, gameId, BestOfLegs, StartValue, WhoStarts, SetWinner) VALUES (@id, @SetIndex, @gameId, @BestOfLegs, @StartValue, @WhoStarts, @SetWinner);";
					command.Parameters.AddWithValue("@id", item.Id.ToString());
					command.Parameters.AddWithValue("@gameId", item.GameId.ToString());
					command.Parameters.AddWithValue("@BestOfLegs", item.BestOfLegs);
					command.Parameters.AddWithValue("@SetIndex", item.SetIndex);
					command.Parameters.AddWithValue("@StartValue", item.StartValue);
					command.Parameters.AddWithValue("@WhoStarts", item.WhoStarts.ToString());
					command.Parameters.AddWithValue("@SetWinner", item.SetWinner.ToString());
					command.ExecuteNonQuery();
					return await GetById(item.Id);
				}
			}
		}

		public async Task<string> CreateTable()
		{
			try
			{
				var sql = $"CREATE TABLE \"{TableName}\" (\"Id\" TEXT, \"gameId\" TEXT, \"SetIndex\" INTEGER, \"BestOfLegs\" INTEGER, \"StartValue\" INTEGER, \"WhoStarts\" TEXT, \"SetWinner\" TEXT, PRIMARY KEY(\"Id\"));";
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

		public async Task<bool> Delete(SetEntity item)
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

		public async Task<SetEntity> GetById(Guid id)
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

		public Task<SetEntity> GetByName(string name)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<SetEntity>> GetItems()
		{
			Items = new List<SetEntity>();
			await ReadAll();
			return Items;
		}

		public async Task<List<SetEntity>> GetSets(Guid gameId)
		{
			var lst = new List<SetEntity>();
			if (Items == null || Items.Count == 0)
			{
				await ReadAll();
			}
			if (Items.Count > 0)
			{
				lst.AddRange(Items.Where(x => x.GameId == gameId));
			}
			return lst;
		}

		public async Task<bool> ReadAll()
		{
			Items = new List<SetEntity>();
			if (string.IsNullOrEmpty(ConnectionString))
				throw new ArgumentException("ConnectionString not set.");

			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			using var cmd = new SQLiteCommand($"SELECT Id, SetIndex, GameId, BestOfLegs, StartValue, WhoStarts, SetWinner FROM {TableName} ORDER BY SetIndex", conn);
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Items.Add(await ReadRecord(reader));
			}

			return true;
		}

		private async Task<SetEntity> ReadRecord(SQLiteDataReader reader)
		{
			var ret = new SetEntity();
			ret.Id = Guid.Parse(reader.GetString(reader.GetOrdinal("Id")));
			ret.GameId = Guid.Parse(reader.GetString(reader.GetOrdinal("GameId")));
			ret.SetIndex = reader.GetInt32(reader.GetOrdinal("SetIndex"));
			ret.BestOfLegs = reader.GetInt32(reader.GetOrdinal("BestOfLegs"));
			ret.StartValue = reader.GetInt32(reader.GetOrdinal("StartValue"));
			ret.WhoStarts = Guid.Parse(reader.GetString(reader.GetOrdinal("WhoStarts")));
			ret.SetWinner = Guid.Parse(reader.GetString(reader.GetOrdinal("SetWinner")));
			ret.Legs = await _legRepo.GetLegs(ret.Id);
			return ret;
		}

		public async Task<SetEntity> Update(SetEntity item)
		{
			return await Create(item);

		}
	}
}