using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public interface IGameLearner
    {
        void Train(IList<IGame> games);

        void PredictGameResult(string player1Id, string player2Id, out double player1ScorePrediction, out double player2ScorePrediction);
    }
}
