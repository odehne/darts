using ScoresDb.Entities;
using System.Data.Entity.Migrations.Model;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Xml.Schema;

namespace ScoresDb.Repositories
{
	public interface IThrowsRepository : IRepository<ThrowEntity>
	{
		Task<AllThrowsEntity> GetThrowsByLegIdAndPlayerId(Guid legId, Guid playerId);
		Task<List<AllThrowsEntity>> GetThrowsByLegId(Guid legId);
	}



	public class ThrowsRepository : IThrowsRepository
	{
		public string TableName => "AllThrows";

		public List<ThrowEntity> Items { get; set; }
		public string ConnectionString { get; set; }

		public ThrowsRepository(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public async Task<ThrowEntity> Create(ThrowEntity item)
		{
			using (var connection = new SQLiteConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					// Erstellen der Tabelle, sofern diese noch nicht existiert.
					command.CommandText = $"INSERT OR REPLACE INTO {TableName} (id, ThrowIndex, Balance, Throw, PlayerId, LegId) VALUES (@id, @ThrowIndex, @Balance, @Throw, @PlayerId, @LegId);";
					command.Parameters.AddWithValue("@id", item.Id.ToString());
					command.Parameters.AddWithValue("@ThrowIndex", item.ThrowIndex);
					command.Parameters.AddWithValue("@Balance", item.Balance);
					command.Parameters.AddWithValue("@Throw", item.Throw);
					command.Parameters.AddWithValue("@PlayerId", item.PlayerId.ToString());
					command.Parameters.AddWithValue("@LegId", item.LegId.ToString());
					command.ExecuteNonQuery();
					return await GetById(item.Id);
				}
			}
		}

		public async Task<string> CreateTable()
		{
			try
			{
				var sql = $"CREATE TABLE \"{TableName}\" (\"Id\" TEXT,  \"ThrowIndex\" INTEGER, \"Balance\" INTEGER, \"Throw\" INTEGER, \"PlayerId\" TEXT, \"LegId\" TEXT, PRIMARY KEY(\"Id\"));";
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

		public async Task<bool> Delete(ThrowEntity item)
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

		public async Task<ThrowEntity> GetById(Guid id)
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

		public Task<ThrowEntity> GetByName(string name)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<ThrowEntity>> GetItems()
		{
			Items = new List<ThrowEntity>();
			await ReadAll();
			return Items;
		}

		public async Task<bool> ReadAll()
		{
			Items = new List<ThrowEntity>();
			if (string.IsNullOrEmpty(ConnectionString))
				throw new ArgumentException("ConnectionString not set.");

			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			using var cmd = new SQLiteCommand($"SELECT Id, ThrowIndex, Balance, Throw, PlayerId, LegId FROM {TableName} ORDER BY ThrowIndex", conn);
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Items.Add(await ReadRecord(reader));
			}

			return true;
		}

		private async Task<ThrowEntity> ReadRecord(SQLiteDataReader reader)
		{
			var ret = new ThrowEntity();
			ret.Id = Guid.Parse(reader.GetString(reader.GetOrdinal("Id")));
			ret.ThrowIndex = reader.GetInt32(reader.GetOrdinal("ThrowIndex"));
			ret.Balance = reader.GetInt32(reader.GetOrdinal("Balance"));
			ret.Throw = reader.GetInt32(reader.GetOrdinal("Throw"));
			ret.PlayerId = Guid.Parse(reader.GetString(reader.GetOrdinal("PlayerId")));
			ret.LegId = Guid.Parse(reader.GetString(reader.GetOrdinal("LegId")));
			return ret;
		}

		public Task<ThrowEntity> Update(ThrowEntity item)
		{
			throw new NotImplementedException();
		}

		public async Task<List<AllThrowsEntity>> GetThrowsByLegId(Guid legId)
		{
			var ret = new List<AllThrowsEntity>();
			if (Items == null || Items.Count == 0)
			{
				await ReadAll();
			}
			if (Items.Count > 0)
			{
				var allThrows = Items.Where(x => x.LegId == legId);
				var lst = new AllThrowsEntity();
				foreach (var playerId in allThrows.Select(p=>p.PlayerId).Distinct())
				{
					lst.PlayerId = playerId;
					lst.AddRange(allThrows.Where(p=>p.PlayerId == playerId));
				}
				ret.Add(lst);
			}
			return ret;
		}

		public async Task<AllThrowsEntity> GetThrowsByLegIdAndPlayerId(Guid legId, Guid playerId)
		{
			var lst = new AllThrowsEntity(playerId, legId);
			if (Items == null || Items.Count == 0)
			{
				await ReadAll();
			}
			if(Items.Count>0)
			{
				lst.AddRange(Items.Where(x=>x.LegId==legId && x.PlayerId==playerId));
			}
			return lst;
		}
	}
}