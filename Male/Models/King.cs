using Avalonia.Media.Imaging;
using Male.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Male.Models {
    public class King(string color, int index): ChessPiece(color, index){
        public override Bitmap? PieceImage => new($"D:\\Proge\\Male\\Male\\Assets\\King{Color}.png");
        public override List<(int, int)> GetPossibleMoves(ObservableCollection<ChessPiece?> chessPieces) {

            List<(int, int)> possibleMoves = [];

            int[] x = [1, 1, 1, 0, 0, -1, -1, -1];
            int[] y = [-1, 0, 1, -1, 1, -1, 0, 1];
            for (int i = 0; i < 8; i++) {
                var move = (Row + x[i], Column + y[i]);
                if (!IsMoveBlocked(move, chessPieces) || CanCapture(move, chessPieces)) {
                    possibleMoves.Add(move);
                }
            }
            var illegalMoves = possibleMoves.Where(move => IsOutOfBounds(move)).ToList();
            foreach (var move in illegalMoves) {
                possibleMoves.Remove(move);
            }
            return possibleMoves;
        }
    }
}

