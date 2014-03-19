using System;
using System.Collections.Generic;
using CommonUtils;

namespace GamePredictor.IncrementalSvd
{
    partial class SFLearner : IGameLearner
    {
        #region Parameters
        public const int SvCount = 1;
        public const double Regularization = 0.015f; //default 0.015f
        public const double LearningRate = 0.001f; //default 0.001f
        public const double SvInitial = 0.1f; //default 0.1f
        public const int MaxEpochs = 128;
        #endregion

        public double[,] S1Vectors;
        public double[,] S2Vectors;
        private PlayerProfile players;
        private double[] tempPlayer1Predictions;
        private double[] tempPlayer2Predictions;

        void Initialize(IList<IGame> games)
        {
            this.players = new PlayerProfile(games);

            this.S1Vectors = new double[players.PlayerCount, SvCount];
            this.S2Vectors = new double[players.PlayerCount, SvCount];

            Utils.InitializeArrayElement(this.S1Vectors, SvInitial);
            Utils.InitializeArrayElement(this.S2Vectors, SvInitial);

            InitializeTempPredictions(this.players, games, out this.tempPlayer1Predictions, out this.tempPlayer2Predictions);
        }

        private static void InitializeTempPredictions(PlayerProfile players, IList<IGame> games,
            out double[] tempPlayer1Predictions, out double[] tempPlayer2Predictions)
        {
            tempPlayer1Predictions = new double[players.PlayerCount];
            tempPlayer2Predictions = new double[players.PlayerCount];

            for (var gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                var player1Index = players[games[gameIndex].Player1Id];
                var player2Index = players[games[gameIndex].Player2Id];

                //TODO: what about bias
                tempPlayer1Predictions[gameIndex] = players.ScoreAverage[player1Index];
                tempPlayer2Predictions[gameIndex] = players.ScoreAverage[player2Index];    
            }
        }

        public void Train(IList<IGame> games)
        {
            this.Initialize(games);

            //for each feature
            for (var svIndex = 0; svIndex < SvCount; svIndex++)
            {
                for (var ephochIndex = 0; ephochIndex < MaxEpochs; ephochIndex++)   // && (rmse - prevRmse > _improvementExitThreshold)
                {
                    RunEpoch(svIndex, games, true);
                    RunEpoch(svIndex, games, false);
                }

                this.UpdateTempPredictions(svIndex, games);
            }//All SVs done
        }

        private void RunEpoch(int svIndex, IEnumerable<IGame> games, bool forPlayer1)
        {
            foreach (var game in games)
            {
                var x1 = this.players[game.Player1Id];
                var x2 = this.players[game.Player2Id];

                if (forPlayer1)
                    UpdateSvdVectors(svIndex, x1, x2, this.tempPlayer1Predictions[x1], game.Player1Score);
                else
                    UpdateSvdVectors(svIndex, x2, x1, this.tempPlayer1Predictions[x2], game.Player2Score);
            }
        }

        private void UpdateSvdVectors(int svIndex, int x1, int x2, double tempPrediction, double actualValue)
        {
            var s1Value = this.S1Vectors[x1, svIndex];
            var s2Value = this.S2Vectors[x2, svIndex];

            tempPrediction += (s1Value * s2Value);

            var err = actualValue - tempPrediction;

            //Save old feature values before changing them
            var x1Increment = LearningRate*((err*s2Value) - (Regularization*s1Value));
            var x2Increment = LearningRate*((err*s1Value) - (Regularization*s2Value));

            // Cross-train the features using saved values
            this.S1Vectors[x1, svIndex] += x1Increment;
            this.S2Vectors[x2, svIndex] += x2Increment;
        }

        private void UpdateTempPredictions(int svIndex, IList<IGame> games)
        {
            for (var gameIndex = 0; gameIndex < games.Count; gameIndex++)
            {
                var game = games[gameIndex];
                var x1 = this.players[game.Player1Id];
                var x2 = this.players[game.Player2Id];

                //TODO: limit predicted value?
                this.tempPlayer1Predictions[gameIndex] += this.S1Vectors[x1, svIndex] * this.S2Vectors[x2, svIndex];
                this.tempPlayer2Predictions[gameIndex] += this.S1Vectors[x2, svIndex] * this.S2Vectors[x1, svIndex];
            }
        }

        public void PredictGameResult(string player1Id, string player2Id, out double player1ScorePrediction, out double player2ScorePrediction)
        {
            var x1 = this.players[player1Id];
            var x2 = this.players[player2Id];

            player1ScorePrediction = this.players.ScoreAverage[x1];
            player2ScorePrediction = this.players.ScoreAverage[x2];
            for (var svIndex = 0; svIndex <= SvCount; svIndex++)
            {
                //TODO: limit values
                player1ScorePrediction += (this.S1Vectors[x1, svIndex] * this.S2Vectors[x2, svIndex]);
                player2ScorePrediction += (this.S1Vectors[x2, svIndex] * this.S2Vectors[x1, svIndex]);  
            }
        }
    }
}
