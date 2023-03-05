using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Darts
{
	public partial class NewGame : Form
	{
		public NewGameModel Details { get; set; }

		public NewGame()
		{
			InitializeComponent();
		}

		public void SetData(NewGameModel data)
		{

		}

		public NewGameModel GetData()
		{
			var data = new NewGameModel();

			var bestOfLegs = Bestofs.BestOfThree;
			var bestOfSets = Bestofs.BestOfFive;

			switch (cmbBestOf.Text)
			{
				case "Best of 3":
					bestOfSets = Bestofs.BestOfThree;
					break;
				case "Best of 5":
					bestOfSets = Bestofs.BestOfFive;
					break;
				case "Best of 7":
					bestOfSets = Bestofs.BestOfSeven;
					break;
				case "Best of 11":
					bestOfSets = Bestofs.BestOfEleven;
					break;
				case "Best of 13":
					bestOfSets = Bestofs.BestOfThirteen;
					break;
			}

			switch (cmbBestOfLegs.Text)
			{
				case "Best of 3":
					bestOfLegs = Bestofs.BestOfThree;
					break;
				case "Best of 5":
					bestOfLegs = Bestofs.BestOfFive;
					break;
			}

			int startValue;
			_ = int.TryParse(cmbStartValue.Text, out startValue);

			//data.Player1 = new Player(txtPlayer1.Text);
			//data.Player2 = new Player(txtPlayer2.Text);

			data.Players = new List<Player>();
			if (!string.IsNullOrEmpty(txtPlayer1.Text))
				data.Players.Add(new Player(txtPlayer1.Text));
			if (!string.IsNullOrEmpty(txtPlayer2.Text))
				data.Players.Add(new Player(txtPlayer2.Text));
			if (!string.IsNullOrEmpty(txtPlayer3.Text))
				data.Players.Add(new Player(txtPlayer3.Text));
			if (!string.IsNullOrEmpty(txtPlayer4.Text))
				data.Players.Add(new Player(txtPlayer4.Text));


			data.StartValue = startValue;
			data.LegsBestOf = bestOfLegs;
			data.SetsBestOf = bestOfSets;

			return data;
		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
