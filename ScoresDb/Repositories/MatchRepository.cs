using ScoresDb.Entities;
using ScoresDb.Models;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Numerics;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ScoresDb.Repositories
{
    public interface IMatchRepository : IRepository<MatchEntity>
    {
		Task<List<MatchEntity>> GetMatchesByPlayerId(Guid playerId);
		Task<List<MatchEntity>> GetWonMatchesByPlayerId(Guid playerId);
		Task<DirectCompareModel> GetWonsAgainstPlayer(Guid player1Id, Guid player2Id);
		Task<PlayerCheckoutHistoryModel> GetCheckoutHistory(Guid id);
		Task<PlayerDartsPerLegModel> GetDartsAVGPerLeg(Guid id);

		Task<PlayerDartsPerLegModel> GetHowManyDartsPerLeg(Guid id);

		Task<WonLegsModel> GetAllPlayerLegs();

		Task<PlayedLegsModel> GetWonOrLostLegs(Guid id);
	}

	public class MatchRepository : IMatchRepository
	{
		public string TableName => "Games";
	
		public List<MatchEntity> Items { get; set; }
		public string ConnectionString { get; set; }

		public PlayersRepository AllPlayers { get; set; }
		public  SetsRepository AllSets { get; set; }
		public LegsRepository AllLegs { get; set; }
		public ThrowsRepository  AllThrows { get; set; }
		public LegPlayersRepository AllLegsAndPlayers { get; set; }
		public MatchPlayersReporsitory  AllMatchPlayers { get; set; }

		public MatchRepository(string connectionString)
		{
			//"Data Source=" + DbPath
			ConnectionString = connectionString;
			AllPlayers = new PlayersRepository(connectionString);
			AllThrows = new ThrowsRepository(connectionString);
			AllLegsAndPlayers = new LegPlayersRepository(connectionString, AllPlayers);
			AllLegs = new LegsRepository(connectionString, AllThrows, AllLegsAndPlayers);
			AllSets = new SetsRepository(connectionString, AllLegs);
			AllMatchPlayers = new MatchPlayersReporsitory(connectionString, AllPlayers);
		}

		public async Task<MatchEntity> SaveGame(MatchEntity theGame)
		{

			for (int i = 0; i < theGame.Players.Count-1; i++)
			{
				theGame.Players[i] = await AllPlayers.Create(theGame.Players[i]);
			}

			await AllMatchPlayers.AddMatch(theGame.Id, theGame.Players);

			var gResult = await Create(theGame);

			foreach (var set in theGame.Sets)
			{
				foreach (var leg in set.Legs)
				{
					foreach (var top in leg.ThrowsOfPlayers)
					{
						_ = await AllThrows.Create(top);  
					}
					_ = await AllLegs.Create(leg);
					foreach (var p in theGame.Players)
					{
						_ = await AllLegsAndPlayers.Create(new LegPlayerEntity { LegId = leg.Id, PlayerId = p.Id });
					}
				}
				_ = await AllSets.Create(set);
			}
			return theGame;
		}

		public async Task<MatchEntity> Create(MatchEntity item)
		{
			using (var connection = new SQLiteConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					// Erstellen der Tabelle, sofern diese noch nicht existiert.
					command.CommandText = $"INSERT OR REPLACE INTO {TableName} (id, GameWinnerId, WhoStarts, BestOfLegs, BestOfSets, GameDate) VALUES (@id, @GameWinnerId, @WhoStarts, @BestOfLegs, @BestOfSets, @GameDate);";
					command.Parameters.AddWithValue("@id", item.Id.ToString());
					command.Parameters.AddWithValue("@GameWinnerId", item.GameWinner.ToString());
					command.Parameters.AddWithValue("@WhoStarts", item.WhoStarts);
					command.Parameters.AddWithValue("@BestOfLegs", item.BestOfLegs);
					command.Parameters.AddWithValue("@BestOfSets", item.BestOfSets);
					command.Parameters.AddWithValue("@GameDate", DateTime.Now);
					command.ExecuteNonQuery();
					return await GetById(item.Id);	
				}
			}
		}

		public async Task<string> CreateTable()
		{
			try
			{
				var sql = $"CREATE TABLE \"{TableName}\" (\"Id\" TEXT, \"GameWinnerId\"\tTEXT, \"WhoStarts\" INTEGER, \"BestOfLegs\" INTEGER, \"BestOfSets\" INTEGER, \"GameDate\" TEXT, PRIMARY KEY(\"Id\") );";
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

		public async Task<bool> Delete(MatchEntity item)
		{
			var query = $"DELETE FROM [{TableName}] WHERE Id = @Id";
			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(query, conn);
			cmd.Parameters.AddWithValue("@Id", item.Id);
			cmd.ExecuteNonQuery();
			await ReadAll();
			return true;
		}

		public async Task<MatchEntity> GetById(Guid id)
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

		public Task<MatchEntity> GetByName(string name)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<MatchEntity>> GetItems()
		{
			Items = new List<MatchEntity>();
			await ReadAll();
			return Items;
		}

		public async Task<bool> ReadAll()
		{
			Items = new List<MatchEntity>();
			if (string.IsNullOrEmpty(ConnectionString))
				throw new ArgumentException("ConnectionString not set.");
			
			using var conn = new SQLiteConnection(ConnectionString);
			conn.Open();
			using var cmd = new SQLiteCommand($"SELECT Id, GameWinnerId, WhoStarts, BestOfLegs, BestOfSets, GameDate FROM {TableName}", conn);
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Items.Add(await ReadRecord(reader));
			}
			
			return true;
		}

		private async Task<MatchEntity> ReadRecord(SQLiteDataReader reader)
		{
			var ret = new MatchEntity();
			ret.Id = Guid.Parse(reader.GetString(reader.GetOrdinal("Id")));
			ret.GameWinner = Guid.Parse(reader.GetString(reader.GetOrdinal("GameWinnerId")));
			ret.WhoStarts = reader.GetInt32(reader.GetOrdinal("WhoStarts"));
			ret.BestOfLegs = reader.GetInt32(reader.GetOrdinal("BestOfLegs"));
			ret.BestOfSets = reader.GetInt32(reader.GetOrdinal("BestOfSets"));
			ret.PlayedAt = reader.GetDateTime(reader.GetOrdinal("GameDate"));
			ret.Sets = await AllSets.GetSets(ret.Id);	
			return ret;
		}

		public async Task<MatchEntity> Update(MatchEntity item)
		{
			return await Create(item);
		}

		public async Task<List<MatchEntity>> GetMatchesByPlayerId(Guid playerId)
		{
			var itms = await GetItems();
			var player = await AllPlayers.GetById(playerId);
			return itms.Where(p=>p.Players.Contains(player)).ToList();
		}

		public async Task<List<MatchEntity>> GetWonMatchesByPlayerId(Guid playerId)
		{
			var itms = await GetItems();
			return itms.Where(p => p.GameWinner == playerId).ToList();
		}

		public async Task<DirectCompareModel> GetWonsAgainstPlayer(Guid player1Id, Guid player2Id)
		{
			var dcr= new DirectCompareModel();
			var itms = await GetItems();
			var player1 = await AllPlayers.GetById(player1Id);
			var player2 = await AllPlayers.GetById(player2Id);

			var player1Matches = itms.Where(m => m.Players.Contains(player1)).ToList(); // & p.Player2.Id == player2Id | p.Player1.Id == player2Id & p.Player2.Id == player1Id).ToList();
			var directMatches = player1Matches.Where(m => m.Players.Contains(player2)).ToList();

			dcr.Player1Wins = directMatches.Count(x => x.GameWinner == player1Id);
			dcr.Player2Wins = directMatches.Count(x => x.GameWinner == player2Id);
			return dcr;
		}

		public async Task<WonLegsModel> GetAllPlayerLegs()
		{
			var players = await AllPlayers.GetItems();
			var lbls = new List<string>();
			var index = 0;
			var ret = new WonLegsModel();

			foreach (var player in players)
			{
				index++;
				lbls.Add(player.Name);
				var legs = await AllLegs.GetAllWonLegs(player.Id);
				var ds = new DataSetModel
				{
					label = player.Name,
					id = index,
					borderColor = "rgb(255, 99, 132)",
					backgroundColor = "rgba(255, 99, 132, 0.5)",
					data = new int[] { legs.Count }
				};
				ret.Datasets.Add(ds);
			}

			ret.labels = lbls.ToArray();
			return ret;
		}

		public async Task<PlayerDartsPerLegModel> GetHowManyDartsPerLeg(Guid id)
		{
			var ret = new PlayerDartsPerLegModel();
			var player = await AllPlayers.GetById(id);
			var lbls = new List<string>();
			var darts = 0;
			var index = 0;
			var totalDarts = new List<int>();

			if (player != null)
			{

				ret.PlayerId = player.Id.ToString();
				ret.PlayerName = player.Name;

				var legs = await AllLegs.GetLegsByPlayerId(player.Id);

				foreach (var leg in legs)
				{
					index++;
					lbls.Add(index.ToString());
					var allThrowsByPlayer = await AllThrows.GetThrowsByLegIdAndPlayerId(leg.Id, player.Id);
					darts = allThrowsByPlayer.Count * 3;
					totalDarts.Add(darts);
				}
				ret.Datasets.Add(new DataSetModel { id = 1, label = string.Join(',', lbls), data = totalDarts.ToArray(), borderColor = "rgb(255, 99, 132)", backgroundColor = "rgba(255, 99, 132, 0.5)" });
			}

			ret.labels = new[] { "Durchschnittliche Darts pro Leg" };
			return ret;
		}

		public async Task<PlayerDartsPerLegModel> GetDartsAVGPerLeg(Guid id)
		{
			var ret = new PlayerDartsPerLegModel();
			var player = await AllPlayers.GetById(id);
			var lbls = new List<string>();
			var avgs = new List<int>();
			var legAvg = 0;
			var index = 0;

			if (player != null)
			{
			
				ret.PlayerId = player.Id.ToString();
				ret.PlayerName = player.Name;

				var legs = await AllLegs.GetLegsByPlayerId(player.Id);

				foreach (var leg in legs)
				{
					index++;
					lbls.Add(index.ToString());
					var allThrowsByPlayer = await AllThrows.GetThrowsByLegIdAndPlayerId(leg.Id, player.Id);
					legAvg = allThrowsByPlayer.Sum(x => x.Throw) / (allThrowsByPlayer.Count - 1);
					avgs.Add(legAvg);
				}
				ret.Datasets.Add(new DataSetModel { id = 1, label = string.Join(',', lbls), data = avgs.ToArray(), borderColor = "rgb(255, 99, 132)", backgroundColor = "rgba(255, 99, 132, 0.5)" });
			}

			ret.labels = lbls.ToArray();
			return ret;
		}

		public async Task<PlayedLegsModel> GetWonOrLostLegs(Guid id)
		{
			var ret = new PlayedLegsModel();
			var player = await AllPlayers.GetById(id);
	
			ret.labels = new string[] { "Gewonnen", "Verloren" };

			if (player != null)
			{
				ret.PlayerId = player.Id.ToString();
				ret.PlayerName = player.Name;

				var legs = await AllLegs.GetLegsByPlayerId(player.Id);

				var wonLegsCount = legs.Where(x => x.LegWinner == player.Id).Count();
				var lostLegsCount = legs.Where(x => x.LegWinner != player.Id).Count();

				ret.Datasets.Add(new DataSetModel { id = 1, label = "Gewonnen", data = new int[] { wonLegsCount }, borderColor = "rgb(128, 235, 0)", backgroundColor = "rgba(128, 235, 0, 0.5)" });
				ret.Datasets.Add(new DataSetModel { id = 2, label = "Verloren", data = new int[] { lostLegsCount }, borderColor = "rgb(0, 85, 235)", backgroundColor = "rgba(0, 85, 235, 0.5)" });

			}
			return ret;
		}

		public async Task<PlayerCheckoutHistoryModel> GetCheckoutHistory(Guid id)
		{
			var ret = new PlayerCheckoutHistoryModel();
			var player = await AllPlayers.GetById(id);
			
			if (player != null)
			{
				ret.PlayerId = player.Id.ToString();
				ret.PlayerName = player.Name;

				var legs = await AllLegs.GetLegsByPlayerId(player.Id);

				foreach (var leg in legs)
				{
					var playerThrows = leg.GetThrows(player.Id);
					{
						if (leg.LegWinner == player.Id)
							ret.WonCheckouts.Add(playerThrows.Last().Balance);
						ret.AllCheckouts.Add(playerThrows.Last().Balance);
					}
				}

			}
			return ret;
		}
	}
}