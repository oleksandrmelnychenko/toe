using Newtonsoft.Json;

namespace TicTacToeGame.Client.Game
{
    public record BoardCell
    {
        public ushort Index { get; init; }
        public Symbol? Value { get; set; }
        public bool IsDirty { get; set; }

        [JsonConstructor]
        public BoardCell(ushort index, Symbol? value = null, bool isDirty = false)
        {
            Index = index;
            Value = value;
            IsDirty = isDirty;
        }
        public BoardCell(ushort index) : this(index, null, false)
        {
        }
    }
}
