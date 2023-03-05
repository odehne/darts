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
		private readonly IThrowsRepository _throwRepo;

		public ObjectRepositoryController(ILogger<ObjectRepositoryController> logger, 
			IMatchRepository gameRepo,
			IThrowsRepository throwRepo,
			IPlayersRepository playerRepo)
		{
			_logger = logger;
			_playerRepo = playerRepo;
			_gameRepo = gameRepo;
			_throwRepo = throwRepo;
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
		[ProducesResponseType(typeof(IEnumerable<PlayerEntity>), StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<PlayerEntity>>> GetPlayers()
		{
			var lst = await _playerRepo.GetItems();
			return Ok(lst);
		}

		[HttpGet]
		[Route("/players/{playerId}")]
		[ProducesResponseType(typeof(PlayerEntity), StatusCodes.Status200OK)]
		public async Task<ActionResult<PlayerEntity>> GetPlayer(string playerId)
		{
			var lst = await _playerRepo.GetById(Guid.Parse(playerId));
			return Ok(lst);
		}

		[HttpGet]
		[Route("/players/{playerId}/checkouts/history")]
		[ProducesResponseType(typeof(PlayerCheckoutHistoryModel), StatusCodes.Status200OK)]
		public async Task<ActionResult<PlayerCheckoutHistoryModel>> GetCheckOutHistory(string playerId)
		{
			var ret = await _gameRepo.GetCheckoutHistory(Guid.Parse(playerId));
			var lbls = new List<string>();
			
			for (int i = 1; i < ret.AllCheckouts.Count; i++)
			{
				lbls.Add(i.ToString());
			}

			ret.labels = lbls.ToArray();
			ret.Datasets.Add(new DataSetModel { data = ret.WonCheckouts.ToArray(), id = 1, label = "Gewonnene Legs", borderColor = "rgb(255, 99, 132)", backgroundColor = "rgba(255, 99, 132, 0.5)" });
			ret.Datasets.Add(new DataSetModel { data = ret.AllCheckouts.ToArray(), id = 2, label = "All Legs", borderColor = "rgb(0, 85, 235)", backgroundColor = "rgba(0, 85, 235, 0.5)" });

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
			var lst = await _gameRepo.GetMatchesByPlayerId(Guid.Parse(playerId));

			var stats = new PlayerStatsModel(lst, player);
			return Ok(stats);
		}
	}
}