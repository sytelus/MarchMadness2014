using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;

namespace GamePredictor
{
    public class EloPlusPlusPredictor : IRatingPredictor
    {
        double[] ratings;
        IDictionary<string, int> playerIndices;

        public const int DefaultMaxIterations = 50;
        public const int RansomSeed = 421;
        public const double DefaultRegularizationFactor = 0.22766315789473687;

        private int maxIterations;
        private double regularizationFactor;
        public EloPlusPlusPredictor(double regularizationFactor = DefaultRegularizationFactor, int maxIterations = DefaultMaxIterations)
        {
            this.regularizationFactor = regularizationFactor;
            this.maxIterations = maxIterations;
        }

        public void Train(IList<IGame> games)
        {
            this.InitializePlayerIndices(games);
            this.ratings = new double[this.playerIndices.Count];
            var neighbourWeightedRatingsSum = new double[this.playerIndices.Count];
            var neighbourWeightSum = new double[this.playerIndices.Count];
            var neighbourWeightedAverage = new double[this.playerIndices.Count];

            for(var epoch = 1; epoch <= maxIterations; epoch++)
            {
                CalculateNeighbourAverage(games, neighbourWeightedRatingsSum, neighbourWeightSum, neighbourWeightedAverage);

                var learningRate = Math.Pow((1 + 0.1 * maxIterations) / (epoch + 0.1 * maxIterations), 0.602);

                Utils.Shuffle(games, RansomSeed);
                for(var gameIndex = 0; gameIndex < games.Count; gameIndex++)
                {
                    var game = games[gameIndex];
                    var player1Index = this.playerIndices[game.Player1Id];
                    var player2Index = this.playerIndices[game.Player2Id];

                    var predicted = PredictGameResult(player1Index, player2Index, game.Player1Advantage);
                    var actual = game.ResultForPlayer1;
                    var update = game.Weight * (predicted - actual) * predicted * (1 - predicted);
                    var regularization1 = regularizationFactor * (this.ratings[player1Index] - neighbourWeightedAverage[player1Index]) / neighbourWeightSum[player1Index];
                    var regularization2 = regularizationFactor * (this.ratings[player2Index] - neighbourWeightedAverage[player2Index]) / neighbourWeightSum[player2Index];

                    this.ratings[player1Index] -= learningRate * (update + regularization1);
                    this.ratings[player2Index] -= learningRate * (-update + regularization2);
                }
            }
        }

        private void InitializePlayerIndices(IList<IGame> games)
        {
            this.playerIndices = new Dictionary<string, int>();
            for (var gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                var game = games[gameIndex];
                this.playerIndices.AddOrGetValue(game.Player1Id, () => playerIndices.Count);
                this.playerIndices.AddOrGetValue(game.Player2Id, () => playerIndices.Count);
            }
        }

        private void CalculateNeighbourAverage(IList<IGame> games, double[] neighbourWeightedRatingsSum, double[] neighbourWeightSum, double[] neighbourWeightedAverage)
        {
            SetArrayValues(neighbourWeightedRatingsSum, 0);
            SetArrayValues(neighbourWeightSum, 0);

            for(var gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                var game = games[gameIndex];
                var player1Index = this.playerIndices[game.Player1Id];
                var player2Index = this.playerIndices[game.Player2Id];

                neighbourWeightSum[player1Index] += game.Weight;
                neighbourWeightSum[player2Index] += game.Weight;
                neighbourWeightedRatingsSum[player1Index] += game.Weight * this.ratings[player2Index];
                neighbourWeightedRatingsSum[player2Index] += game.Weight * this.ratings[player1Index];
            }

            for (var playerIndex = 0; playerIndex < this.playerIndices.Count; playerIndex++)
                neighbourWeightedAverage[playerIndex] = neighbourWeightedRatingsSum[playerIndex] / neighbourWeightSum[playerIndex];
        }

        //TODO: add to Utils.cs
        public static void SetArrayValues<T>(IList<T> list, T value)
        {
            for (var i = 0; i < list.Count; i++)
                list[i] = value;
        }

        public double GetRating(int playerIndex)
        {
            return ratings[playerIndex];
        }

        public double PredictGameResult(string player1Id, string player2Id, double player1Advantage = 0)
        {
            var player1Index = this.playerIndices[player1Id];
            var player2Index = this.playerIndices[player2Id];
            return PredictGameResult(player1Index, player2Index, player1Advantage);
        }
        private double PredictGameResult(int player1Index, int player2Index, double player1Advantage = 0)
        {
            return 1 / (1 + Math.Pow(Math.E, this.ratings[player2Index] - this.ratings[player1Index] - player1Advantage));
        }
    }
}
