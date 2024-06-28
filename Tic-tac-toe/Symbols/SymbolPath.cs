using System;
using System.IO;


namespace Tic_tac_toe.Symbols
{
    internal static class SymbolPath
    {
        private static readonly string BasePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        private const string XRelativePath = "Symbols\\X.png";
        private const string ORelativePath = "Symbols\\O.png";

        public static readonly string XPath = Path.Combine(BasePath, XRelativePath);
        public static readonly string OPath = Path.Combine(BasePath, ORelativePath);
    }

}
