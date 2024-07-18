using System;
using System.IO;

namespace TicTacToeGame.Client.Symbols
{
    public static class SymbolPath
    {
        private static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        private const string XRelativePath = "Symbols\\X.png";
        private const string ORelativePath = "Symbols\\O.png";
        private const string EmptyRelativePath = "Symbol\\Empty.png";

        public static readonly string XPath = Path.Combine(BasePath, XRelativePath);
        public static readonly string OPath = Path.Combine(BasePath, ORelativePath);
        public static readonly string EmptyPath = Path.Combine(BasePath, EmptyRelativePath);
    }
}
