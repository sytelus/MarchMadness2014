﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public interface IGameLearner
    {
        void Train(IList<IGame> games);
        double PredictGameResult(string player1Id, string player2Id, double player1Advantage = 0);
    }
}