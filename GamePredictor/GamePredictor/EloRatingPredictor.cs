using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;

namespace GamePredictor
{
    public class EloRatingPredictor : IRatingPredictor
    {
        Dictionary<int, double> ratings = new Dictionary<int, double>();

        const double InitialRating = 25;

        public void Observe(int player1ID, int player2ID, double resultForPlayer1, double time)
        {
            var player1Rating = this.ratings.get
        }

        public double GetRating(int playerID)
        {
            throw new NotImplementedException();
        }

        public double PredictGameResult(int player1ID, int player2ID)
        {
            throw new NotImplementedException();
        }

        private override double GetValueKFactorForRating(double rating)
        {
            return (rating < 2400) ? 15 : 10;
        }
    }
}
