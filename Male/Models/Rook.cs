using Avalonia.Media.Imaging;
using Male.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Male.Models {
    public class Rook(string color, int index): ChessPiece(color, index){
        public override Bitmap? PieceImage => new($"D:\\Proge\\Male\\Male\\Assets\\Rook{Color}.png");
        public override List<(int, int)> GetPossibleMoves(ObservableCollection<ChessPiece?> chessPieces){
            List<(int, int)> possibleMoves = [];

            for (int i = 0; i < 8; i++) {
                if (i != Index % 8) {
                    var move = (Index / 8, i);
                    possibleMoves.Add(move);
                }
                if (i != Index / 8) {
                    var move = (i, Index % 8);
                    possibleMoves.Add(move);
                }
            }
            RemoveBlockedMoves(possibleMoves, chessPieces);

            return possibleMoves;
        }
        private void RemoveBlockedMoves(List<(int Row, int Column)> possibleMoves, ObservableCollection<ChessPiece?> chessPieces){

            var blocking = FindWhereBlockingStarts(possibleMoves, chessPieces);

            foreach (var block in blocking) {
                var movesToRemove = possibleMoves.Where(move =>
                    (block.Row < Row && move.Row <= block.Row && move.Column == block.Column) ||
                    (block.Row > Row && move.Row >= block.Row && move.Column == block.Column) ||
                    (block.Column < Column && move.Column <= block.Column && move.Row == block.Row) ||
                    (block.Column > Column && move.Column >= block.Column && move.Row == block.Row)
                ).ToList();

                foreach (var move in movesToRemove) {
                    possibleMoves.Remove(move);
                }
            }
            var illegalMoves = possibleMoves.Where(move => IsOutOfBounds(move)).ToList();
            foreach (var move in illegalMoves) {
                possibleMoves.Remove(move);
            }
        }

        private List<(int Row, int Column)> FindWhereBlockingStarts(List<(int Row, int Column)> possibleMoves, ObservableCollection<ChessPiece?> chessPieces) {
            
            var blocking = new List<(int Row, int Column)>();

            for (int i = 0; i < possibleMoves.Count; i++) {
                var move = possibleMoves[i];
                if (CanCapture(move, chessPieces)) {

                    if (move.Row < Row) blocking.Add((move.Row - 1, Column));
                    if (move.Row > Row) blocking.Add((move.Row + 1, Column));
                    if (move.Column < Column) blocking.Add((Row, move.Column - 1));
                    if (move.Column > Column) blocking.Add((Row, move.Column + 1));

                } else if (IsMoveBlocked(move, chessPieces)) blocking.Add(move);
            }
            return blocking;
        }
    }
}
