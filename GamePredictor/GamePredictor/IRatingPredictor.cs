using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public interface IRatingPredictor
    {
        void Observe(int player1ID, int player2ID, double resultForPlayer1, double time);
        double GetRating(int playerID);
        double PredictGameResult(int player1ID, int player2ID);
    }
}
