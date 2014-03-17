using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;
using System.IO;

namespace GamePredictor
{
    public class GameResultParser
    {
        public IDictionary<string, List<Game>> GamesBySeasons {get; private set;}
        public GameResultParser(string filePath)
        {
            this.GamesBySeasons = new Dictionary<string, List<Game>>();

            using(var file = File.OpenText(filePath))
            {
                var headerColumns = file.ReadLine().Split(Utils.CommaDelimiter); //header

                var line = file.ReadLine();
                while (line != null)
                {
                    var columns = line.Split(Utils.CommaDelimiter);

                    var game = new Game(columns, headerColumns);

                    GamesBySeasons.AddToDictionarySet(game.Season, game);

                    line = file.ReadLine();
                }
            }
        }
    }
}
