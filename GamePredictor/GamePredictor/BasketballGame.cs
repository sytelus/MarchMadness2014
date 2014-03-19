using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;

namespace GamePredictor
{
    public class BasketballGame : IGame
    {
        public string Season { get; private set; }
        public int DayNumber { get; private set; }
        public int WinningTeamID { get; private set; }
        public int WinningTeamScore { get; private set; }
        public int LoosingTeamID { get; private set; }
        public int LoosingTeamScore { get; private set; }
        public bool? IsWinningTeamHome { get; private set; }
        public int? OvertimePeriods { get; private set; }

        private const double HomeAdvantageFactor = 0;   // 0.0838371309618372;

        public BasketballGame(string[] valueColumns, string[] headerColumns)
        {
            for(var headerIndex = 0; headerIndex < headerColumns.Length; headerIndex++)
            {
                switch(headerColumns[headerIndex])
                {
                    case "season":
                        this.Season = valueColumns[headerIndex]; break;
                    case "daynum":
                        this.DayNumber = int.Parse(valueColumns[headerIndex]); break;
                    case "wteam":
                        this.WinningTeamID = int.Parse(valueColumns[headerIndex]); break;
                    case "wscore":
                        this.WinningTeamScore = int.Parse(valueColumns[headerIndex]); break;
                    case "lteam":
                        this.LoosingTeamID = int.Parse(valueColumns[headerIndex]); break;
                    case "lscore":
                        this.LoosingTeamScore = int.Parse(valueColumns[headerIndex]); break;
                    case "wloc":
                        this.IsWinningTeamHome = ParseTeamLocation(valueColumns[headerIndex]); break;
                    case "numot":
                        this.OvertimePeriods = ParseOvertimePeriods(valueColumns[headerIndex]); break;
                    default:
                        throw new ArgumentException("Header column value '{0}' is not recognized".FormatEx(headerColumns[headerIndex]));
                }
            }

            this.Player1Id = this.WinningTeamID.ToStringInvariant();
            this.Player2Id = this.LoosingTeamID.ToStringInvariant();

            this.Player1WinMeasure = GetWinMeasure(this.WinningTeamScore, this.LoosingTeamScore);

            this.Player1Score = this.WinningTeamScore;
            this.Player2Score = this.LoosingTeamScore;

            //Below value is found by trial & error
            this.Player1ScoreAdusted = this.IsWinningTeamHome.IsTrue() ? (1 - HomeAdvantageFactor) * this.Player1Score : this.Player1Score;
            this.Player2ScoreAdusted = this.IsWinningTeamHome.IsFalse() ? (1 - HomeAdvantageFactor) * this.Player2Score : this.Player2Score;
            this.Player1WinMeasureAdjusted = GetWinMeasure(this.Player1ScoreAdusted, this.Player2ScoreAdusted);
            this.Player1HasAdvantage = this.IsWinningTeamHome;
        }

        private static double GetWinMeasure(double winningTeamScore, double loosingTeamScore)
        {
            //For most of the games ration W/ (W + L) is very close to 0.5 indicating scores were very similar. Below eq models the fact 
            //that it is exponentially more difficult to generate the score difference.
            return Logistic(winningTeamScore - loosingTeamScore);
        }

        private static double Logistic(double value)
        {
            return 1 / (1 + Math.Pow(Math.E, -value));
        }

        private int? ParseOvertimePeriods(string value)
        {
            if (value == "NA")
                return null;
            else
                return int.Parse(value);
        }

        private bool? ParseTeamLocation(string value)
        {
            switch(value)
            {
                case "H": return true;
                case "A": return false;
                case "N": return null;
                default:
                    throw new ArgumentException("Home location column value '{0}' can't be parsed".FormatEx(value));
            }
        }

        #region IGame
        public string Player1Id {get; private set; }
        public string Player2Id { get; private set; }
        public double Player1WinMeasure { get; private set; }
        public double Player1Score { get; private set; }
        public double Player2Score { get; private set; }

        public double Weight
        {
            get { return 1; }
        }

        public double Player1ScoreAdusted { get; private set; }
        public double Player2ScoreAdusted { get; private set; }
        public double Player1WinMeasureAdjusted { get; private set; }
        public bool? Player1HasAdvantage { get; private set; }
        #endregion
    }
}
