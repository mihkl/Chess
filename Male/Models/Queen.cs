using Avalonia.Media.Imaging;
using Male.Models.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Male.Models {
    public class Queen(string color, int index): ChessPiece(color, index){
        public override Bitmap? PieceImage => new($"D:\\Proge\\Male\\Male\\Assets\\Queen{Color}.png");

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

        private void RemoveBlockedMoves(List<(int Row, int Column)> possibleMoves, ObservableCollection<ChessPiece?> chessPieces) {
            var blocking = FindWhereBlockingStarts(possibleMoves, chessPieces);

            foreach (var block in blocking) {
                var movesToRemove = possibleMoves.Where(move =>
                    
                    (block.Row < Row && block.Column < Column && move.Row <= block.Row && move.Column <= block.Column && IsDiagonalMove(move)) || 
                    (block.Row > Row && block.Column > Column && move.Row >= block.Row && move.Column >= block.Column && IsDiagonalMove(move)) || 
                    (block.Row < Row && block.Column > Column && move.Row <= block.Row && move.Column >= block.Column && IsDiagonalMove(move)) || 
                    (block.Row > Row && block.Column < Column && move.Row >= block.Row && move.Column <= block.Column && IsDiagonalMove(move)) || 
                                                                                                                                                  
                    (block.Row < Row && block.Column == Column && move.Row <= block.Row && move.Column == block.Column && IsVerticalMove(move)) || 
                    (block.Row > Row && block.Column == Column && move.Row >= block.Row && move.Column == block.Column && IsVerticalMove(move)) || 
                    (block.Column < Column && block.Row == Row && move.Column <= block.Column && move.Row == block.Row && IsHorizontalMove(move)) || 
                    (block.Column > Column && block.Row == Row && move.Column >= block.Column && move.Row == block.Row && IsHorizontalMove(move)) 
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

        private bool IsDiagonalMove((int Row, int Column) move) => Math.Abs(move.Row - Row) == Math.Abs(move.Column - Column);

        private bool IsVerticalMove((int Row, int Column) move) => move.Column == Column;

        private bool IsHorizontalMove((int Row, int Column) move) => move.Row == Row;

        private List<(int Row, int Column)> FindWhereBlockingStarts(List<(int Row, int Column)> possibleMoves, ObservableCollection<ChessPiece?> chessPieces) {
            var blocking = new List<(int Row, int Column)>();

            foreach (var move in possibleMoves) {
                if (CanCapture(move, chessPieces)) {
                    if (IsDiagonalMove(move)) {
                        blocking.Add(GetNextDiagonalPosition(move));
                    } else if (IsVerticalMove(move)) {
                        blocking.Add(GetNextVerticalPosition(move));
                    } else if (IsHorizontalMove(move)) {
                        blocking.Add(GetNextHorizontalPosition(move));
                    }
                } else if (IsMoveBlocked(move, chessPieces)) {
                    blocking.Add(move);
                }
            }
            return blocking;
        }

        private (int Row, int Column) GetNextDiagonalPosition((int Row, int Column) move) {
            int rowDirection = move.Row < Row ? -1 : 1;
            int colDirection = move.Column < Column ? -1 : 1;
            return (move.Row + rowDirection, move.Column + colDirection);
        }

        private (int Row, int Column) GetNextVerticalPosition((int Row, int Column) move) {
            int rowDirection = move.Row < Row ? -1 : 1;
            return (move.Row + rowDirection, move.Column);
        }

        private (int Row, int Column) GetNextHorizontalPosition((int Row, int Column) move) {
            int colDirection = move.Column < Column ? -1 : 1;
            return (move.Row, move.Column + colDirection);
        }

    }
}
