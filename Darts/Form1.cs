using static System.Net.Mime.MediaTypeNames;

// https://youtu.be/Ly-9VTXJlnA

namespace Darts
{
	public partial class Form1 : Form
	{
		public Game TheGame { get; set; }

		public Form1()
		{
			InitializeComponent();
		}

		#region buttons

		private void button2_Click(object sender, EventArgs e)
		{
			txtP1.Text += "7";
		}

		private void btnP1C_Click(object sender, EventArgs e)
		{
			txtP1.Text = string.Empty;
		}

		private void btnP11_Click(object sender, EventArgs e)
		{
			txtP1.Text += "1";
		}

		private void btnP12_Click(object sender, EventArgs e)
		{
			txtP1.Text += "2";
		}

		private void btnP13_Click(object sender, EventArgs e)
		{
			txtP1.Text += "3";
		}

		private void btnP10_Click(object sender, EventArgs e)
		{
			txtP1.Text += "0";
		}

		private void btnP14_Click(object sender, EventArgs e)
		{
			txtP1.Text += "4";
		}

		private void btnP15_Click(object sender, EventArgs e)
		{
			txtP1.Text += "5";
		}

		private void btnP16_Click(object sender, EventArgs e)
		{
			txtP1.Text += "6";
		}

		private void btnP18_Click(object sender, EventArgs e)
		{
			txtP1.Text += "8";
		}

		private void btnP19_Click(object sender, EventArgs e)
		{
			txtP1.Text += "9";
		}

		private void btnP11OK_Click(object sender, EventArgs e)
		{
			ApplyThrow();
		}

		#endregion

		private void ApplyThrow()
		{
			if (int.TryParse(txtP1.Text, out int value))
			{
				if (value <= 180 && value >= 0)
				{
					TheGame.DecreaseValue(value);
				}
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			KeypadP1.Enabled = false;
			btnReEnterValue.Enabled = false;
			btnSaveGame.Enabled = false;
		}

		private void TheGame_CurrentThrowCompleted(int whatPlayer, int thrownValue, int newRest, int whoStarted)
		{

			UpdateScoreBoard();
			btnReEnterValue.Enabled = true;

		}

		private void UpdateScoreBoard()
		{

			picActivePlayer1.Visible = false;
			picActivePlayer2.Visible = false;
			picActivePlayer3.Visible = false;
			picActivePlayer4.Visible = false;

			for (int i = 0; i < TheGame.Players.Count ; i++)
			{
				switch (i)
				{
					case 0:
						lblPlayerName1.Text = TheGame.Players[i].Name;
						txtCurrentBalanceP1.Text = TheGame.GetBalance(1).ToString();
						txtDartOutP1.Text = TheGame.GetDartOut(TheGame.GetBalance(1));
						txtHighScoreP1.Text = TheGame.GetHighestScore(1).ToString();
						txtAvgP1.Text = TheGame.GetAvg(1).ToString();
						if(TheGame.WhatPlayer==1) picActivePlayer1.Visible = true;
						break;
					case 1:
						lblPlayerName2.Text = TheGame.Players[i].Name;
						txtCurrentBalanceP2.Text = TheGame.GetBalance(2).ToString();
						txtDartOutP2.Text = TheGame.GetDartOut(TheGame.GetBalance(2));
						txtHighScoreP2.Text = TheGame.GetHighestScore(2).ToString();
						txtAvgP2.Text = TheGame.GetAvg(2).ToString();
						if (TheGame.WhatPlayer == 2) picActivePlayer2.Visible = true;
						break;
					case 2:
						lblPlayerName3.Text = TheGame.Players[i].Name; 
						txtCurrentBalanceP3.Text = TheGame.GetBalance(3).ToString();
						txtDartOutP3.Text = TheGame.GetDartOut(TheGame.GetBalance(3));
						txtHighScoreP3.Text = TheGame.GetHighestScore(3).ToString();
						txtAvgP3.Text = TheGame.GetAvg(3).ToString();
						if (TheGame.WhatPlayer == 3) picActivePlayer3.Visible = true;
						break;
					case 3:
						lblPlayerName4.Text = TheGame.Players[i].Name; 
						txtCurrentBalanceP4.Text = TheGame.GetBalance(4).ToString();
						txtDartOutP4.Text = TheGame.GetDartOut(TheGame.GetBalance(4));
						txtHighScoreP4.Text = TheGame.GetHighestScore(4).ToString();
						txtAvgP4.Text = TheGame.GetAvg(4).ToString();
						if (TheGame.WhatPlayer == 4) picActivePlayer4.Visible = true;
						break;
				}
			}

			txtP1.Text = "";
			txtP1.Focus();

		}

		private void btnNewGame_Click(object sender, EventArgs e)
		{
			btnReEnterValue.Enabled = false;

			var newGame = new NewGame();
			var result = newGame.ShowDialog();

			if(result == DialogResult.OK)
			{
				var model = newGame.GetData();
				TheGame = new Game(model, "c:\\temp\\darts.db");
				TheGame.GameCompleted += TheGame_GameCompleted;
				TheGame.SetCompleted += TheGame_SetCompleted;
				TheGame.LegCompleted += TheGame_LegCompleted;
				TheGame.CurrentThrowCompleted += TheGame_CurrentThrowCompleted;
				ResetMainForm();
			}
		}

		private void TheGame_LegCompleted(int whatPlayer, int whoStarts)
		{
			if (TheGame != null)
				_ = TheGame.SaveGame().Result;
		
			ResetMainForm();
		}

		private void ResetMainForm()
		{
			KeypadP1.Enabled = true;

			lblPlayerName1.Text = "";
			txtDartOutP1.Text = "";
			txtHighScoreP1.Text = "";
			txtAvgP1.Text = "";
			txtDartOutP1.Enabled=false;
			txtHighScoreP1.Enabled = false;
			txtAvgP1.Enabled = false;
			lblLegsPlayer1.Text ="";
			lblSetsPlayer1.Text = "";

			lblPlayerName2.Text = "";
			txtDartOutP2.Text = "";
			txtHighScoreP2.Text = "";
			txtAvgP2.Enabled = false;
			txtDartOutP2.Enabled = false;
			txtHighScoreP2.Enabled = false;
			txtAvgP2.Text = "";
			lblLegsPlayer2.Text = "";
			lblSetsPlayer2.Text = "";

			lblPlayerName3.Text = "";
			txtDartOutP3.Text = "";
			txtHighScoreP3.Text = "";
			txtAvgP3.Text = "";
			txtDartOutP3.Enabled = false;
			txtHighScoreP3.Enabled = false;
			txtAvgP3.Enabled = false;
			lblLegsPlayer3.Text = "";
			lblSetsPlayer3.Text = "";

			lblPlayerName4.Text = "";
			txtDartOutP4.Text = "";
			txtHighScoreP4.Text = "";
			txtAvgP4.Text = "";
			txtDartOutP4.Enabled = false;
			txtHighScoreP4.Enabled = false;
			txtAvgP4.Enabled = false;
			lblLegsPlayer4.Text = "";
			lblSetsPlayer4.Text = "";

			picActivePlayer1.Visible = false;
			picActivePlayer2.Visible = false;
			picActivePlayer3.Visible = false;
			picActivePlayer4.Visible = false;

			txtCurrentBalanceP1.Text = "0";
			txtCurrentBalanceP2.Text = "0";
			txtCurrentBalanceP3.Text = "0";
			txtCurrentBalanceP4.Text = "0";


			for (int i = 0; i < TheGame.Players.Count; i++)
			{
				switch (i)
				{
					case 0:
						lblPlayerName1.Text = TheGame.Players[i].Name;
						txtCurrentBalanceP1.Text = TheGame.StartValue.ToString();
						txtLegsP1.Text = TheGame.Sets.Last().GetLegs(1);
						txtSetsP1.Text = TheGame.GetSets(1);
						lblLegsPlayer1.Text = $"Legs ({TheGame.Players[i].Name}):";
						lblSetsPlayer1.Text = $"Sets ({TheGame.Players[i].Name}):";
						if(TheGame.WhatPlayer==1) picActivePlayer1.Visible = true;
						break;
					case 1:
						lblPlayerName2.Text = TheGame.Players[i].Name;
						txtCurrentBalanceP2.Text = TheGame.StartValue.ToString();
						txtLegsP2.Text = TheGame.Sets.Last().GetLegs(2);
						txtSetsP2.Text = TheGame.GetSets(2);
						lblLegsPlayer2.Text = $"Legs ({TheGame.Players[i].Name}):";
						lblSetsPlayer2.Text = $"Sets ({TheGame.Players[i].Name}):";
						if (TheGame.WhatPlayer == 2) picActivePlayer2.Visible = true;
						break;
					case 2:
						lblPlayerName3.Text = TheGame.Players[i].Name;
						txtCurrentBalanceP3.Text = TheGame.StartValue.ToString();
						txtLegsP3.Text = TheGame.Sets.Last().GetLegs(3);
						txtSetsP3.Text = TheGame.GetSets(3);
						lblLegsPlayer3.Text = $"Legs ({TheGame.Players[i].Name}):";
						lblSetsPlayer3.Text = $"Sets ({TheGame.Players[i].Name}):";
						if (TheGame.WhatPlayer == 3) picActivePlayer3.Visible = true;
						break;
					case 3:
						lblPlayerName4.Text = TheGame.Players[i].Name;
						txtCurrentBalanceP4.Text = TheGame.StartValue.ToString();
						txtLegsP4.Text = TheGame.Sets.Last().GetLegs(4);
						txtSetsP4.Text = TheGame.GetSets(4);
						lblLegsPlayer4.Text = $"Legs ({TheGame.Players[i].Name}):";
						lblSetsPlayer4.Text = $"Sets ({TheGame.Players[i].Name}):";
						if (TheGame.WhatPlayer == 4) picActivePlayer4.Visible = true;

						break;

				}
			}


			btnReEnterValue.Enabled = true;

			txtP1.Text = "";
			
	
		}

		private void TheGame_SetCompleted(int whatPlayer, int whoStarts)
		{
			ResetMainForm();
		}

		private void TheGame_GameCompleted(int whatPlayer)
		{

			Player p = TheGame.Players[whatPlayer-1];

			MessageBox.Show($"{p.Name} hat gewonnen!", "Herzlichen Glückwunsch!", MessageBoxButtons.OK);
		}

		private void btnReEnterValue_Click(object sender, EventArgs e)
		{
			var currentValueTuple = TheGame.GetLastThrownValue();
			if (currentValueTuple != null)
			{
				var frm = new NumPad();
				frm.SetValue(currentValueTuple.Item2);
				var result = frm.ShowDialog();
				if (result == DialogResult.OK)
				{
					var newValue = frm.GetValue();
					Tuple<int, int>? newValueTuple = TheGame.Sets.Last().Legs.Last().UpdateValue(newValue, currentValueTuple.Item1);

					if(newValueTuple!=null)
						UpdateScoreBoard();
				}
			}
		}

		private void SaveGame_Click(object sender, EventArgs e)
		{
			if(TheGame!=null)
			{
				Task.Factory.StartNew(() => { var ret = TheGame.SaveGame(); });
			}
		}

	}
}