using ScoresDb.Entities;
using ScoresDb.Models;

namespace ScoresDb.Repositories
{
    public interface IMatchRepository : IRepository<MatchEntity>
    {
		Task<List<MatchEntity>> GetMatchesByPlayerId(Guid playerId);
		Task<List<MatchEntity>> GetWonMatchesByPlayerId(Guid playerId);
		Task<DirectCompareModel> GetWonsAgainstPlayer(Guid player1Id, Guid player2Id);
		Task<PlayerCheckoutHistoryModel> GetCheckoutHistory(Guid id);
		Task<PlayerDetailsModel> GetPlayerDetails(Guid id);
        Task<PlayerDartsPerLegModel> GetDartsAVGPerLeg(Guid id);
        Task<PlayerBestLegModel> GetBestLeg(Guid id, int startValue);

        Task<PlayerDartsPerLegModel> GetHowManyDartsPerLeg(Guid id);

		Task<WonLegsModel> GetAllPlayerLegs();

		Task<PlayedLegsModel> GetWonOrLostLegs(Guid id);

        Task<List<LegEntity>> GetAllMatchesByDateAndPlayerId(Guid playerId, DateTime startDate, int startValue);

        Task<PlayerDartsPerLegModel> GetOneWeekTrend(Guid playerId, int startValue);

    }

}