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
            Dictionary<string, double> rmseBySeasons = new Dictionary<string, double>();
            foreach(var seasonGamesKvp in tourneyGamesBySeasons.GamesBySeasons)
            {
                var predictor = new EloPlusPlusPredictor(); //new TeamSeedPredictor(this.teamSeedsBySeason[seasonGamesKvp.Key]);
                var trainGames = regularGamesBySeasons.GamesBySeasons[seasonGamesKvp.Key];
                var testGames = seasonGamesKvp.Value;
                var rmse = GetRmse(trainGames, testGames, predictor);
                rmseBySeasons[seasonGamesKvp.Key] = rmse;
            }

            var avgRmse = rmseBySeasons.Average(kvp => kvp.Value);

            Clipboard.SetData(DataFormats.Text, avgRmse.ToStringInvariant());
            MessageBox.Show("Avg RMSE = {0}".FormatEx(avgRmse));
        }

        private double GetRmse(IList<IGame> trainGames, IList<IGame> testGames, IRatingPredictor predictor)
        {
            predictor.Train(trainGames);

            double totalErrorSquare = 0;
            foreach (var testGame in testGames)
            {
                var predicted = predictor.PredictGameResult(testGame.Player1Id, testGame.Player2Id);
                var actual = testGame.ResultForPlayer1;
                totalErrorSquare += (actual - predicted) * (actual - predicted);
            }

            return Math.Sqrt(totalErrorSquare / testGames.Count);
        }


        private double GetOptimalRegularizationFactor(IList<IGame> games, double lowR = 0.01, double highR = 0.99)
        {
            var increment = (highR - lowR) / 5;
            if (increment < 0.01)
                return (lowR + highR) / 2;

            double firstBestRmse = double.PositiveInfinity, secondBestRmse = double.PositiveInfinity;
            double firstBestR = lowR, secondBestR = highR;
            for(var r = lowR; r <= highR; r+=increment)
            {
                var rmse = GetRmseForRegularization(games, r);
                if (rmse < firstBestRmse)
                {
                    secondBestRmse = firstBestRmse;
                    secondBestR = firstBestR;

                    firstBestRmse = rmse;
                    firstBestR = r;
                }
                else if (rmse < secondBestRmse)
                {
                    secondBestRmse = rmse;
                    secondBestR = r;
                }
            }

            return this.GetOptimalRegularizationFactor(games, Math.Min(firstBestR, secondBestR), Math.Max(firstBestR, secondBestR));
        }

        private double GetRmseForRegularization(IList<IGame> games, double regularizationFactor)
        {
            var predictor = new EloPlusPlusPredictor(regularizationFactor);
            var foldsCount = 3;
            var foldSize = games.Count / foldsCount;

            var totalRmse = 0d;
            for (var fold = 0; fold < foldsCount; fold++)
            {
                var testSetStart = fold * foldSize;
                var testSetEnd = testSetStart + foldSize;

                var trainSet = games.Where((g, i) => i < testSetStart || i >= testSetEnd).ToArray();
                var knownPlayers = new HashSet<string>();
                knownPlayers.AddRange(trainSet.Select(g => g.Player1Id));
                knownPlayers.AddRange(trainSet.Select(g => g.Player2Id));

                var testSet = games.Where((g, i) => i >= testSetStart && i < testSetEnd &&
                    knownPlayers.Contains(g.Player1Id) && knownPlayers.Contains(g.Player2Id)).ToArray();

                totalRmse += GetRmse(trainSet, testSet, predictor);
            }

            return totalRmse / foldsCount;
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
            var optimalRs = regularGamesBySeasons.GamesBySeasons.Values.Select(gs => this.GetOptimalRegularizationFactor(gs)).ToArray();
            MessageBox.Show(optimalRs.Average().ToString());
        }
    }
}
