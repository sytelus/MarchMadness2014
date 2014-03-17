using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;

namespace GamePredictor
{
    public class Game
    {
        public string Season { get; private set; }
        public int DayNumber { get; private set; }
        public int WinningTeamID { get; private set; }
        public int WinningTeamScore { get; private set; }
        public int LoosingTeamID { get; private set; }
        public int LoosingTeamScore { get; private set; }
        public bool? IsWinningTeamHome { get; private set; }
        public int? OvertimePeriods { get; private set; }

        public Game(string[] valueColumns, string[] headerColumns)
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
    }
}
