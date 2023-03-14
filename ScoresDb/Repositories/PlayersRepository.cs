using ScoresDb.Entities;
using ScoresDb.Models;
using System.Data.SQLite;

namespace ScoresDb.Repositories
{
    public interface IPlayersRepository : IRepository<PlayerEntity>
    {
        public Task<SideBarData> GetSideBarData();

    }

    public class PlayersRepository : IPlayersRepository
	{
		public string TableName => "Players";

		public List<PlayerEntity> Items { get; set; }
		public string ConnectionString { get; set; }

		public PlayersRepository(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public async Task<PlayerEntity> Create(PlayerEntity item)
		{
			var existingPlayer = await GetByName(item.Name);

			if (existingPlayer != null)
			{
				item.Id = existingPlayer.Id;
			}

			using (var connection = new SQLiteConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					// Erstellen der Tabelle, sofern diese noch nicht existiert.
					command.CommandText = $"INSERT OR REPLACE INTO {TableName} (id, Name, TotalAvg, GameAvg, HighestScore, HighestCheckout) VALUES (@id, @Name, @TotalAvg, @GameAvg, @HighestScore, @HighestCheckout);";
					command.Parameters.AddWithValue("@id", item.Id.ToString());
					command.Parameters.AddWithValue("@Name", item.Name);
					command.Parameters.AddWithValue("@TotalAvg", item.TotalAvg);
					command.Parameters.AddWithValue("@GameAvg", item.GameAvg );
					command.Parameters.AddWithValue("@HighestScore", item.HighestScore);
					command.Parameters.AddWithValue("@HighestCheckout", item.HighestCheckOut);
					command.ExecuteNonQuery();
					return await GetById(item.Id);
				}
			}
		}


		public async Task<string> CreateTable()
		{
			try
			{ 
				var sql = $"CREATE TABLE \"{TableName}\" (\"Id\"	TEXT, \"Name\" TEXT, \"TotalAvg\" REAL, \"GameAvg\" REAL, \"HighestScore\" INTEGER, \"HighestCheckout\" INTEGER, PRIMARY KEY(\"Id\"));";
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

		public async Task<bool> Delete(PlayerEntity item)
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

		public async Task<PlayerEntity> GetById(Guid id)
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

		public async Task<PlayerEntity> GetByName(string name)
		{
			if (Items == null || Items.Count == 0)
			{
				var b = await ReadAll();
				if (!b)
					return null;
			}
			var itm = Items.FirstOrDefault(x => x.Name == name);
			if (itm == null)
			{
				if (await ReadAll())
					itm = Items.FirstOrDefault(x => x.Name == name);
			}
			return itm;
		}

		public async Task<IEnumerable<PlayerEntity>> GetItems()
		{
			Items = new List<PlayerEntity>();
			await ReadAll();
			return Items;
		}

		public async Task<bool> ReadAll()
		{
			Items = new List<PlayerEntity>();
			if (string.IsNullOrEmpty(ConnectionString))
				throw new ArgumentException("ConnectionString not set.");
			try
			{
				using var conn = new SQLiteConnection(ConnectionString);
				conn.Open();
				using var cmd = new SQLiteCommand($"SELECT id, Name, TotalAvg, GameAvg, HighestScore, HighestCheckout FROM {TableName}", conn);
				var reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					Items.Add(await ReadRecord(reader));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				throw;
			}
		

			return true;
		}

		private async Task<PlayerEntity> ReadRecord(SQLiteDataReader reader)
		{
			var ret = new PlayerEntity();
			ret.Id = Guid.Parse(reader.GetString(reader.GetOrdinal("Id")));
			ret.Name = reader.GetString(reader.GetOrdinal("Name"));

			ret.TotalAvg = reader.GetDouble(reader.GetOrdinal("TotalAvg"));

			ret.GameAvg = reader.GetDouble(reader.GetOrdinal("GameAvg"));

			ret.HighestScore = reader.GetInt32(reader.GetOrdinal("HighestScore"));
			ret.HighestCheckOut = reader.GetInt32(reader.GetOrdinal("HighestCheckout"));

			return ret;

		}

		public async Task<PlayerEntity> Update(PlayerEntity item)
		{
			return await Create(item);
		}

        public async Task<SideBarData> GetSideBarData()
        {
            var lst = await GetItems();
			var ret = new SideBarData();

            ret.Players.Add(new PlayerModel { Id = "home", Name = "Home", Path = "/" });

            foreach (var item in lst)
			{
				ret.Players.Add(new PlayerModel { Id = item.Id.ToString(), Name = item.Name, Path = "/Player/" + item.Id.ToString() });
			}

			return ret;
        }
    }
}