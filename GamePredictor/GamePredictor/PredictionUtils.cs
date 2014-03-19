using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;

namespace GamePredictor
{
    public static class PredictionUtils
    {
        public static PredictionErrorStats GetPredictionErrorStats(IList<IGame> trainGames, IList<IGame> testGames, IGameLearner predictor)
        {
            predictor.Train(trainGames);

            var errorStats = new PredictionErrorStats();
            foreach (var testGame in testGames)
            {
                var predicted = PredictGame(predictor, testGame.Player1Id, testGame.Player2Id);
                var actual = testGame.Player1WinMeasure;
                errorStats.Observe(actual, predicted);
            }

            return errorStats;
        }

        public static double PredictGame(IGameLearner predictor, string player1Id, string player2Id)
        {
            double player1PredictedScore, player2PredictedScore;
            return PredictGame(predictor, player1Id, player2Id, out player1PredictedScore, out player2PredictedScore);
        }
        public static double PredictGame(IGameLearner predictor, string player1Id, string player2Id,
            out double player1PredictedScore, out double player2PredictedScore)
        {
            predictor.PredictGameResult(player1Id, player2Id, out player1PredictedScore, out player2PredictedScore);
            var predicted = GetPlayer1WinMeasure(player1PredictedScore, player2PredictedScore);

            predicted = Clip(predicted);

            return predicted;
        }

        public static double Clip(double value, double min = 0.001, double max = 0.999)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        public static double GetPlayer1WinMeasure(double player1PredictedScore, double player2PredictedScore)
        {
            return 1 / (1 + Math.Pow(Math.E, player2PredictedScore - player1PredictedScore)); //TODO: accomodate advantage
        }

        public static double SweepParameter(IList<IGame> games, double lowR, double highR,
            Func<double, IGameLearner> getPredictorForParameter, double minDelta = 0.01, double intervals = 10)
        {
            var increment = (highR - lowR) / intervals;
            if (increment < minDelta)
                return (lowR + highR) / 2;

            double firstBestRmse = double.PositiveInfinity, secondBestRmse = double.PositiveInfinity;
            double firstBestR = lowR, secondBestR = highR;
            for (var r = lowR; r <= highR; r += increment)
            {
                var rmse = GetCrossValidationErrorStats(games, getPredictorForParameter(r));
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

            return SweepParameter(games, Math.Min(firstBestR, secondBestR), Math.Max(firstBestR, secondBestR), 
                getPredictorForParameter, minDelta, intervals);
        }

        public static double GetCrossValidationErrorStats(IList<IGame> games, IGameLearner predictor, int foldsCount = 4)
        {
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

                totalRmse += PredictionUtils.GetPredictionErrorStats(trainSet, testSet, predictor).GetRmse();
            }

            return totalRmse / foldsCount;
        }
    }
}
