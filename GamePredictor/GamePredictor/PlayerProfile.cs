using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;

namespace GamePredictor
{
    public class PlayerProfile
    {
        readonly IDictionary<string, int> playerIndices;
        readonly double[] neighbourWeightedRatingsSum;
        public readonly double[] NeighbourWeightSum;
        public readonly double[] NeighbourWeightedAverage;

        public PlayerProfile(IList<IGame> games)
        {
            this.playerIndices = new Dictionary<string, int>();
            for (var gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                var game = games[gameIndex];
                this.playerIndices.AddOrGetValue(game.Player1Id, () => playerIndices.Count);
                this.playerIndices.AddOrGetValue(game.Player2Id, () => playerIndices.Count);
            }

            this.neighbourWeightedRatingsSum = new double[this.playerIndices.Count];
            this.NeighbourWeightSum = new double[this.playerIndices.Count];
            this.NeighbourWeightedAverage = new double[this.playerIndices.Count];
        }

        public int PlayerCount
        {
            get { return this.playerIndices.Count; }
        }

        public int this[string playerId]
        {
            get { return this.playerIndices[playerId]; }
        }

        public void CalculateNeighbourAverage(IList<IGame> games, double[] ratings)
        {
            Utils.InitializeArrayElement(neighbourWeightedRatingsSum, 0);
            Utils.InitializeArrayElement(NeighbourWeightSum, 0);

            for (var gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                var game = games[gameIndex];
                var player1Index = this.playerIndices[game.Player1Id];
                var player2Index = this.playerIndices[game.Player2Id];

                this.NeighbourWeightSum[player1Index] += game.Weight;
                this.NeighbourWeightSum[player2Index] += game.Weight;
                this.neighbourWeightedRatingsSum[player1Index] += game.Weight * ratings[player2Index];
                this.neighbourWeightedRatingsSum[player2Index] += game.Weight * ratings[player1Index];
            }

            for (var playerIndex = 0; playerIndex < this.playerIndices.Count; playerIndex++)
                this.NeighbourWeightedAverage[playerIndex] = neighbourWeightedRatingsSum[playerIndex] / NeighbourWeightSum[playerIndex];
        }
    }
}
