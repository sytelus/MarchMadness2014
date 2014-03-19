using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;

namespace GamePredictor
{
    public class EloPlusPlusLearner : IGameLearner
    {
        double[] ratings;

        public const int DefaultMaxIterations = 600; //400 optimal for NCAA baseball, 50 for Chase
        public const int RansomSeed = 42;
        public const double DefaultRegularizationFactor = 0.2256;

        private PlayerProfile players;

        private readonly int maxIterations;
        private readonly double regularizationFactor;
        public EloPlusPlusLearner(double regularizationFactor = DefaultRegularizationFactor, int maxIterations = DefaultMaxIterations)
        {
            this.regularizationFactor = regularizationFactor;
            this.maxIterations = maxIterations;
        }

        public void Train(IList<IGame> games)
        {
            this.players = new PlayerProfile(games);
            this.ratings = new double[this.players.PlayerCount];

            for(var epoch = 1; epoch <= maxIterations; epoch++)
            {
                var neighbourRatingsAverage = GetNeighbourRatingsAverage(games, this.players, this.ratings);

                var learningRate = Math.Pow((1 + 0.1 * maxIterations) / (epoch + 0.1 * maxIterations), 0.602);

                games.Shuffle(RansomSeed);
                foreach (var game in games)
                {
                    var player1Index = this.players[game.Player1Id];
                    var player2Index = this.players[game.Player2Id];

                    double player1PredictedScore, player2PredictedScore;
                    this.PredictGameResult(player1Index, player2Index, out player1PredictedScore, out player2PredictedScore);
                    var predicted = PredictionUtils.GetPlayer1WinMeasure(player1PredictedScore, player2PredictedScore);

                    var actual = game.Player1WinMeasure;
                    var update = game.Weight * (predicted - actual) * predicted * (1 - predicted);
                    var regularization1 = regularizationFactor * (this.ratings[player1Index] - neighbourRatingsAverage[player1Index]) / this.players.GameCount[player1Index];
                    var regularization2 = regularizationFactor * (this.ratings[player2Index] - neighbourRatingsAverage[player2Index]) / this.players.GameCount[player2Index];

                    this.ratings[player1Index] -= learningRate * (update + regularization1);
                    this.ratings[player2Index] -= learningRate * (-update + regularization2);
                }
            }
        }

        private static double[] GetNeighbourRatingsAverage(IList<IGame> games, PlayerProfile players, double[] ratings)
        {
            var ratingsSum = new double[players.PlayerCount];
            var ratingsAverage = new double[players.PlayerCount];

            for (var gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                var game = games[gameIndex];
                var player1Index = players[game.Player1Id];
                var player2Index = players[game.Player2Id];

                ratingsSum[player1Index] += game.Weight * ratings[player2Index];
                ratingsSum[player2Index] += game.Weight * ratings[player1Index];
            }

            for (var playerIndex = 0; playerIndex < players.PlayerCount; playerIndex++)
                ratingsAverage[playerIndex] = ratingsSum[playerIndex] / players.GameCount[playerIndex];

            return ratingsAverage;
        }

        public double GetRating(int playerIndex)
        {
            return ratings[playerIndex];
        }

        public void PredictGameResult(string player1Id, string player2Id, out double player1ScorePrediction, out double player2ScorePrediction)
        {
            var player1Index = this.players[player1Id];
            var player2Index = this.players[player2Id];

            PredictGameResult(player1Index, player2Index, out player1ScorePrediction, out player2ScorePrediction);
        }

        private void PredictGameResult(int player1Index, int player2Index, out double player1ScorePrediction, out double player2ScorePrediction)
        {
            player1ScorePrediction = this.ratings[player1Index];
            player2ScorePrediction = this.ratings[player2Index];
        }
    }
}
