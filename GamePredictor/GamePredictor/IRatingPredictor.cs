using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public interface IRatingPredictor
    {
        void Initialize(int playerCount);
        void Train(IList<IGame> games);
        double GetRating(int playerIndex);
        double PredictGameResult(int player1Index, int player2Index);
    }
}
