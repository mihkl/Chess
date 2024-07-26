using Avalonia;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Male.Models.Base
{
    public abstract class ChessPiece(string? color, int index) {
        public string? Color { get; set; } = color;
        public int Index { get; set; } = index;
        public int Row => Index / 8;
        public int Column => Index % 8;
        public (int, int) Position => (Row, Column);
        public virtual Bitmap? PieceImage { get; }
        public virtual List<(int, int)> GetPossibleMoves(ObservableCollection<ChessPiece?> chessPieces) { return []; }
        public string PieceInfo => $"{Color} {GetType().Name} at {Row}, {Column}";

        public virtual void Move(ChessPiece selectedPiece, ChessPiece piece, ObservableCollection<ChessPiece?> chessPieces) {
            if (selectedPiece is null) return;
            var index = piece.Index;
            var selectedPieceIndex = selectedPiece.Index;
            chessPieces[index] = selectedPiece;
            selectedPiece.Index = index;
            chessPieces[selectedPieceIndex] = new EmptyPiece(selectedPieceIndex);
        }
        protected virtual bool IsMoveBlocked((int Row, int Column) move, ObservableCollection<ChessPiece?> chessPieces) {
            if (chessPieces.Any(x => (x is not EmptyPiece) && x?.Row == move.Row && x?.Column == move.Column)) {
                return true;
            }
            return false;
        }
        protected virtual bool CanCapture((int Row, int Column) move, ObservableCollection<ChessPiece?> chessPieces) {
            if (chessPieces.Any(x => (x?.Color != Color) && (x is not EmptyPiece) && x?.Row == move.Row && x?.Column == move.Column)) {
                return true;
            }
            return false;
        }

        protected virtual bool IsOutOfBounds((int Row, int Column) move) {
            if (move.Row < 0 || move.Row > 7 || move.Column < 0 || move.Column > 7) {
                return true;
            }
            return false;
        }
    }
}
