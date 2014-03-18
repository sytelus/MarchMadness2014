using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public interface IGame
    {
       string Player1Id {get;}
       string Player2Id { get; }
        /// <summary>
        /// Number between 0 to 1 inclusive. If player 1 is true winner then 1, true looser then 0, 0.5 for draw.
        /// </summary>
       double ResultForPlayer1 {get;}
       double Weight {get;}
        /// <summary>
        /// Sometimes player have advantage in game, for example, white in chess or home team in baseball.
        /// Below number is between -ve to +ve infinity that is a boost in player's rating for this game. If not used, set to 0.
        /// To get this number for logistic distribution,
        /// 1. Get difference of average game outcomes for games with advantage and without advantage.
        /// 2. Get the rating difference by solving logistic equation.
        /// </summary>
       double Player1Advantage {get;}
    }
}
