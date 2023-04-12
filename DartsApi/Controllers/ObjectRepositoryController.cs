using DartsApi.Models;
using Microsoft.AspNetCore.Mvc;
using ScoresDb.Entities;
using ScoresDb.Models;
using ScoresDb.Repositories;
using System.Data.Entity.Core.Metadata.Edm;

namespace DartsApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ObjectRepositoryController : ControllerBase
	{
		private readonly ILogger<ObjectRepositoryController> _logger;
		private readonly IMatchRepository _gameRepo;
		private readonly IPlayersRepository _playerRepo;
        private readonly ILegsRepository _legsRepo;
        private readonly IThrowsRepository _throwRepo;

		public ObjectRepositoryController(ILogger<ObjectRepositoryController> logger, 
			IMatchRepository gameRepo,
			IThrowsRepository throwRepo,
			IPlayersRepository playerRepo,
            ILegsRepository legsRepo)
		{
			_logger = logger;
			_playerRepo = playerRepo;
			_gameRepo = gameRepo;
			_throwRepo = throwRepo;
			_legsRepo = legsRepo;
		}

		[HttpGet]
		[Route("/games")]
		[ProducesResponseType(typeof(IEnumerable<MatchEntity>), StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<MatchEntity>>> GetGames()
		{
			var lst = await _gameRepo.GetItems();
			return Ok(lst);
		}

		[HttpGet]
		[Route("/games/{gameId}")]
		[ProducesResponseType(typeof(MatchEntity), StatusCodes.Status200OK)]
		public async Task<ActionResult<MatchEntity>> GetGame(string gameId)
		{
			var lst = await _gameRepo.GetById(Guid.Parse(gameId));
			return Ok(lst);
		}

		[HttpGet]
		[Route("/players")]
		[ProducesResponseType(typeof(SideBarData), StatusCodes.Status200OK)]
		public async Task<ActionResult<SideBarData>> GetPlayers()
		{
			var lst = await _playerRepo.GetSideBarData();

			return Ok(lst);
		}

		[HttpGet]
		[Route("/players/{playerId}")]
		[ProducesResponseType(typeof(PlayerDetailsModel), StatusCodes.Status200OK)]
		public async Task<ActionResult<PlayerDetailsModel>> GetPlayer(string playerId)
		{
			var details = await _gameRepo.GetPlayerDetails(Guid.Parse(playerId));

			return Ok(details);
		}
		                     
		[HttpGet]
		[Route("/players/{playerId}/checkouts/history")]
		[ProducesResponseType(typeof(PlayerCheckoutHistoryModel), StatusCodes.Status200OK)]
		public async Task<ActionResult<PlayerCheckoutHistoryModel>> GetCheckOutHistory(string playerId)
		{
			var ret = await _gameRepo.GetCheckoutHistory(Guid.Parse(playerId));
			var lbls = new List<string>();
			
			for (int i = 1; i < ret.WonCheckouts.Count; i++)
			{
				lbls.Add(i.ToString());
			}

			ret.labels = lbls.ToArray();
			ret.Datasets.Add(new DataSetModel { data = ret.WonCheckouts.ToArray(), id = 1, label = "", borderColor = "rgb(255, 140, 25)", backgroundColor = "rgba(255, 140, 25, 0.75)" });
		
			return Ok(ret);
		}

		[HttpGet]
		[Route("/players/{playerId}/legs/winsandlosses")]
		[ProducesResponseType(typeof(PlayerDartsPerLegModel), StatusCodes.Status200OK)]
		public async Task<ActionResult<PlayedLegsModel>> GetWonOrLostLegs(string playerId)
		{
			var ret = await _gameRepo.GetWonOrLostLegs(Guid.Parse(playerId));
			return Ok(ret);
		}

		[HttpGet]
		[Route("/players/{playerId}/legs/avg")]
		[ProducesResponseType(typeof(PlayerDartsPerLegModel), StatusCodes.Status200OK)]
		public async Task<ActionResult<PlayerDartsPerLegModel>> GetPlayersLegAvg(string playerId)
		{
			var ret = await _gameRepo.GetDartsAVGPerLeg(Guid.Parse(playerId));
			return Ok(ret);
		}

        [HttpGet]
        [Route("/players/{playerId}/legs/{startvalue}/bestleg")] //https://localhost:7141/players/fb3fc2b2-a01b-4dd6-99e9-838262a8a614/legs/170bestleg
        [ProducesResponseType(typeof(PlayerBestLegModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<PlayerBestLegModel>> GetPlayersBestLeg(string playerId, int startvalue)
        {
            var ret = await _gameRepo.GetBestLeg(Guid.Parse(playerId), startvalue);
            return Ok(ret);
        }

        [HttpGet]
        [Route("/players/{playerId}/legs/{startvalue}/lastweekstrend")]
        [ProducesResponseType(typeof(PlayerDartsPerLegModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<PlayerDartsPerLegModel>> GetOneWeekTrend(string playerId, int startvalue)
        {
            var ret = await _gameRepo.GetOneWeekTrend(Guid.Parse(playerId), startvalue);
            return Ok(ret);
        }

        [HttpGet]
        [Route("/players/{playerId}/legs/{startvalue}/counthighthrows")]
        [ProducesResponseType(typeof(PlayerDartsPerLegModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<PlayerDartsPerLegModel>> GetHighThrows(string playerId, int startvalue)
        {
            var ret = await _gameRepo.CountHighestThrows(Guid.Parse(playerId), startvalue);
            return Ok(ret);
        }

        [HttpGet]
		[Route("/players/{playerId}/legs/darts")]
		[ProducesResponseType(typeof(PlayerDartsPerLegModel), StatusCodes.Status200OK)]
		public async Task<ActionResult<PlayerDartsPerLegModel>> GetHowManyDartsPerLeg(string playerId)
		{
			var ret = await _gameRepo.GetHowManyDartsPerLeg(Guid.Parse(playerId));
			return Ok(ret);
		}

     

        [HttpGet]
		[Route("/players/legs")]
		[ProducesResponseType(typeof(WonLegsModel), StatusCodes.Status200OK)]
		public async Task<ActionResult<WonLegsModel>> GetAllPlayerLegs()
		{
			var ret = await _gameRepo.GetAllPlayerLegs();
			return Ok(ret);
		}


		[HttpGet]
		[Route("/players/{playerId}/statistics")]
		[ProducesResponseType(typeof(PlayerStatsModel), StatusCodes.Status200OK)]
		public async Task<ActionResult<PlayerStatsModel>> GetPlayerStats(string playerId)
		{
			var player = await _playerRepo.GetById(Guid.Parse(playerId));
            var allMatches = await _gameRepo.GetMatchesByPlayerId(Guid.Parse(playerId));
            var allLegs = await _legsRepo.GetLegsByPlayerId(Guid.Parse(playerId));

            var stats = new PlayerStatsModel(allLegs, allMatches, player);
			return Ok(stats);
		}
	}
}