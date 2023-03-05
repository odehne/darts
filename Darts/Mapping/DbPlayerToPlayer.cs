using ScoresDb.Entities;

namespace Darts.Mapping
{
	public class DbEntityMapper
	{

		public static PlayerEntity ToPlayerEntiy(Player player)
		{
			return new PlayerEntity
			{
				Name = player.Name,
				Id = player.Id,
				HighestCheckOut = player.HighestCheckOut,
				HighestScore = player.HighestScore
			};
		}

		public static MatchEntity ToGameEntity(Game game)
		{
			var entity = new MatchEntity
			{
				Id = game.Id,
				WhoStarts = game.WhoStarts,
				GameWinner= game.WinnerId,
				BestOfLegs = game.BestOfLegs,
				BestOfSets = game.BestOfSets,
				PlayedAt = DateTime.Today,
				StartValue = game.StartValue
			};

			foreach (var p in game.Players)
			{
				entity.Players.Add(ToPlayerEntiy(p));
			}

			var sets = new List<SetEntity>();
			
			foreach (var set in game.Sets)
			{
				if (set.SetWinner > 0)
				{
					var setWinnner = game.Players[set.SetWinner - 1].Id;
					var setStartPlayer = game.Players[set.WhoStarts - 1].Id;
					sets.Add(ToSetEntiy(set, setWinnner, setStartPlayer, game.Id, entity.Players.Select(x => x.Id).ToList()));
				}
			}

			entity.Sets = sets;
			return entity;
		}

		public static SetEntity ToSetEntiy(Set set, Guid setWinner, Guid whoStarted, Guid GameId, List<Guid> playerIds)
		{
			var entity = new SetEntity
			{
				Id = set.Id,
				SetIndex = set.Index,
				BestOfLegs = set.BestOfLegs,
				StartValue = set.StartValue,
				GameId = GameId,
				SetWinner= setWinner,
				WhoStarts=whoStarted
			};

			var legs = new List<LegEntity>();

			foreach (var leg in set.Legs)
			{
				if (leg.LegWinner > 0)
				{
					var legWinner = playerIds[leg.LegWinner - 1];
					var legEntity = ToLegEntiy(leg, set.Id, legWinner, whoStarted);
					legs.Add(legEntity);
				}
			}
			entity.Legs = legs;

			return entity;
		}

		public static LegEntity ToLegEntiy(Leg leg, Guid setId, Guid legWinner, Guid whoStarted)
		{
			var entity = new LegEntity
			{
				Id = leg.Id,
				LegIndex = leg.Index,
				SetId= setId,
				StartValue= leg.StartValue,
				WhoStarts = whoStarted,
				LegWinner = legWinner,
			};

			var i = 0;
			foreach (var playerId in leg.PlayerIds)
			{
				var playerThrows = new AllThrowsEntity(playerId, leg.Id);

				foreach (var thrs in leg.ThrowsOfPlayers.Where(x=>x.PlayerId == playerId))
				{
					foreach (var thr in thrs)
						playerThrows.Add(new ThrowEntity { Balance = thr.Balance, Id = thr.Id, LegId = leg.Id, PlayerId = playerId, Throw = thr.Throw, ThrowIndex = thr.Index });
				}
				entity.ThrowsOfPlayers.Add(playerThrows);
			}
			return entity;
		}

		public static ThrowEntity ToThrowEntiy(SingleThrow th, Guid legId, Guid playerId)
		{
			var entity = new ThrowEntity
			{
				Id = th.Id,
				ThrowIndex = th.Index,
				Balance= th.Balance,
				Throw = th.Throw,
				LegId = legId,
				PlayerId = playerId
			};
			
			return entity;
		}

		internal static Player FromPlayerEntiy(PlayerEntity p1)
		{
			return new Player
			{
				Id = p1.Id,
				Name = p1.Name,
				HighestCheckOut = p1.HighestCheckOut,
				HighestScore = p1.HighestScore,
				TotalAvg = p1.TotalAvg,
				GameAvg = p1.GameAvg
			};

		}
	}
}
