using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public class AlwaysDrawPredictor : IRatingPredictor
    {

        public void Train(IList<IGame> games)
        {
        }

        public double GetRating(int playerIndex)
        {
            return 0;
        }

        public double PredictGameResult(string player1Id, string player2Id, double player1Advantage = 0)
        {
            return 0.5;
        }
    }
}
