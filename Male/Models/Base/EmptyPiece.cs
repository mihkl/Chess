using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Male.Models.Base {
    public class EmptyPiece(int index): ChessPiece(null, index) {
        public override Bitmap? PieceImage => new($"D:\\Proge\\Male\\Male\\Assets\\Empty.png");
    }
}
