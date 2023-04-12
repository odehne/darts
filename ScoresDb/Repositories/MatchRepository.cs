using ScoresDb.Entities;
using ScoresDb.Models;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Numerics;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ScoresDb.Repositories
{

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
				ret.Datasets.Add(new LineChartDataSetModel { id = 1, label = "", data = totalDarts.ToArray(), borderColor = "rgb(255, 140, 25)", backgroundColor = "rgba(255, 140, 25, 0.75)" });
			}

			ret.labels = lbls.ToArray();
            return ret;
		}

        public async Task<PlayerBestLegModel> GetBestLeg(Guid id, int startValue)
        {
            var ret = new PlayerBestLegModel();
            var player = await AllPlayers.GetById(id);
			var lowestThrowCount170 = 0;
            var lowestThrowCount301 = 0;
            var lowestThrowCount501 = 0;

			var lowestThrowsByPlayer170 = new AllThrowsEntity();
            var lowestThrowsByPlayer301 = new AllThrowsEntity();
            var lowestThrowsByPlayer501 = new AllThrowsEntity();

			if (player != null)
			{

				ret.PlayerId = player.Id.ToString();
				ret.PlayerName = player.Name;

				var legs = await AllLegs.GetLegsByPlayerId(player.Id);

				foreach (var leg in legs)
				{
					if(leg.StartValue == startValue)
					{
                        switch (leg.StartValue)
                        {
                            case 170:
                                var allThrowsByPlayer170 = await AllThrows.GetThrowsByLegIdAndPlayerId(leg.Id, player.Id);
                                var dc170 = allThrowsByPlayer170.Count() * 3;
                                if (lowestThrowCount170 > dc170 | lowestThrowCount170 == 0)
                                {
                                    lowestThrowCount170 = dc170;
                                    lowestThrowsByPlayer170 = allThrowsByPlayer170;
                                }

                                break;
                            case 301:
                                var allThrowsByPlayer301 = await AllThrows.GetThrowsByLegIdAndPlayerId(leg.Id, player.Id);
                                var dc301 = allThrowsByPlayer301.Count() * 3;
                                if (lowestThrowCount301 > dc301 | lowestThrowCount301 == 0)
                                {
                                    lowestThrowCount301 = dc301;
                                    lowestThrowsByPlayer301 = allThrowsByPlayer301;
                                }
                                break;
                            case 501:
                                var allThrowsByPlayer501 = await AllThrows.GetThrowsByLegIdAndPlayerId(leg.Id, player.Id);
                                var dc501 = allThrowsByPlayer501.Count() * 3;
                                if (lowestThrowCount501 > dc501 | lowestThrowCount301 == 0)
                                {
                                    lowestThrowCount501 = dc501;
                                    lowestThrowsByPlayer501 = allThrowsByPlayer501;
                                }
                                break;
                        }
                    }
				}
			}

            switch (startValue)
            {
                case 170:
                    ret.labels = lowestThrowsByPlayer170.GetLabels();
                    ret.Datasets.Add(new LineChartDataSetModel { id = 1, label = "", data = lowestThrowsByPlayer170.GetThrows(), borderColor = "rgb(255, 140, 25)", backgroundColor = "rgba(255, 140, 25, 0.75)" });
                    break;
                case 301:
                    ret.labels = lowestThrowsByPlayer301.GetLabels();
                    ret.Datasets.Add(new LineChartDataSetModel { id = 1, label = "", data = lowestThrowsByPlayer301.GetThrows(), borderColor = "rgb(255, 140, 25)", backgroundColor = "rgba(255, 140, 25, 0.75)" });
                    break;
                case 501:
                    ret.labels = lowestThrowsByPlayer501.GetLabels();
                    ret.Datasets.Add(new LineChartDataSetModel { id = 1, label = "", data = lowestThrowsByPlayer501.GetThrows(), borderColor = "rgb(255, 140, 25)", backgroundColor = "rgba(255, 140, 25, 0.75)" });
                    break;
            }

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
                ret.Datasets.Add(new LineChartDataSetModel { id = 1, label = "", data = avgs.ToArray(), borderColor = "rgb(255, 140, 25)", backgroundColor = "rgba(255, 140, 25, 0.75)" });
            }

            ret.labels = lbls.ToArray();
            return ret;
        }

        public async Task<List<LegEntity>> GetLastTenMatchesByPlayerId(Guid playerId, int startValue)
        {
            var ret = new List<LegEntity>();
			var found = 0;

            Items = new List<MatchEntity>();
            await ReadAll();

			var lastTenMatches = Items.OrderByDescending(x => x.PlayedAt);
            foreach (var match in lastTenMatches)
            {
                foreach (var set in match.Sets)
                {
					foreach (var leg in set.Legs)
                    {
						if(leg.StartValue == startValue)
						{
                            foreach (var thr in leg.ThrowsOfPlayers)
                            {
                                if (thr.PlayerId == playerId)
                                {
									found++;
                                    ret.Add(leg);
                                    break;
                                }
                            }
                        }
						if (found >= 10)
							return ret;
                    }
                }
            }
            return ret;
        }

        public async Task<List<LegEntity>> GetAllMatchesByDateAndPlayerId(Guid playerId, DateTime startDate, int startValue)
		{
			var ret = new List<LegEntity>();
            var legs = await AllLegs.GetAllWonLegs(playerId);
			var found = false;

            Items = new List<MatchEntity>();
            await ReadAll();

			var lastWeeksMatches = Items.Where(m => m.PlayedAt >= startDate).ToList();
			foreach (var match in lastWeeksMatches)
			{
				foreach(var set in match.Sets)
				{
					found = false;
					foreach(var leg in set.Legs)
					{
						if (leg.StartValue == startValue)
						{
                            foreach (var thr in leg.ThrowsOfPlayers)
                            {
                                if (thr.PlayerId == playerId)
                                {
                                    ret.Add(leg);
                                    found = true;
                                    break;
                                }
                            }
		                }
	                    if (found)
                          break;
                    }
                }
			}
			return ret;

        }


        public async Task<PlayerDartsPerLegModel> CountHighestThrows(Guid playerId, int startValue)
        {
            var ret = new PlayerDartsPerLegModel();
            var player = await AllPlayers.GetById(playerId);
            var lastTenMatches = await GetLastTenMatchesByPlayerId(playerId, startValue);
            var lbls = new List<string>();
            var overEigthy = new List<int>();
            var legAvg = 0;
            var index = 0;

            if (player != null)
            {

                ret.PlayerId = player.Id.ToString();
                ret.PlayerName = player.Name;

				for (int i = 0; i < lastTenMatches.Count(); i++)
				{
			        lbls.Add(index.ToString());
                    var allThrowsByPlayer = await AllThrows.GetThrowsByLegIdAndPlayerId(lastTenMatches[i].Id, player.Id);
                	overEigthy.Add(CountHighestThrows(allThrowsByPlayer, 80));
                    index++;
                }

                ret.Datasets.Add(new LineChartDataSetModel { id = 1, label = ">= 80", data = overEigthy.ToArray(), borderColor = "rgb(255, 140, 25)", backgroundColor = "rgba(255, 140, 25, 0.75)" });

            }

            ret.labels = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            return ret;
        }

        public async Task<PlayerDartsPerLegModel> GetOneWeekTrend(Guid playerId, int startValue)
		{
			var ret = new PlayerDartsPerLegModel();
            var player = await AllPlayers.GetById(playerId);
            var lastWeeksWonLegs = await GetAllMatchesByDateAndPlayerId(playerId, DateTime.Now.AddDays(-7), startValue);
			var lbls = new List<string>();
			var avgs = new List<int>();
            var highscores = new List<int>();
            var legAvg = 0;
			var index = 0;

			if (player != null)
			{
			
				ret.PlayerId = player.Id.ToString();
				ret.PlayerName = player.Name;

				foreach (var leg in lastWeeksWonLegs)
				{
					index++;
                    lbls.Add(index.ToString());
                    var allThrowsByPlayer = await AllThrows.GetThrowsByLegIdAndPlayerId(leg.Id, player.Id);
                    legAvg = allThrowsByPlayer.Sum(x => x.Throw) / (allThrowsByPlayer.Count - 1);
                    highscores.Add(GetHighestThrow(allThrowsByPlayer));
                    avgs.Add(legAvg);
				}
				if(avgs.Count > 0)
				{
                    ret.Datasets.Add(new LineChartDataSetModel { id = 1, label = "Averages", data = avgs.ToArray(), borderColor = "rgb(255, 140, 25)", backgroundColor = "rgba(255, 140, 25, 0.75)" });
                    ret.Datasets.Add(new LineChartDataSetModel { id = 1, label = "Highscores", data = highscores.ToArray(), borderColor = "rgb(179,25,255)", backgroundColor = "rgba(179,25,255, 0.75)" });
                }
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

				var legs = await AllLegs.GetLegsByPlayerId(player.Id).ConfigureAwait(false);

				var wonLegsCount = legs.Where(x => x.LegWinner == player.Id).Count();
				var lostLegsCount = legs.Where(x => x.LegWinner != player.Id).Count();

				ret.Datasets.Add(new PieChartDataSetModel
				{
					id = 1,
					label = "Gewonnen",
					data = new int[] { wonLegsCount, lostLegsCount },
					borderColor = new string[] { "rgba(255, 140, 25, 0.75)", "rgba(179,89,0, 0.5)" },
					backgroundColor = new string[] { "rgba(255, 140, 25, 0.75)", "rgba(179,89,0, 0.5)" }
				});
                //ret.Datasets.Add(new DataSetModel { id = 2, label = "Verloren", data = new int[] { lostLegsCount }, borderColor = "rgb(0, 85, 235)", backgroundColor = "rgba(0, 85, 235, 0.5)" });

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
                            ret.WonCheckouts.Add(playerThrows.Last().Throw);
					}
				}

			}
			return ret;
		}

        public async Task<PlayerDetailsModel> GetPlayerDetails(Guid id)
        {
            var model = new PlayerDetailsModel();
            var player = await AllPlayers.GetById(id);
            var avgs170 = new List<int>();
            var avgs301 = new List<int>();
            var avgs501 = new List<int>();
            var dartCountPerLegs301 = new List<int>();
            var dartCountPerLegs501 = new List<int>();
            var dartCountPerLegs170 = new List<int>();
            var bestLeg170 = new List<int>();
            var bestLeg301 = new List<int>();
            var bestLeg501 = new List<int>();
        
            var highscores170 = new List<int>();
            var highscores301 = new List<int>();
            var highscores501 = new List<int>();
            var checkouts170 = new List<int>();
            var checkouts301 = new List<int>();
            var checkouts501 = new List<int>();

            var allThrowsByPlayer170 = new AllThrowsEntity();
            var allThrowsByPlayer301 = new AllThrowsEntity();
            var allThrowsByPlayer501 = new AllThrowsEntity();

			var legAvg170 = 0;
			var legAvg301 = 0;
			var legAvg501 = 0;
			var dc170 = 0;
			var dc301 = 0;
			var dc501 = 0;

            if (player != null)
            {

                model.Id = player.Id.ToString();
                model.Name = player.Name;

                var legs = await AllLegs.GetLegsByPlayerId(player.Id);

                foreach (var leg in legs)
                {
                    
					switch (leg.StartValue)
					{
						case 170:
                            allThrowsByPlayer170 = await AllThrows.GetThrowsByLegIdAndPlayerId(leg.Id, player.Id);
                            legAvg170 = allThrowsByPlayer170.Sum(x => x.Throw) / (allThrowsByPlayer170.Count - 1);
                            dc170 = allThrowsByPlayer170.Count() * 3;
                            dartCountPerLegs170.Add(dc170);
                            avgs170.Add(legAvg170);
                            checkouts170.Add(allThrowsByPlayer170[allThrowsByPlayer170.Count - 1].Throw);
                            highscores170.Add(GetHighestThrow(allThrowsByPlayer170));
                            break;
                        case 301:
                            allThrowsByPlayer301 = await AllThrows.GetThrowsByLegIdAndPlayerId(leg.Id, player.Id);
                            legAvg301 = allThrowsByPlayer301.Sum(x => x.Throw) / (allThrowsByPlayer301.Count - 1);
                            dc301 = allThrowsByPlayer301.Count() * 3;
                            dartCountPerLegs301.Add(dc301);
                            avgs301.Add(legAvg301);
                            checkouts301.Add(allThrowsByPlayer301[allThrowsByPlayer301.Count - 1].Throw);
                            highscores301.Add(GetHighestThrow(allThrowsByPlayer301));
                            break;
                        case 501:
                            allThrowsByPlayer501 = await AllThrows.GetThrowsByLegIdAndPlayerId(leg.Id, player.Id);
                            legAvg501 = allThrowsByPlayer501.Sum(x => x.Throw) / (allThrowsByPlayer501.Count - 1);
                            dc501 = allThrowsByPlayer501.Count() * 3;
                            dartCountPerLegs501.Add(dc501);
                            avgs501.Add(legAvg501);
                            checkouts501.Add(allThrowsByPlayer501[allThrowsByPlayer501.Count - 1].Throw);
                            highscores501.Add(GetHighestThrow(allThrowsByPlayer501));
                            break;
                    }
                }

				var matches = await AllMatchPlayers.GetMatchesByPlayer(player.Id);

                if (avgs170.Count > 0)
				{
                    model.AllLegAvg170 = avgs170.Sum() / avgs170.Count;
                    model.BestLegAvg170 = avgs170.Max();
                }
				if (avgs301.Count > 0)
				{
					model.AllLegAvg301 = avgs301.Sum() / avgs301.Count;
					model.BestLegAvg301 = avgs301.Max();
				}
                if (avgs501.Count>0)
				{
                    model.AllLegAvg501 = avgs501.Sum() / avgs501.Count;
                    model.BestLegAvg501 = avgs501.Max();
                }

                if (dartCountPerLegs170.Count > 0)
                    model.BestDartCount170 = dartCountPerLegs170.Min();
                if (dartCountPerLegs301.Count > 0)
                    model.BestDartCount301 = dartCountPerLegs301.Min();
                if (dartCountPerLegs501.Count>0)
					model.BestDartCount501 = dartCountPerLegs501.Min();

                model.HighScore170 = highscores170.Max();
                model.HighScore301 = highscores301.Max();
                model.HighScore501 = highscores501.Max();
                model.HighestCheckout170 = checkouts170.Max();
                model.HighestCheckout301 = checkouts301.Max();
                model.HighestCheckout501 = checkouts501.Max();

                model.LegCount170 = legs.Where(x => x.StartValue == 170).Count();
                model.LegCount301 = legs.Where(x => x.StartValue == 301).Count();
                model.LegCount501 = legs.Where(x => x.StartValue == 501).Count();
                model.MatchCount = matches.Count();

            }

            return model;
        }

        public int CountHighestThrows(AllThrowsEntity allThrowsByPlayer, int minimum)
        {
            var count = 0;

            foreach (var thr in allThrowsByPlayer)
            {
                if (thr.Throw >= minimum)
                    count++;
            }
            return count;
        }

        public int GetHighestThrow(AllThrowsEntity allThrowsByPlayer)
		{
			var highest = 0;
			foreach (var thr in allThrowsByPlayer)
			{
				if(thr.Throw > highest)
					highest = thr.Throw;
			}
			return highest;
		}

    }

}