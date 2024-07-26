using Avalonia.Media.Imaging;
using Male.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Male.Models {
    public class Pawn(string color, int index): ChessPiece(color, index){
        public override Bitmap? PieceImage => new($"D:\\Proge\\Male\\Male\\Assets\\Pawn{Color}.png");
        public bool HasMoved { get; set; } = false;

        public override List<(int, int)> GetPossibleMoves(ObservableCollection<ChessPiece?> chessPieces){
            List<(int, int)> possibleMoves = [];

            var move = ((Color == "Black" ? Row + 1 : Row - 1, Column));

            if (!IsMoveBlocked(move, chessPieces)) {
                possibleMoves.Add(move);
            }
            if (!HasMoved){
                move = ((Color == "Black" ? Row + 2 : Row - 2, Column));
                if (!IsMoveBlocked(move, chessPieces)){
                    possibleMoves.Add(move);
                }
            }

            move = ((Color == "Black" ? Row + 1 : Row - 1, Column + 1));

            if (CanCapture(move, chessPieces)){
                possibleMoves.Add(move);
            }

            move = ((Color == "Black" ? Row + 1 : Row - 1, Column - 1));

            if (CanCapture(move, chessPieces)){
                possibleMoves.Add(move);
            }

            var illegalMoves = possibleMoves.Where(move => IsOutOfBounds(move)).ToList();
            foreach (var imove in illegalMoves) {
                possibleMoves.Remove(imove);
            }
            return possibleMoves;
        }

        public override void Move(ChessPiece selectedPiece, ChessPiece piece, ObservableCollection<ChessPiece?> chessPieces) {
            base.Move(selectedPiece, piece, chessPieces);
            HasMoved = true;
        }
    }
}
