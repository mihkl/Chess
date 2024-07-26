using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Male.Models.Base
{
    public class ChessSquare: ReactiveObject {
        private bool isValidMove;

        public int Row { get; set; }
        public int Column { get; set; }

        public bool IsValidMove {
            get => isValidMove;
            set => this.RaiseAndSetIfChanged(ref isValidMove, value);
        }

        public string BackgroundColor => (Row + Column) % 2 == 0 ? "White" : "#63430a";

        public void UpdateValidMoves(ObservableCollection<(int, int)> validMoves) {
            IsValidMove = validMoves.Contains((Row, Column));
        }
    }


}
