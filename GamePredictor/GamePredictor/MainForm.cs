﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonUtils;
using System.IO;

namespace GamePredictor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonMain_Click(object sender, EventArgs e)
        {
            var rmseBySeasons = new Dictionary<string, PredictionErrorStats>();
            foreach(var seasonGamesKvp in tourneyGamesBySeasons.GamesBySeasons)
            {
                var predictor = new EloPlusPlusLearner(); // new TeamSeedPredictor(this.teamSeedsBySeason[seasonGamesKvp.Key]);
                var trainGames = regularGamesBySeasons.GamesBySeasons[seasonGamesKvp.Key];
                var testGames = seasonGamesKvp.Value;
                var errorStats = PredictionUtils.GetPredictionErrorStats(trainGames, testGames, predictor);
                rmseBySeasons[seasonGamesKvp.Key] = errorStats;
            }

            var avgRmse = rmseBySeasons.Average(kvp => kvp.Value.GetRmse());
            var avgLogLoss = rmseBySeasons.Average(kvp => kvp.Value.GetLogLoss());

            Clipboard.SetData(DataFormats.Text, string.Concat(avgLogLoss, "\t", avgRmse));
            MessageBox.Show("Avg RMSE = {0}, Avg LogLoss = {1}".FormatEx(avgRmse, avgLogLoss));
        }

        private IDictionary<string, IDictionary<string, int>> TeamSeedsParser(string filePath)
        {
            return File.ReadAllLines(filePath).Select(line => line.Split(Utils.CommaDelimiter)).Skip(1)
                .GroupBy(columns => columns[0])
                .Select(g => new KeyValuePair<string, IDictionary<string, int>>(g.Key, 
                    g.Select(c => new KeyValuePair<string, int>(c[2], ParseSeedColumn(c[1]))).ToDictionary()))
                .ToDictionary();
        }

        private static int ParseSeedColumn(string value)
        {
            value = new string(value.Where(c => char.IsNumber(c)).ToArray());
            return int.Parse(value);
        }

        GameResultParser regularGamesBySeasons;
        GameResultParser tourneyGamesBySeasons;
        IDictionary<string, IDictionary<string, int>> teamSeedsBySeason;
        private void MainForm_Load(object sender, EventArgs e)
        {
            regularGamesBySeasons = new GameResultParser(@"..\..\..\..\data\kaggle\regular_season_results.csv");
            tourneyGamesBySeasons = new GameResultParser(@"..\..\..\..\data\kaggle\tourney_results.csv");
            teamSeedsBySeason = TeamSeedsParser(@"..\..\..\..\data\kaggle\tourney_seeds.csv");
        }

        private void buttonCalibrateRegularization_Click(object sender, EventArgs e)
        {
            var optimalRs = regularGamesBySeasons.GamesBySeasons.Values
                .Select(gs => PredictionUtils.SweepParameter(gs, 0.01, 0.99, (r) => new EloPlusPlusLearner(r))).ToArray();
            MessageBox.Show(optimalRs.Average().ToString());
        }

        private double ScoreBoostForHomeTeam(IList<IGame> games)
        {
            var players = new PlayerProfile(games);
            double homeTeamScoreSums = 0d, notHomeTeamScoreSums = 0d;
            int homeTeamScoreCount = 0, notHomeTeamScoreCount = 0;

            foreach (var game in games)
            {
                if (game.Player1HasAdvantage.IsTrue())
                {
                    homeTeamScoreCount++;
                    homeTeamScoreSums += game.Player1Score;

                    notHomeTeamScoreCount++;
                    notHomeTeamScoreSums += game.Player2Score;
                }
                else if (game.Player1HasAdvantage.IsFalse())
                {
                    homeTeamScoreCount++;
                    homeTeamScoreSums += game.Player2Score;

                    notHomeTeamScoreCount++;
                    notHomeTeamScoreSums += game.Player1Score;
                }
                else
                {
                    notHomeTeamScoreCount++;
                    notHomeTeamScoreSums += game.Player1Score;

                    notHomeTeamScoreCount++;
                    notHomeTeamScoreSums += game.Player2Score;
                }
            }

            var averageHomeTeamScore = homeTeamScoreSums/homeTeamScoreCount;
            var averageNotHomeTeamScore = notHomeTeamScoreSums / notHomeTeamScoreCount;

            return (averageHomeTeamScore - averageNotHomeTeamScore)/averageNotHomeTeamScore;
        }

        private void buttonHomeAdvantage_Click(object sender, EventArgs e)
        {
            var boost = this.regularGamesBySeasons.GamesBySeasons.Values.Select(games => games.Select(g => (BasketballGame)g).ToArray())
                .Select(games => ScoreBoostForHomeTeam(games))
                .Average();
            MessageBox.Show(boost.ToStringInvariant());
        }
    }
}
