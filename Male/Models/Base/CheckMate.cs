using Avalonia.Media;
using Avalonia.Remote.Protocol.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Male.Models.Base {
    public class CheckMate {
        public static bool IsKingInCheck(ChessPiece king, ObservableCollection<ChessPiece?> chessPieces) {
            var kingColor = king.Color;
            var enemyPieces = chessPieces.Where(x => x?.Color != kingColor && x is not EmptyPiece).ToList();
            var enemyMoves = GetAllPossibleMoves(chessPieces, enemyPieces).ToList();

            var debugMove = enemyMoves.Where(x => x == king.Position).ToList();
            return enemyMoves.Any(x => x == king.Position);
        }

        private static List<(int Row, int Column)> GetAllPossibleMoves(ObservableCollection<ChessPiece?> chessPieces, List<ChessPiece?> enemyPieces) {
            var moves = new List<(int Row, int Column)>();
            foreach (var piece in enemyPieces) {
                if (piece != null) {
                    moves.AddRange(piece.GetPossibleMoves(chessPieces));
                }
            }
            return moves;
        }

        public static bool IsCheckmate(ChessPiece king, ObservableCollection<ChessPiece?> chessPieces, bool isKingInCheck) {
            if (!isKingInCheck) return false;

            bool isCheckmate = true;
            var friendlyPieces = chessPieces.Where(x => x?.Color == king.Color).ToList();
            var newChessPieces = new ObservableCollection<ChessPiece?>(chessPieces);
            Dictionary<int, string> debugMoves = [];

            int i = 0;
            foreach (var piece in friendlyPieces) {
                
                var moves = piece!.GetPossibleMoves(chessPieces);
                foreach (var move in moves) {
                    bool savesKing = SimulateMove(piece, move, newChessPieces, king);
                    debugMoves[i] = $"{piece.GetType()} {piece.Color} {piece.Position} -> Move: {move}, Saves king: {savesKing}";
                    i++;
                    if (SimulateMove(piece, move, newChessPieces, king)) {
                        isCheckmate = false;
                    }
                }
            }
            debugMoves = debugMoves.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            return isCheckmate;
        }

        private static bool SimulateMove(ChessPiece piece, (int Row, int Column) move, ObservableCollection<ChessPiece?> multiVerseBoard, ChessPiece king) {
            
            var index = move.Row * 8 + move.Column;
            var pieceIndex = piece.Index;
            multiVerseBoard[index] = piece;
            piece.Index = index;
            multiVerseBoard[pieceIndex] = new EmptyPiece(pieceIndex);

            if (piece.GetType().Name == "King") {
                king = multiVerseBoard[index]!;
            }

            if (!IsKingInCheck(king, multiVerseBoard)) {
                CleanUp(piece, multiVerseBoard, index, pieceIndex);
                return true;
            } else {
                CleanUp(piece, multiVerseBoard, index, pieceIndex);
                return false;
            }
        }

        private static void CleanUp(ChessPiece piece, ObservableCollection<ChessPiece?> multiVerseBoard, int index, int pieceIndex) {
            multiVerseBoard[index] = new EmptyPiece(index);
            multiVerseBoard[pieceIndex] = piece;
            piece.Index = pieceIndex;
        }
    }
}
