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

        public readonly double[] GameCount;
        public readonly double[] ScoreSum;
        public readonly double[] ScoreAverage;

        public PlayerProfile(IList<IGame> games)
        {
            this.playerIndices = new Dictionary<string, int>();
            for (var gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                var game = games[gameIndex];
                this.playerIndices.AddOrGetValue(game.Player1Id, () => playerIndices.Count);
                this.playerIndices.AddOrGetValue(game.Player2Id, () => playerIndices.Count);
            }

            this.GameCount = new double[this.playerIndices.Count];

            this.ScoreSum = new double[this.playerIndices.Count];
            this.ScoreAverage = new double[this.playerIndices.Count];

            for (var gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                var game = games[gameIndex];
                var player1Index = this.playerIndices[game.Player1Id];
                var player2Index = this.playerIndices[game.Player2Id];

                this.ScoreSum[player1Index] += game.Weight * game.Player1Score;
                this.ScoreSum[player2Index] += game.Weight * game.Player2Score;

                this.GameCount[player1Index] += game.Weight;
                this.GameCount[player2Index] += game.Weight;
            }

            for (var playerIndex = 0; playerIndex < this.playerIndices.Count; playerIndex++)
                this.ScoreAverage[playerIndex] = this.ScoreSum[playerIndex] / GameCount[playerIndex];
        }

        public int PlayerCount
        {
            get { return this.playerIndices.Count; }
        }

        public int this[string playerId]
        {
            get { return this.playerIndices[playerId]; }
        }
    }
}
