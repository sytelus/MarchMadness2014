using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePredictor
{
    public interface IGame
    {
       int Player1Index {get;}
       int player2Index {get;}
       double ResultForPlayer1 {get;}
       double Weight {get;}
       double ResultBiasForPlyer1 {get;}
    }
}
