using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;

namespace Tic_tac_toe.Models
{
    public class Cell : CellBase
    {
        //private Guid? id;
        private Bitmap? boxImage;
        private string? symbolName;
        public bool IsEmpty { get; set; } = true;   

        //public Guid? Id { get; init; }

        //public Cell()
        //{
        //    id = Guid.NewGuid();
        //}

        public Bitmap BoxImage
        {
            get { return boxImage; }
            private set
            {
                boxImage = value;
            }
        }

        public string SymbolName
        {
            get { return symbolName; }
            private set
            {
                symbolName = value;
            }
        }

        public void BoxReset()
        {
            IsEmpty = true;
            BoxImage = null;
            SymbolName = null;
        }

        public void BoxSetValues(Bitmap image, string symbolName)
        {
            BoxImage = image;
            SymbolName = symbolName;
            IsEmpty = false;
        }
        
    }
}
