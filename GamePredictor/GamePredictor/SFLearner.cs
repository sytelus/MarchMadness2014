using System;
using System.Collections.Generic;
using CommonUtils;

namespace GamePredictor.IncrementalSvd
{
    partial class SFLearner : IGameLearner
    {
        #region Parameters
        public readonly static int SVCount = 1;
        public readonly static int SVCountForL2 = 64;    //5 seems to be the best
        public static readonly float RegularizationConstant = 0.015f; //default 0.015f
        public static float LearningRate = 0.001f; //default 0.001f
        public static float LearningRateForL2 = 0.001f; //default 0.001f -- doesn't seem to affect if you lower by 1/20th
        public static float SVInitial = 0.1f; //default 0.1f
        public static int MaxEpochs = 128;
        #endregion

        public double[,] S1Vectors;
        public double[,] S2Vectors;
        private PlayerProfile playerProfile;
        private double[] tempPredictions;

        void InitializeS1S2(PlayerProfile players)
        {
            this.S1Vectors = new double[players.PlayerCount, SVCount];
            this.S2Vectors = new double[players.PlayerCount, SVCount];

            Utils.InitializeArrayElement(this.S1Vectors, SVInitial);
            Utils.InitializeArrayElement(this.S2Vectors, SVInitial);
        }

        //void InitializeTempPredictions(LearningData learningData)
        //{
        //    if ((this.tempPredictions == null) || (this.tempPredictions.Length != learningData.Data.DataCount))
        //        this.tempPredictions = new float[learningData.Data.DataCount];

        //    for (var dataIndex = 0; dataIndex < learningData.Data.DataCount; dataIndex++)
        //    {
        //        this.tempPredictions[dataIndex] = learningData.ItemProfiles.X2Avg[learningData.Data.X2[dataIndex]]
        //             + learningData.ItemProfiles.X1AvgBias[learningData.Data.X1[dataIndex]];
        //    }
        //}

        public void Train(IList<IGame> games)
        {
            this.playerProfile = new PlayerProfile(games);
            InitializeS1S2(this.playerProfile);
            //InitializeTempPredictions(learningData);

            //for each feature
            for (var svIndex=0; svIndex < SVCount; svIndex++)
            {
                for (var ephochIndex = 0; ephochIndex < MaxEpochs; ephochIndex++) // && (rmse - prevRmse > _improvementExitThreshold)
                    RunEpoch(svIndex, ephochIndex, games); 

                //UpdateTempPredictions(svIndex, learningData);
            }//All SVs done
        }

        private double RunEpoch(int svIndex, int epochIndex, IList<IGame> games)
        {
            double predictionErrorSquareSum = 0, learningRate = LearningRate
                , regularizationConstant = RegularizationConstant;

            #region Perf Loop
            for (var dataIndex = 0; dataIndex < games.Count; dataIndex++)
            {
                var x1 = this.playerProfile[games[dataIndex].Player1Id];
                var x2 = this.playerProfile[games[dataIndex].Player2Id];

                var s1Value = this.S1Vectors[x1, svIndex];
                var s2Value = this.S2Vectors[x2, svIndex];

                #region Inilined version of PredictRating
                // Get cached value for old features or default to an average
                var predictedValue = this.tempPredictions[dataIndex];

                // Add contribution of current feature
                predictedValue = predictedValue + (s1Value * s2Value);
                #endregion

                var err = games[dataIndex].ResultForPlayer1 - predictedValue;

                //Save old feature values before changing them
                var x1Increment = learningRate * ((err * s2Value) - (regularizationConstant * s1Value));
                var x2Increment = learningRate * ((err * s1Value) - (regularizationConstant * s2Value));

                //Update local variable instead of local to avoid contention
                predictionErrorSquareSum += err*err;

                // Cross-train the features using saved values
                this.S1Vectors[x1, svIndex] += x1Increment;
                this.S2Vectors[x2, svIndex] += x2Increment;
            }
            #endregion

            return predictionErrorSquareSum;
        }

        private void UpdateTempPredictions(int svIndex, IList<IGame> games)
        {
            var dataCount = games.Count;
            var svCount = SVCount;

            #region Update predictions
            // Save predictions for this feature
            for (var dataIndex = 0; dataIndex < dataCount; dataIndex++)
            {
                var x1 = this.playerProfile[games[dataIndex].Player1Id];
                var x2 = this.playerProfile[games[dataIndex].Player2Id];
                                   
                var predictedValue = this.tempPredictions[dataIndex];
                if (predictedValue > 5) predictedValue = 5;
                else if (predictedValue < 1) predictedValue = 1;

                predictedValue += this.S1Vectors[x1, svIndex] * this.S2Vectors[x2, svIndex];

                this.tempPredictions[dataIndex] = predictedValue;
            }
            #endregion
        }

        public double PredictGameResult(string player1Id, string player2Id, double player1Advantage = 0)
        {
            var x1 = this.playerProfile[player1Id];
            var x2 = this.playerProfile[player2Id];

            var predictedValue = 0d; //TODO: learningData.ItemProfiles.X2Avg[x2] + learningData.ItemProfiles.X1AvgBias[x1]; //1; 

            for (var svIndex = 0; svIndex <= SVCount; svIndex++)
            {
                predictedValue += (this.S1Vectors[x1, svIndex] * this.S2Vectors[x2, svIndex]);
                //TODO: limit values
            }

            return predictedValue;
        }
    }
}
