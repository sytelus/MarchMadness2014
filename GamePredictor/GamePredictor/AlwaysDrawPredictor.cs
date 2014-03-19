using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public class AlwaysDrawPredictor : IGameLearner
    {

        public void Train(IList<IGame> games)
        {
        }

        public void PredictGameResult(string player1Id, string player2Id, out double player1ScorePrediction, out double player2ScorePrediction)
        {
            player1ScorePrediction = 0.5;
            player2ScorePrediction = 0.5;
        }
    }
}
