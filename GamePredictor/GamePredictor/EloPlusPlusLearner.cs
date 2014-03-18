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

        public const int DefaultMaxIterations = 100; //400 optimal for NCAA baseball, 50 for Chase
        public const int RansomSeed = 42;
        public const double DefaultRegularizationFactor = 0.2256;
        public const double DefaultPlayer1Advantage = 0;

        private PlayerProfile playerProfile;

        private readonly int maxIterations;
        private readonly double regularizationFactor;
        private readonly double player1Advantage;
        public EloPlusPlusLearner(double regularizationFactor = DefaultRegularizationFactor, int maxIterations = DefaultMaxIterations, double player1Advantage = DefaultPlayer1Advantage)
        {
            this.regularizationFactor = regularizationFactor;
            this.maxIterations = maxIterations;
            this.player1Advantage = player1Advantage;
        }

        public void Train(IList<IGame> games)
        {
            this.playerProfile = new PlayerProfile(games);
            this.ratings = new double[this.playerProfile.PlayerCount];

            for(var epoch = 1; epoch <= maxIterations; epoch++)
            {
                this.playerProfile.CalculateNeighbourAverage(games, this.ratings);

                var learningRate = Math.Pow((1 + 0.1 * maxIterations) / (epoch + 0.1 * maxIterations), 0.602);

                games.Shuffle(RansomSeed);
                for(var gameIndex = 0; gameIndex < games.Count; gameIndex++)
                {
                    var game = games[gameIndex];
                    var player1Index = this.playerProfile[game.Player1Id];
                    var player2Index = this.playerProfile[game.Player2Id];

                    var predicted = PredictGameResult(player1Index, player2Index, game.Player1HasAdvantage.IsTrue() ? this.player1Advantage : 0);
                    var actual = game.ResultForPlayer1;
                    var update = game.Weight * (predicted - actual) * predicted * (1 - predicted);
                    var regularization1 = regularizationFactor * (this.ratings[player1Index] - this.playerProfile.NeighbourWeightedAverage[player1Index]) / this.playerProfile.NeighbourWeightSum[player1Index];
                    var regularization2 = regularizationFactor * (this.ratings[player2Index] - this.playerProfile.NeighbourWeightedAverage[player2Index]) / this.playerProfile.NeighbourWeightSum[player2Index];

                    this.ratings[player1Index] -= learningRate * (update + regularization1);
                    this.ratings[player2Index] -= learningRate * (-update + regularization2);
                }
            }
        }

        public double GetRating(int playerIndex)
        {
            return ratings[playerIndex];
        }

        public double PredictGameResult(string player1Id, string player2Id, double player1Advantage = 0)
        {
            var player1Index = this.playerProfile[player1Id];
            var player2Index = this.playerProfile[player2Id];
            return PredictGameResult(player1Index, player2Index, player1Advantage);
        }
        private double PredictGameResult(int player1Index, int player2Index, double player1Advantage = 0)
        {
            return 1 / (1 + Math.Pow(Math.E, this.ratings[player2Index] - this.ratings[player1Index] - player1Advantage));
        }
    }
}
