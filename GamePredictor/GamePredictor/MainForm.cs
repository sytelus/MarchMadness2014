using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonUtils;

namespace GamePredictor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var regularGames = new GameResultParser(@"..\..\..\..\data\regular_season_results.csv");
            var tourneyGames = new GameResultParser(@"..\..\..\..\data\tourney_results.csv");

            MessageBox.Show("{0}, {1}".FormatEx(regularGames.GamesBySeasons.Count, tourneyGames.GamesBySeasons.Count));
        }
    }
}
