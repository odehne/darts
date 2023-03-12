using ScoresDb.Entities;
using System.Data.SQLite;

namespace ScoresDb.Repositories
{
	public interface ILegPlayersRepository : IRepository<LegPlayerEntity>
	{

		Task<List<PlayerEntity>> GetPlayersByLegId(Guid legId);
		Task<List<Guid>> GetLegsByPlayerId(Guid playerId);

	}

	public class LegPlayersRepository : ILegPlayersRepository
	{
		public string TableName => "LegPlayers";

		public List<LegPlayerEntity> Items { get; set; }
		public string ConnectionString { get; set; }

		public IPlayersRepository _playersRepo;

		public LegPlayersRepository(string connectionString, IPlayersRepository playersRepo)
		{
			ConnectionString = connectionString;
			_playersRepo = playersRepo;
		}

		public async Task<LegPlayerEntity> Create(LegPlayerEntity item)
		{
			using (var connection = new SQLiteConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					// Erstellen der Tabelle, sofern diese noch nicht existiert.
					command.CommandText = $"INSERT OR REPLACE INTO {TableName} (id, LegId, PlayerId) VALUES (@id, @LegId, @PlayerId);";
					command.Parameters.AddWithValue("@id", item.Id.ToString());
					command.Parameters.AddWithValue("@LegId", item.LegId.ToString());
					command.Parameters.AddWithValue("@PlayerId", item.PlayerId.ToString());
					command.ExecuteNonQuery();
					return await GetById(item.Id);
				}
			}
		}

		public async Task<string> CreateTable()
		{
			try
			{
				var sql = $"CREATE TABLE \"{TableName}\" (\"Id\" TEXT,  \"LegId\" TEXT, \"PlayerId\" TEXT, PRIMARY KEY(\"Id\"));";
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

		public async Task<bool> Delete(LegPlayerEntity item)
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

		public async Task<LegPlayerEntity> GetById(Guid id)
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

		public Task<LegPlayerEntity> GetByName(string name)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<LegPlayerEntity>> GetItems()
		{
			Items = new List<LegPlayerEntity>();
			await ReadAll();
			return Items;
		}

		public async Task<List<PlayerEntity>> GetPlayersByLegId(Guid legId)
		{
			var ret = new List<PlayerEntity>();
			if (Items == null || Items.Count == 0)
			{
				await ReadAll();
			}
			if (Items.Count > 0)
			{
				var lst = Items.Where(x => x.LegId == legId);
				foreach (var mp in lst)
					ret.Add(await _playersRepo.GetById(mp.PlayerId));
			}
			return ret;
		}

		public async Task<bool> ReadAll()
		{

			if (Items == null)
				Items = new List<LegPlayerEntity>();
			else
				Items.Clear();

			if (string.IsNullOrEmpty(ConnectionString))
				throw new ArgumentException("ConnectionString not set.");

			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			using var cmd = new SQLiteCommand($"SELECT Id, LegId, PlayerId FROM {TableName} ORDER BY Id", conn);
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Items.Add(await ReadRecord(reader));
			}

			return true;
		}

		private async Task<LegPlayerEntity> ReadRecord(SQLiteDataReader reader)
		{
			var ret = new LegPlayerEntity();
			ret.Id = Guid.Parse(reader.GetString(reader.GetOrdinal("Id")));
			ret.PlayerId = Guid.Parse(reader.GetString(reader.GetOrdinal("PlayerId")));
			ret.LegId = Guid.Parse(reader.GetString(reader.GetOrdinal("LegId")));
			return ret;
		}

		public async Task<LegPlayerEntity> Update(LegPlayerEntity item)
		{
			return await Create(item);
		}

		public async Task<bool> AddLeg(Guid legId, List<PlayerEntity> players)
		{
			foreach (var player in players)
			{
				var id = await GetIdByLegAndPlayer(legId, player.Id);
				if (id == Guid.Empty)
					await Create(new LegPlayerEntity { LegId = legId, PlayerId = player.Id });
			}
			return true;
		}

		public async Task<Guid> GetIdByLegAndPlayer(Guid legId, Guid playerId)
		{

			if (string.IsNullOrEmpty(ConnectionString))
				throw new ArgumentException("ConnectionString not set.");

			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			using var cmd = new SQLiteCommand($"SELECT Id, LegId, PlayerId FROM {TableName} WHERE LegId = @legId AND PlayerId = @playerId", conn);
			cmd.Parameters.AddWithValue("@legId", legId);
			cmd.Parameters.AddWithValue("@playerId", playerId);

			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				var rec = await ReadRecord(reader);
				if (rec != null)
					return rec.Id;
			}

			return Guid.Empty;
		}

		public async Task<List<Guid>> GetLegsByPlayerId(Guid playerId)
		{
			var ret = new List<Guid>();
			if (Items == null || Items.Count == 0)
			{
				await ReadAll();
			}
			if (Items.Count > 0)
			{
				var lst = Items.Where(x => x.PlayerId == playerId);
				foreach (var mp in lst)
				{
					if(!ret.Contains(mp.Id))
						ret.Add(mp.LegId);
				}
					
			}
			return ret;
		}
	}

}