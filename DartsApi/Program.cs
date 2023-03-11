using Microsoft.Extensions.DependencyInjection;
using ScoresDb.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPlayersRepository, PlayersRepository>(s => new PlayersRepository("Data Source=c:\\temp\\darts.db"));
builder.Services.AddSingleton<ILegPlayersRepository, LegPlayersRepository>(s => new LegPlayersRepository("Data Source=c:\\temp\\darts.db", s.GetRequiredService<IPlayersRepository>()));
builder.Services.AddSingleton<IThrowsRepository, ThrowsRepository>(s => new ThrowsRepository("Data Source=c:\\temp\\darts.db"));
builder.Services.AddSingleton<ILegsRepository, LegsRepository>(s => new LegsRepository("Data Source=c:\\temp\\darts.db", s.GetRequiredService<IThrowsRepository>(), s.GetRequiredService<ILegPlayersRepository>()));
builder.Services.AddSingleton<IMatchRepository, MatchRepository>(s=> new MatchRepository("Data Source=c:\\temp\\darts.db"));
builder.Services.AddSingleton<ISetsRepository, SetsRepository>(s => new SetsRepository("Data Source=c:\\temp\\darts.db", s.GetRequiredService<ILegsRepository>()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseCors(x => x.AllowAnyMethod()
				  .AllowAnyHeader()
				  .SetIsOriginAllowed(origin => true) // allow any origin
				  .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
