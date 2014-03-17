using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;

namespace GamePredictor
{
    public class EloPlusPlusPredictor : IRatingPredictor
    {
        double[] ratings;
        int maxIterations = 100;

        public void Initialize(int playerCount)
        {
            this.ratings = new double[playerCount];
        }

        public void Train(IList<IGame>[] games)
        {
            for(var iteration = 0; iteration < maxIterations; iteration++)
            {
                Utils.
            }
        }

        public double GetRating(int playerIndex)
        {
            return ratings[playerIndex];
        }

        public double PredictGameResult(int player1Index, int player2Index)
        {
            throw new NotImplementedException();
        }
    }
}
