namespace ScoresDb.Models
{
    public class PlayerDetailsModel
	{
        public string Id { get; set; }
        public string Name { get; set; }        // Spieler Name
        public int MatchCount { get; set; }         // Anzahl aller Spiele
        public double AllLegAvg501 { get; set; }
        public double AllLegAvg301 { get; set; }
        public double AllLegAvg170 { get; set; } // Durchschnitt über alle 170'er Legs
        public double BestLegAvg501 { get; set; }
        public double BestLegAvg301 { get; set; }
        public double BestLegAvg170 { get; set; } // Bestes Leg (170)
        public int HighScore170 { get; set; }     // Höchster Wurf (170)  
        public int HighScore301 { get; set; }
        public int HighScore501 { get; set; }
        public int BestDartCount170 { get; set; }   // Geringste Wurfanzahl (170)
        public int BestDartCount301 { get; set; }
        public int BestDartCount501 { get; set; }
        public int HighestCheckout170 { get; set; } // Höchster Checkout (170)
        public int HighestCheckout301 { get; set; }
        public int HighestCheckout501 { get; set; }
   	    public int LegCount170 { get; set; }        // Anzahl der Legs (170)
        public int LegCount301 { get; set; }
        public int LegCount501 { get; set; }
    }

}
