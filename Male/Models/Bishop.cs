using Avalonia.Media.Imaging;
using Male.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Male.Models {
    public class Bishop(string color, int index): ChessPiece(color, index){
        public override Bitmap? PieceImage => new($"D:\\Proge\\Male\\Male\\Assets\\Bishop{Color}.png");

        public override List<(int, int)> GetPossibleMoves(ObservableCollection<ChessPiece?> chessPieces) {
            List<(int, int)> possibleMoves = [];

            for (int i = 1; i < 8; i++) {
                var move = (Row + i, Column + i);
                possibleMoves.Add(move);

                move = (Row - i, Column - i);
                possibleMoves.Add(move);

                move = (Row + i, Column - i);
                possibleMoves.Add(move);

                move = (Row - i, Column + i);
                possibleMoves.Add(move);
            }
            RemoveBlockedMoves(possibleMoves, chessPieces);
            return possibleMoves;
        }

        private void RemoveBlockedMoves(List<(int Row, int Column)> possibleMoves, ObservableCollection<ChessPiece?> chessPieces) {

            var blocking = FindWhereBlockingStarts(possibleMoves, chessPieces);

            foreach (var block in blocking) {
                var movesToRemove = possibleMoves.Where(move =>
                    (block.Row < Row && block.Column < Column && move.Row <= block.Row && move.Column <= block.Column) ||
                    (block.Row > Row && block.Column > Column && move.Row >= block.Row && move.Column >= block.Column) ||
                    (block.Row < Row && block.Column > Column && move.Row <= block.Row && move.Column >= block.Column) ||
                    (block.Row > Row && block.Column < Column && move.Row >= block.Row && move.Column <= block.Column)
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

                    if (move.Row < Row && move.Column < Column) blocking.Add((move.Row - 1, move.Column -1));
                    if (move.Row > Row && move.Column > Column) blocking.Add((move.Row + 1, move.Column + 1));
                    if (move.Row < Row && move.Column > Column) blocking.Add((move.Row - 1, move.Column + 1));
                    if (move.Row > Row && move.Column < Column) blocking.Add((move.Row + 1, move.Column - 1));

                } else if (IsMoveBlocked(move, chessPieces)) blocking.Add(move);
            }
            return blocking;
        }
    }

}
