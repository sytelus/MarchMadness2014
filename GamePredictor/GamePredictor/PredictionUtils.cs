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
        public static PredictionErrorStats GetPredictionErrorStats(IList<IGame> trainGames, IList<IGame> testGames, IRatingPredictor predictor)
        {
            predictor.Train(trainGames);

            var errorStats = new PredictionErrorStats();
            foreach (var testGame in testGames)
            {
                var predicted = predictor.PredictGameResult(testGame.Player1Id, testGame.Player2Id);
                var actual = testGame.ResultForPlayer1;
                errorStats.Observe(actual, predicted);
            }

            return errorStats;
        }

        public static double SweepParameter(IList<IGame> games, double lowR, double highR,
            Func<double, IRatingPredictor> getPredictorForParameter, double minDelta = 0.01, double intervals = 10)
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

        public static double GetCrossValidationErrorStats(IList<IGame> games, IRatingPredictor predictor, int foldsCount = 4)
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
