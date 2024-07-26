using System.Collections.ObjectModel;
using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Reactive;
using Male.Models.Base;
using Male.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Collections.Generic;


namespace Male.ViewModels {
    public class MainViewModel: ViewModelBase {
        public ReactiveCommand<ChessPiece?, Unit> SelectPiece { get; }
        public CheckMate CheckMate { get; set; } = new();
        public ObservableCollection<ChessSquare> ChessBoard { get; set; }
        public ObservableCollection<ChessPiece?> ChessPieces { get; set; }
        public ObservableCollection<(int, int)> CurrentValidMoves { get; set; }
        public ObservableCollection<ChessPiece?> BlackPieces { get; set; }
        public ObservableCollection<ChessPiece?> WhitePieces { get; set; }
        private List<ChessPiece?> piecesList => [.. ChessPieces];
        public bool IsWhiteTurn { get; set; } = true;
        public bool IsBlackTurn => !IsWhiteTurn;
        private bool isCheckmate;
        private bool isKingInCheck;

        private ChessPiece? selectedPiece;
        private string pieceInfo = "No piece selected";

        public ChessPiece? SelectedPiece {
            get => selectedPiece;
            set {
                this.RaiseAndSetIfChanged(ref selectedPiece, value);
                PieceInfo = selectedPiece?.PieceInfo ?? "No piece selected"; 
            }
        }

        public string PieceInfo {
            get => pieceInfo;
            set => this.RaiseAndSetIfChanged(ref pieceInfo, value);
        }

        public bool IsCheckmate {
            get => isCheckmate;
            set => this.RaiseAndSetIfChanged(ref isCheckmate, value);
        }
        
        public bool IsKingInCheck {
            get => isKingInCheck;
            set => this.RaiseAndSetIfChanged(ref isKingInCheck, value);
        }

        public MainViewModel() {
            ChessBoard = [];
            ChessPieces = [];
            CurrentValidMoves = [];
            BlackPieces = [];
            WhitePieces = [];
            SelectPiece = ReactiveCommand.Create<ChessPiece?>(TrySelectPiece);

            InitializeBoard();
            InitializePieces();
            UpdateBlackPieces();
            UpdateWhitePieces();
        }

        private void UpdateWhitePieces() {
            WhitePieces.Clear();
            foreach (var piece in ChessPieces) {
                if (piece?.Color == "White") {
                    WhitePieces.Add(piece);
                }
            }
        }

        private void UpdateBlackPieces() {
            BlackPieces.Clear();
            foreach (var piece in ChessPieces) {
                if (piece?.Color == "Black") {
                    BlackPieces.Add(piece);
                }
            }
        }

        private void TrySelectPiece(ChessPiece? piece) {
            if (piece is null) return;

            if (SelectedPiece is null && piece is EmptyPiece) {
                return;
            }
            if (SelectedPiece is null || SelectedPiece?.Color == piece.Color) {
                if (piece?.Color == "Black" && IsWhiteTurn) return;
                if (piece?.Color == "White" && IsBlackTurn) return;
                SelectedPiece = piece;
                CurrentValidMoves = AddPossibleMoves();
                return;
            }
            if (SelectedPiece == piece) {
                SelectedPiece = null;
                CurrentValidMoves = [];
                return;
            }
            if (CurrentValidMoves.Contains((piece.Row, piece.Column))) {
                MakeMove(piece);
            }
        }

        private void MakeMove(ChessPiece piece) {
            SelectedPiece?.Move(SelectedPiece, piece, ChessPieces);
            CurrentValidMoves = [];
            UpdateValidMoves(CurrentValidMoves);
            SelectedPiece = null;
            CurrentValidMoves = [];

            IsCheckOrCheckmate();
        }

        private void IsCheckOrCheckmate() {
            if (IsWhiteTurn) {
                IsWhiteTurn = false;
                UpdateBlackPieces();

                var king = BlackPieces.ToList().FirstOrDefault(x => x?.GetType().Name == "King");
                IsKingInCheck = CheckMate.IsKingInCheck(king!, ChessPieces);
                IsCheckmate = CheckMate.IsCheckmate(king!, ChessPieces, IsKingInCheck);

            } else {
                IsWhiteTurn = true;
                UpdateWhitePieces();

                var king = WhitePieces.ToList().FirstOrDefault(x => x?.GetType().Name == "King");
                IsKingInCheck = CheckMate.IsKingInCheck(king!, ChessPieces);
                IsCheckmate = CheckMate.IsCheckmate(king!, ChessPieces, IsKingInCheck);
            }
        }

        private ObservableCollection<(int, int)> AddPossibleMoves() {
            if (SelectedPiece is null) return [];
            var moves = SelectedPiece.GetPossibleMoves(ChessPieces);
            var validMoves = new ObservableCollection<(int, int)>();
            foreach (var move in moves) {
                if (move.Item1 >= 0 && move.Item1 < 8 && move.Item2 >= 0 && move.Item2 < 8) {
                    validMoves.Add(move);
                }
            }
            UpdateValidMoves(validMoves);
            return validMoves;
        }

        private void UpdateValidMoves(ObservableCollection<(int, int)> validMoves) {
            foreach (var square in ChessBoard) {
                square.UpdateValidMoves(validMoves);
            }
        }

        private void InitializeBoard() {
            for (int row = 0; row < 8; row++) {
                for (int col = 0; col < 8; col++) {
                    ChessBoard.Add(new ChessSquare {
                        Row = row,
                        Column = col,
                    });
                }
            }
        }

        private void InitializePieces() {
            AddMajorPieces("Black", 0);
            AddPawns("Black", 8);
            AddEmptySpaces();
            AddPawns("White", 48);
            AddMajorPieces("White", 56);
        }

        private void AddMajorPieces(string color, int startIndex) {
            var pieces = new Type[] { typeof(Rook), typeof(Knight), typeof(Bishop), typeof(Queen), typeof(King), typeof(Bishop), typeof(Knight), typeof(Rook) };
            for (int i = 0; i < pieces.Length; i++) {
                ChessPieces.Add((ChessPiece?)Activator.CreateInstance(pieces[i], color, startIndex + i));
            }
        }

        private void AddPawns(string color, int startIndex) {
            for (int col = 0; col < 8; col++) {
                ChessPieces.Add(new Pawn(color, startIndex + col));
            }
        }

        private void AddEmptySpaces() {
            for (int row = 2; row < 6; row++) {
                for (int col = 0; col < 8; col++) {
                    ChessPieces.Add(new EmptyPiece((row * 8) + col));
                }
            }
        }
    }
}
