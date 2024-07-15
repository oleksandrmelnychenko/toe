using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tic_tac_toe_Server.Game
{
    public record BoardCell
    {
        public ushort Index { get; init; }
        public Symbol Value { get; set; }
        public bool IsDirty { get; set; }

        [JsonConstructor]
        public BoardCell(ushort index, Symbol value, bool isDirty = false)
        {
            Index = index;
            Value = value;
            IsDirty = isDirty;
        }
        public BoardCell(ushort index) : this(index, Symbol.Empty, false)
        {
        }
    }
}
