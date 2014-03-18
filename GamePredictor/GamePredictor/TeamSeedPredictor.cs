using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public class TeamSeedPredictor : IGameLearner
    {
        IDictionary<string, int> teamSeeds;
        public TeamSeedPredictor(IDictionary<string, int> teamSeeds)
        {
            this.teamSeeds = teamSeeds;
        }

        public void Train(IList<IGame> games)
        {
            
        }

        public double PredictGameResult(string player1Id, string player2Id, double player1Advantage = 0)
        {
            return 0.5 + 0.03 * (this.teamSeeds[player2Id] - this.teamSeeds[player1Id]);
        }
    }
}
