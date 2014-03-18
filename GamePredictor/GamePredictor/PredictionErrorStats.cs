using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public class PredictionErrorStats
    {
        double totalErrorSquare = 0;
        double totalLogLoss = 0;
        int count = 0;

        public void Observe(double actual, double predicted)
        {
            totalErrorSquare += (actual - predicted) * (actual - predicted);
            totalLogLoss += actual * Math.Log(predicted) + (1 - actual) * Math.Log(1 - predicted);
            count++;
        }

        public double GetRmse()
        {
            return Math.Sqrt(totalErrorSquare / count);
        }

        public double GetLogLoss()
        {
            return -totalLogLoss / count;
        }
    }
}
