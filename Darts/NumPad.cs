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
	public partial class NumPad : Form
	{
		private int OldValue { get; set; }

		public NumPad()
		{
			InitializeComponent();
		}

		public int GetValue()
		{
			var newValue = OldValue;

			if (int.TryParse(txtValue.Text, out newValue))
				return newValue;

			return OldValue;
		}

		public void SetValue(int oldValue)
		{
			OldValue = oldValue;
			txtValue.Text = OldValue.ToString();
			txtValue.Select();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnP1C_Click(object sender, EventArgs e)
		{
			txtValue.Text = "";
		}

		private void btnP11_Click(object sender, EventArgs e)
		{
			txtValue.Text += "1";
		}

		private void btnP12_Click(object sender, EventArgs e)
		{
			txtValue.Text += "2";
		}

		private void btnP13_Click(object sender, EventArgs e)
		{

			txtValue.Text += "3";
		}

		private void btnP14_Click(object sender, EventArgs e)
		{
			txtValue.Text += "4";
		}

		private void btnP15_Click(object sender, EventArgs e)
		{
			txtValue.Text += "5";
		}

		private void btnP16_Click(object sender, EventArgs e)
		{
			txtValue.Text += "6";
		}

		private void btnP17_Click(object sender, EventArgs e)
		{
			txtValue.Text += "7";
		}

		private void btnP18_Click(object sender, EventArgs e)
		{
			txtValue.Text += "8";
		}

		private void btnP19_Click(object sender, EventArgs e)
		{
			txtValue.Text += "9";
		}

		private void btnP10_Click(object sender, EventArgs e)
		{
			txtValue.Text += "0";
		}
	}
}
