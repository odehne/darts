using ScoresDb.Entities;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SQLite;

namespace ScoresDb.Repositories
{
	public interface IMatchPlayersRepository : IRepository<MatchPlayerEntity>
	{

		Task<List<PlayerEntity>> GetPlayersByMatch(Guid matchId);
		Task<bool> AddMatch(Guid matchId, List<PlayerEntity> players);

		Task<Guid> GetIdByMatchAndPlayer(Guid matchId, Guid playerId);
	}

	public class MatchPlayersReporsitory : IMatchPlayersRepository
	{
		public string TableName => "MatchPlayers";

		public List<MatchPlayerEntity> Items { get; set; }
		public string ConnectionString { get; set; }

		public IPlayersRepository _playersRepo;

		public MatchPlayersReporsitory(string connectionString, IPlayersRepository playersRepo)
		{
			ConnectionString = connectionString;
			_playersRepo = playersRepo;
		}

		public async Task<MatchPlayerEntity> Create(MatchPlayerEntity item)
		{
			using (var connection = new SQLiteConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					// Erstellen der Tabelle, sofern diese noch nicht existiert.
					command.CommandText = $"INSERT OR REPLACE INTO {TableName} (id, MatchId, PlayerId) VALUES (@id, @MatchId, @PlayerId);";
					command.Parameters.AddWithValue("@id", item.Id.ToString());
					command.Parameters.AddWithValue("@MatchId", item.MatchId.ToString());
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
				var sql = $"CREATE TABLE \"{TableName}\" (\"Id\" TEXT,  \"MatchId\" TEXT, \"PlayerId\" TEXT, PRIMARY KEY(\"Id\"));";
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

		public async Task<bool> Delete(MatchPlayerEntity item)
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

		public async Task<MatchPlayerEntity> GetById(Guid id)
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

		public Task<MatchPlayerEntity> GetByName(string name)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<MatchPlayerEntity>> GetItems()
		{
			Items = new List<MatchPlayerEntity>();
			await ReadAll();
			return Items;
		}

		public async Task<List<PlayerEntity>> GetPlayersByMatch(Guid matchId)
		{
			var ret = new List<PlayerEntity>();
			if (Items == null || Items.Count == 0)
			{
				await ReadAll();
			}
			if (Items.Count > 0)
			{
				var lst = Items.Where(x => x.MatchId == matchId);
				foreach (var mp in lst)
					ret.Add( await  _playersRepo.GetById(mp.PlayerId));
			}
			return ret;
		}

		public async Task<bool> ReadAll()
		{
			Items = new List<MatchPlayerEntity>();
			if (string.IsNullOrEmpty(ConnectionString))
				throw new ArgumentException("ConnectionString not set.");

			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			using var cmd = new SQLiteCommand($"SELECT Id, MatchId, PlayerId FROM {TableName} ORDER BY Id", conn);
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Items.Add(await ReadRecord(reader));
			}

			return true;
		}

		private async Task<MatchPlayerEntity> ReadRecord(SQLiteDataReader reader)
		{
			var ret = new MatchPlayerEntity();
			ret.Id = Guid.Parse(reader.GetString(reader.GetOrdinal("Id")));
			ret.PlayerId = Guid.Parse(reader.GetString(reader.GetOrdinal("PlayerId")));
			ret.MatchId = Guid.Parse(reader.GetString(reader.GetOrdinal("MatchId")));
			return ret;
		}

		public async Task<MatchPlayerEntity> Update(MatchPlayerEntity item)
		{
			return await Create(item);
		}

		public async Task<bool> AddMatch(Guid matchId, List<PlayerEntity> players)
		{
			foreach (var player in players)
			{
				var id = await GetIdByMatchAndPlayer(matchId, player.Id);
				if(id==Guid.Empty)
					await Create(new MatchPlayerEntity { MatchId = matchId, PlayerId = player.Id });
			}
			return true;
		}

		public async Task<Guid> GetIdByMatchAndPlayer(Guid matchId, Guid playerId)
		{
			
			if (string.IsNullOrEmpty(ConnectionString))
				throw new ArgumentException("ConnectionString not set.");

			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			using var cmd = new SQLiteCommand($"SELECT Id, MatchId, PlayerId FROM {TableName} WHERE MatchId = @matchId AND PlayerId = @playerId", conn);
			cmd.Parameters.AddWithValue("@matchId", matchId);
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
	}
}