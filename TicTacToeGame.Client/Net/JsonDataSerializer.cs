using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Net
{
    public static class JsonDataSerializer
    {
        public static string SerializeGameBoardData(List<BoardCell> cells)
        {
            string json = JsonConvert.SerializeObject(cells);

            return json;
        }

        public static List<BoardCell> DeserializeGameBoardData(string boardJson)
        {
            if(string.IsNullOrEmpty(boardJson))
            {
                Debug.WriteLine($"Null data recived.");
                return null;
            }
            List<BoardCell> boardCells = JsonConvert.DeserializeObject<List<BoardCell>>(boardJson);

            return boardCells;
        }

        public static string SerializeCell(BoardCell cell)
        {
            return JsonConvert.SerializeObject(cell);
        }

        public static BoardCell DeserializeCell(string cellJson)
        {
            return JsonConvert.DeserializeObject<BoardCell>(cellJson);
        }

        public static string SerializeMove(PlayerMove move)
        {
            return JsonConvert.SerializeObject(move);
        }

        public static PlayerMove DeserializeMove(string move)
        {
            return JsonConvert.DeserializeObject<PlayerMove>(move);
        }
    }
}
