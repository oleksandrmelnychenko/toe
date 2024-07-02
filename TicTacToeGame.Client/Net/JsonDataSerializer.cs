using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Net
{
    public static class JsonDataSerializer
    {
        public static string SerializeGameData(List<BoardCell> cells)
        {
            string json = JsonConvert.SerializeObject(cells);

            return json;
        }

        public static List<BoardCell> DeserializeGameData(string json)
        {
            if(string.IsNullOrEmpty(json))
            {
                Debug.WriteLine($"Null data recived.");
                return null;
            }
            List<BoardCell> boardCells = JsonConvert.DeserializeObject<List<BoardCell>>(json);

            return boardCells;
        }
    }
}
