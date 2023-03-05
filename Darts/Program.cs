namespace Darts
{
	internal static class Program
	{
		

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			//var p1 = new Player("Paul");
			//var p2 = new Player("Papa");

			//TheGame = new Game(p1, p2, Bestofs.BestOfFive, 3, 501);
			
			//Console.WriteLine(TheGame.PossibleCheckouts.GetDartOut(170));

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			Application.Run(new Form1());
		}
	}
}