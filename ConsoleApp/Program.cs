using System;

namespace chess
{
    class ChessGamelauncher
    {
        static void Main(string[] args)
        {

            new ChessGame().play();
        }
    }

    class ChessGame
    {
        ChessPiece[,] board;
        int numOfMoves;
        int[] timesBoardAppeared;
        string boardsHistory;
        int fiftyMovesIndex;
        string pawnMovestwostep;
        King whiteKing;
        King blackKing;
        bool blackKingMoved;
        bool whiteKingMoved;
        bool blackRightRookMoved;
        bool blackLeftRookMoved;
        bool whiteRightRookMoved;
        bool whiteLeftRookMoved;


        public ChessGame()
        {
            board = new ChessPiece[8, 8];
            timesBoardAppeared = new int[50];
            whiteKing = new King(7, 4, true);
            blackKing = new King(0, 4, false);
            bool color = false;
            for (int i = 0; i < 8; i += 7)
            {
                board[i, 0] = new Rook(i, 0, color);
                board[i, 7] = new Rook(i, 7, color);
                board[i, 1] = new Knight(i, 1, color);
                board[i, 6] = new Knight(i, 6, color);
                board[i, 2] = new Bishop(i, 2, color);
                board[i, 5] = new Bishop(i, 5, color);
                board[i, 4] = color ? whiteKing : blackKing;
                board[i, 3] = new Queen(i, 3, color);
                color = !color;
            }
            for (int i = 1; i < 7; i += 5)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new Pawn(i, j, color);
                }
                color = !color;
            }
            boardsHistory = this.ToString();
            timesBoardAppeared[0] = 1;
        }

        public ChessPiece[,] getBoard()
        {
            return board;
        }
        public int getNumOfMoves()
        {
            return numOfMoves;
        }
        public string getBoardsHistory()
        {
            return boardsHistory;
        }
        public string getPawnMovestwostep()
        {
            return pawnMovestwostep;
        }
        public King getWhiteKing()
        {
            return whiteKing;
        }
        public King getBlackKing()
        {
            return blackKing;
        }
        public bool getBlackKingMoved()
        {
            return blackKingMoved;
        }
        public bool getWhiteKingMoved()
        {
            return whiteKingMoved;
        }
        public bool getBlackRightRookMoved()
        {
            return blackRightRookMoved;
        }
        public bool getBlackLeftRookMoved()
        {
            return blackLeftRookMoved;
        }
        public bool getWhiteRightRookMoved()
        {
            return whiteRightRookMoved;
        }
        public bool getWhiteLeftRookMoved()
        {
            return whiteLeftRookMoved;
        }
        public override string ToString() // Print the board as string
        {
            string[,] outputBoard = new string[9, 9]; // String output of the current board
            outputBoard[0, 0] = " ";
            outputBoard[0, 1] = " A"; outputBoard[0, 2] = " B"; outputBoard[0, 3] = " C"; outputBoard[0, 4] = " D"; outputBoard[0, 5] = " E"; outputBoard[0, 6] = " F"; outputBoard[0, 7] = " G"; outputBoard[0, 8] = " H";
            outputBoard[8, 0] = " 1"; outputBoard[7, 0] = " 2"; outputBoard[6, 0] = " 3"; outputBoard[5, 0] = " 4"; outputBoard[4, 0] = " 5"; outputBoard[3, 0] = " 6"; outputBoard[2, 0] = " 7"; outputBoard[1, 0] = " 8";

            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    var piece = this.board[i - 1, j - 1];
                    outputBoard[i, j] = piece != null ? piece.ToString().Trim() : "  ";
                }
            }

            string finalBoard = "";
            // Add graphical view
            finalBoard = " ---------------------------" + "\n";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    finalBoard += outputBoard[i, j] + "|";
                }
                finalBoard += "\n" + " ---------------------------" + "\n";
            }
            return finalBoard;
        }
        public void play()
        {
            Console.WriteLine("In order to make a move, enter the current location and desired location (for example, a2a3). To offer a draw, type 'draw'.\n");
            ChessPiece location = new EmptyPiece(-1, -1, true), destination = new EmptyPiece(-1, -1, true);
            int moveStatus;
            bool color = true;
            bool drawOffered = false;

            Console.WriteLine(this);
            while (true)
            {
                while (true)
                {
                    drawOffered = userInput(location, destination, color);
                    if (drawOffered)
                    {
                        Console.WriteLine("Draw offered. Do you accept? (yes/no)");
                        string response = Console.ReadLine();
                        if (response.ToLower() == "yes")
                        {
                            Console.WriteLine("****** The game ended in a draw *******");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Draw offer declined.");
                            drawOffered = false;
                            continue;
                        }
                    }

                    moveStatus = move(location.getY(), location.getX(), destination.getY(), destination.getX(), false);
                    if (moveStatus > 0)
                        break;
                    Console.WriteLine("Illegal move");
                    if (moveStatus == -2)
                        Console.WriteLine("Prohibited move: the king is under a check.");
                }

                Console.WriteLine(this);
                if (moveStatus == 10)
                    Console.WriteLine("****** Checkmate! The " + (color ? "white" : "black") + " player won the game *******");
                if (moveStatus == 9)
                    Console.WriteLine("********* The " + (!color ? "white" : "black") + " king is under a check **********");
                if (moveStatus == 3)
                    Console.WriteLine("****** The game ended in a draw *******");
                if (moveStatus == 10 || moveStatus == 3)
                    break;
                color = !color;
            }
            Console.ReadLine();
        }


        public int move(int locationY, int locationX, int destinationY, int destinationX, bool checkMoveAvailable)
        {
            if (!board[locationY, locationX].isLegalMove(this, destinationY, destinationX))
                return -1;

            // Save the original values in case it's required to revert
            bool destinationIsNull = board[destinationY, destinationX] == null;
            ChessPiece copyPiece = board[locationY, locationX].copy();
            ChessPiece copyDestination = null;
            if (board[destinationY, destinationX] != null)
                copyDestination = board[destinationY, destinationX].copy();

            executeMove(board[locationY, locationX], copyPiece, destinationY, destinationX, checkMoveAvailable);

            // Check for mate conditions
            King enemyKing = copyPiece.getColor() ? blackKing : whiteKing;
            bool enemyKingInThreat = !checkMoveAvailable && isKingInThreat(enemyKing.getY(), enemyKing.getX(), !copyPiece.getColor());
            if (enemyKingInThreat && !movesAvailable(!copyPiece.getColor()))
                return 10;

            // Check if the king is in check or if the function was called just to validate the move
            King king = copyPiece.getColor() ? whiteKing : blackKing;
            bool kingIsInCheck = isKingInThreat(king.getY(), king.getX(), copyPiece.getColor());
            if (kingIsInCheck)
            {
                undoMove(copyPiece, copyDestination, destinationY, destinationX, destinationIsNull);
                return -2;
            }
            if (checkMoveAvailable)
            {
                undoMove(copyPiece, copyDestination, destinationY, destinationX, destinationIsNull);
                return 1;
            }

            updateBoardStatus(copyPiece, destinationY, destinationX, destinationIsNull);

            // Handle draw cases
            if (!movesAvailable(!copyPiece.getColor()) || (!drawNoEnoughTools()))
                return 3;
            if ((!(copyPiece is Pawn)) && destinationIsNull)
            {
                fiftyMovesIndex++;
                if (isThreefoldRepetition(copyPiece) || fiftyMovesIndex == 49)
                    return 3;
            }
            else
            {
                // Reset the draw counter if a pawn moves or a piece is captured
                fiftyMovesIndex = 0;
            }

            numOfMoves++;
            if (enemyKingInThreat)
                return 9;

            return 1;
        }


        public void executeMove(ChessPiece piece, ChessPiece copyPiece, int destinationY, int destinationX, bool checkMoveAvailable)
        {
            // moves the tool to the desired location.
            bool destinationIsNull = board[destinationY, destinationX] == null;
            board[destinationY, destinationX] = piece;
            board[piece.getY(), piece.getX()] = null;
            board[destinationY, destinationX].setY(destinationY);
            board[destinationY, destinationX].setX(destinationX);

            if (copyPiece is King && copyPiece.getColor())
                whiteKing = (King)(board[destinationY, destinationX]);
            if (copyPiece is King && !copyPiece.getColor())
                blackKing = (King)(board[destinationY, destinationX]);
            if (copyPiece is Pawn && (destinationY == 0 || destinationY == 7) && !checkMoveAvailable)
                promotion(destinationY, destinationX, copyPiece.getColor());

            // in case of en passant  move takes the opponent's soldier off the board
            if (copyPiece is Pawn && copyPiece.getX() != destinationX && destinationIsNull)
                board[copyPiece.getY(), destinationX] = null;

            // moves the tool to the desired location ​​in case of castling.
            if (copyPiece is King && Math.Abs(copyPiece.getX() - destinationX) > 1)
            {
                if (destinationX == 6)
                {
                    board[copyPiece.getY(), 5] = board[copyPiece.getY(), 7];
                    board[copyPiece.getY(), 5].setX(5);
                    board[copyPiece.getY(), 7] = null;
                }
                else
                {
                    board[copyPiece.getY(), 3] = board[copyPiece.getY(), 0];
                    board[copyPiece.getY(), 3].setX(3);
                    board[copyPiece.getY(), 0] = null;
                }
            }
        }

        public void undoMove(ChessPiece copyPiece, ChessPiece copyDestination, int destinationY, int destinationX, bool destinationIsNull)
        {
            if (copyPiece is Pawn && copyPiece.getX() != destinationX && destinationIsNull)
                board[copyPiece.getY(), destinationX] = new Pawn(copyPiece.getY(), destinationX, !copyPiece.getColor());

            if (copyPiece is King && Math.Abs(copyPiece.getX() - destinationX) > 1)
            {
                if (destinationX == 6)
                {
                    board[copyPiece.getY(), 7] = new Rook(copyPiece.getY(), 7, copyPiece.getColor());
                    board[copyPiece.getY(), 5] = null;
                }
                else
                {
                    board[copyPiece.getY(), 0] = new Rook(copyPiece.getY(), 0, copyPiece.getColor());
                    board[copyPiece.getY(), 3] = null;
                }

            }
            board[destinationY, destinationX] = copyDestination;
            board[copyPiece.getY(), copyPiece.getX()] = copyPiece;
            if (copyPiece is King && copyPiece.getColor())
            {
                whiteKing.setY(copyPiece.getY());
                whiteKing.setX(copyPiece.getX());
            }
            if (copyPiece is King && !copyPiece.getColor())
            {
                blackKing.setY(copyPiece.getY());
                blackKing.setX(copyPiece.getX());
            }
        }

        public void promotion(int destinationY, int destinationX, bool color)
        {
            Console.WriteLine("enter the first letter of the tool you want to promote according to the following list \n Queen:   Q  \n Rook:    R \n Bishop:  B \n Knight:  K");
            string input = Console.ReadLine();
            bool flag = true;
            while (flag)
            {
                flag = false;
                switch (input)
                {
                    case "Q":
                    case "q":
                        board[destinationY, destinationX] = new Queen(destinationY, destinationX, color);
                        break;
                    case "R":
                    case "r":
                        board[destinationY, destinationX] = new Rook(destinationY, destinationX, color);
                        break;
                    case "B":
                    case "b":
                        board[destinationY, destinationX] = new Bishop(destinationY, destinationX, color);
                        break;
                    case "K":
                    case "k":
                        board[destinationY, destinationX] = new Knight(destinationY, destinationX, color);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }
        }

        public bool isKingInThreat(int y, int x, bool color)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != null && board[i, j].getColor() != color && !(board[i, j] is King))
                    {
                        if (board[i, j].isLegalMove(this, y, x))
                            return true;
                    }
                }
            }
            return false;
        }

        public bool movesAvailable(bool color)
        {
            for (int y1 = 0; y1 < 8; y1++)
            {
                for (int x1 = 0; x1 < 8; x1++)
                {
                    if (board[y1, x1] != null && board[y1, x1].getColor() == color)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                if (board[y2, x2] == null || board[y2, x2].getColor() == !color)
                                {
                                    if (move(y1, x1, y2, x2, true) > 0)
                                        return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool isThreefoldRepetition(ChessPiece copyPiece)
        {
            string colorAndStatusOfKingsAndRooks = "" + (copyPiece.getColor() ? 't' : 'f') + (whiteKingMoved ? 't' : 'f') + (whiteRightRookMoved ? 't' : 'f') +
                    (whiteLeftRookMoved ? 't' : 'f') + (blackKingMoved ? 't' : 'f') + (blackRightRookMoved ? 't' : 'f') + (blackLeftRookMoved ? 't' : 'f');

            boardsHistory += this.ToString() + colorAndStatusOfKingsAndRooks;
            int length = this.ToString().Length + 7;
            for (int i = 0, j = 0; i < boardsHistory.Length; i += length, j++)
            {
                string temp = boardsHistory.Substring(i, length);
                if (temp.Equals(this.ToString() + colorAndStatusOfKingsAndRooks))
                {
                    timesBoardAppeared[j]++;
                    if (timesBoardAppeared[j] == 3)
                        return true;
                    break;
                }
            }
            return false;
        }

        public void updateBoardStatus(ChessPiece copyPiece, int destinationY, int destinationX, bool destinationIsNull)
        {
            if (copyPiece is King)
            {
                if (copyPiece.getColor())
                    whiteKingMoved = true;
                if (!copyPiece.getColor())
                    blackKingMoved = true;
            }

            if (copyPiece is Rook && copyPiece.getColor())
            {
                if (copyPiece.getX() == 0)
                    whiteLeftRookMoved = true;
                else if (copyPiece.getX() == 7)
                    whiteRightRookMoved = (true);
            }
            if (copyPiece is Rook && !copyPiece.getColor())
            {
                if (copyPiece.getX() == 0)
                    blackLeftRookMoved = true;
                else if (copyPiece.getX() == 7)
                    blackRightRookMoved = true;
            }

            if (copyPiece is Pawn || !destinationIsNull)
            {
                boardsHistory = "";
                fiftyMovesIndex = 0;
                timesBoardAppeared = new int[50];
            }
            if (copyPiece is Pawn && Math.Abs(copyPiece.getY() - destinationY) > 1)
                pawnMovestwostep = "" + destinationX + copyPiece.getColor() + numOfMoves;
        }

        public bool drawNoEnoughTools()
        {
            int whiteTools = 0;
            int blackTools = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] is Queen || board[i, j] is Pawn || board[i, j] is Rook)
                        return true;
                    if (board[i, j] is Knight || board[i, j] is Bishop)
                    {
                        if (board[i, j].getColor())
                            whiteTools++;
                        else
                            blackTools++;
                    }
                    if (whiteTools > 1 || blackTools > 1)
                        break;
                }
            }
            return (whiteTools > 1 || blackTools > 1);
        }

        public bool userInput(ChessPiece location, ChessPiece destination, bool color)
        {
            int locationY, locationX, destinationY, destinationX;
            while (true)
            {
                Console.WriteLine("Enter current location and desired location, or type 'draw' to offer a draw:");
                string st = Console.ReadLine();
                if (st.ToLower() == "draw")
                {
                    // Mark the location and destination to indicate a draw offer
                    location.setY(-1);
                    location.setX(-1);
                    destination.setY(-1);
                    destination.setX(-1);
                    return true; // Draw offered
                }

                if (st.Length != 4)
                {
                    Console.WriteLine("4 characters are required.");
                    continue;
                }

                locationX = convertUserInputToNumbers(st[0]);
                destinationX = convertUserInputToNumbers(st[2]);

                if ((locationX == 9) || (destinationX == 9) || (st[1] < '1' || st[1] > '8') || (st[3] < '1' || st[3] > '8'))
                {
                    Console.WriteLine("Incorrect input!");
                    continue;
                }

                locationY = 8 - int.Parse(st[1].ToString());
                destinationY = 8 - int.Parse(st[3].ToString());

                if (board[locationY, locationX] == null || board[locationY, locationX].getColor() != color)
                {
                    Console.WriteLine("You can only move with the " + (color ? "white" : "black") + " pieces.");
                    continue;
                }
                if (board[destinationY, destinationX] != null && board[destinationY, destinationX].getColor() == color)
                {
                    Console.WriteLine("Prohibited move: the position is occupied by another piece of the same color.");
                    continue;
                }
                break;
            }
            location.setY(locationY);
            location.setX(locationX);
            destination.setY(destinationY);
            destination.setX(destinationX);
            return false; // No draw offered
        }
        public int convertUserInputToNumbers(char ch)
        {
            switch (ch)
            {
                case 'a':
                case 'A':
                    return 0;
                case 'b':
                case 'B':
                    return 1;
                case 'c':
                case 'C':
                    return 2;
                case 'd':
                case 'D':
                    return 3;
                case 'e':
                case 'E':
                    return 4;
                case 'f':
                case 'F':
                    return 5;
                case 'g':
                case 'G':
                    return 6;
                case 'h':
                case 'H':
                    return 7;
                default:
                    return 9;
            }
        }

    }

    class ChessPiece
    {
        int x;
        int y;
        bool color;

        public ChessPiece(int y, int x, bool color)
        {
            this.y = y;
            this.x = x;
            this.color = color;
        }

        public virtual ChessPiece copy()
        {
            return new ChessPiece(this.getY(), this.getX(), this.getColor());
        }

        public int getX()
        {
            return x;
        }
        public int getY()
        {
            return y;
        }
        public bool getColor()
        {
            return color;
        }
        public void setX(int x)
        {
            this.x = x;
        }
        public void setY(int y)
        {
            this.y = y;
        }

        public virtual bool isLegalMove(ChessGame game, int y, int x)
        {
            return true;
        }
    }

    class King : ChessPiece
    {
        public King(int y, int x, bool color)
            : base(y, x, color)
        {
        }

        public override ChessPiece copy()
        {
            return new King(this.getY(), this.getX(), this.getColor());
        }

        public override bool isLegalMove(ChessGame game, int y, int x)
        {
            int x1 = Math.Abs(this.getX() - x);
            int y1 = Math.Abs(this.getY() - y);
            if ((y1 == 1 && x1 == 0) || (y1 == 0 && x1 == 1) || (x1 == 1 && y1 == 1))
                return true;

            // castling
            if (game.isKingInThreat(this.getY(), this.getX(), this.getColor()))
                return false;
            if (this.getColor() == true && (!game.getWhiteKingMoved()))
            {
                if ((y == 7 && x == 2) && (!game.getWhiteLeftRookMoved()) && game.getBoard()[7, 3] == null && game.getBoard()[7, 1] == null && !game.isKingInThreat(7, 3, this.getColor()))
                    return true;
                if ((y == 7 && x == 6) && (!game.getWhiteRightRookMoved()) && game.getBoard()[7, 5] == null && !game.isKingInThreat(7, 5, this.getColor()))
                    return true;
            }
            else if (this.getColor() == false && (!game.getBlackKingMoved()))
            {
                if ((y == 0 && x == 2) && (!game.getBlackLeftRookMoved()) && game.getBoard()[0, 3] == null && game.getBoard()[0, 1] == null && !game.isKingInThreat(0, 3, this.getColor()))
                    return true;
                if ((y == 0 && x == 6) && (!game.getBlackRightRookMoved()) && game.getBoard()[0, 5] == null && !game.isKingInThreat(0, 5, this.getColor()))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return (this.getColor() ? " W" : " B") + "K ";
        }
    }

    class Queen : ChessPiece
    {
        public Queen(int y, int x, bool color)
            : base(y, x, color)
        {
        }

        public override ChessPiece copy()
        {
            return new Queen(this.getY(), this.getX(), this.getColor());
        }

        public override bool isLegalMove(ChessGame game, int y, int x)
        {

            Bishop queenAsBishop = new Bishop(this.getY(), this.getX(), this.getColor());
            Rook queenAsRook = new Rook(this.getY(), this.getX(), this.getColor());

            return queenAsBishop.isLegalMove(game, y, x) || queenAsRook.isLegalMove(game, y, x);
        }

        public override string ToString()
        {
            return (this.getColor() ? " W" : " B") + "Q ";
        }
    }

    class Rook : ChessPiece
    {
        public Rook(int y, int x, bool color)
            : base(y, x, color)
        {
        }

        public override ChessPiece copy()
        {
            return new Rook(this.getY(), this.getX(), this.getColor());
        }

        public override bool isLegalMove(ChessGame game, int y, int x)
        {
            if (this.getX() == x && this.getY() - y > 0)
            {
                for (int i = this.getY() - 1; i >= 0 && i > y; i--)
                {
                    if (game.getBoard()[i, x] != null)
                        return false;
                }
            }
            else if (this.getX() == x && this.getY() - y < 0)
            {
                for (int i = this.getY() + 1; i <= 7 && i < y; i++)
                {
                    if (game.getBoard()[i, x] != null)
                        return false;
                }
            }
            else if (this.getY() == y && this.getX() - x > 0)
            {
                for (int i = this.getX() - 1; i >= 0 && i > x; i--)
                {
                    if (game.getBoard()[y, i] != null)
                        return false;
                }
            }
            else if (this.getY() == y && this.getX() - x < 0)
            {
                for (int i = this.getX() + 1; i <= 7 && i < x; i++)
                {
                    if (game.getBoard()[y, i] != null)
                        return false;
                }
            }
            if (this.getY() == y || this.getX() == x)
                return true;
            return false;
        }

        public override string ToString()
        {
            return (this.getColor() ? " W" : " B") + "R ";
        }
    }

    class Knight : ChessPiece
    {
        public Knight(int y, int x, bool color)
                   : base(y, x, color)
        {
        }

        public override ChessPiece copy()
        {
            return new Knight(this.getY(), this.getX(), this.getColor());
        }

        public override bool isLegalMove(ChessGame game, int y, int x)
        {
            int x1 = Math.Abs(this.getX() - x);
            int y1 = Math.Abs(this.getY() - y);
            return x1 * y1 == 2;
        }

        public override string ToString()
        {
            return (this.getColor() ? " W" : " B") + "N ";
        }
    }

    class Bishop : ChessPiece
    {
        public Bishop(int y, int x, bool color)
                   : base(y, x, color)
        {
        }

        public override ChessPiece copy()
        {
            return new Bishop(this.getY(), this.getX(), this.getColor());
        }

        public override bool isLegalMove(ChessGame game, int y, int x)
        {
            int x1 = Math.Abs(this.getX() - x);
            int y1 = Math.Abs(this.getY() - y);
            if (!(x1 == y1))
                return false;

            if (this.getY() - y > 0 && this.getX() - x > 0)
            {
                while (x < 7 && y < 7 && x != this.getX() - 1)
                {
                    if (game.getBoard()[++y, ++x] != null)
                        return false;
                }
            }
            else if (this.getY() - y < 0 && this.getX() - x < 0)
            {
                while (x > 0 && y > 0 && x != this.getX() + 1)
                {
                    if (game.getBoard()[--y, --x] != null)
                        return false;
                }
            }
            else if (this.getY() - y < 0 && this.getX() - x > 0)
            {
                while (x < 7 && y > 0 && x != this.getX() - 1)
                {
                    if (game.getBoard()[--y, ++x] != null)
                        return false;
                }
            }
            else if (this.getY() - y > 0 && this.getX() - x < 0)
            {
                while (x > 0 && y < 7 && x != this.getX() + 1)
                {
                    if (game.getBoard()[++y, --x] != null)
                        return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return (this.getColor() ? " W" : " B") + "B ";
        }
    }

    class Pawn : ChessPiece
    {
        public Pawn(int y, int x, bool color)
             : base(y, x, color)
        {
        }

        public override ChessPiece copy()
        {
            return new Pawn(this.getY(), this.getX(), this.getColor());
        }

        public override bool isLegalMove(ChessGame game, int y, int x)
        {
            //checks a case where the soldier advances one step.
            if ((this.getX() - x == 0 && game.getBoard()[y, x] == null) && ((this.getColor() && this.getY() - y == 1) || (!this.getColor() && this.getY() - y == -1)))
                return true;
            //checks a case in which a soldier kills an enemy piece.
            if ((Math.Abs(this.getX() - x) == 1 && game.getBoard()[y, x] != null) && ((this.getColor() && this.getY() - y == 1) || (!this.getColor() && this.getY() - y == -1)))
                return true;
            //checks a case where a soldier can advance two steps.
            if ((this.getY() == 6 && this.getColor()) || (this.getY() == 1 && !this.getColor()))
            {
                if (this.getX() - x == 0 && ((this.getColor() && y == 4) || (!this.getColor() && y == 3)))
                    return true;
            }

            // en passant move.
            if ((game.getBoard()[y, x] == null) && (Math.Abs(this.getX() - x) == 1) && ((this.getY() == 3 && this.getColor() && y == 2) || (this.getY() == 4 && !this.getColor() && y == 5)))
            {
                string currentBoard = "" + x + (!this.getColor()) + (game.getNumOfMoves() - 1);
                if (currentBoard.Equals(game.getPawnMovestwostep()))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return (this.getColor() ? " W" : " B") + "P ";
        }
    }

    class EmptyPiece : ChessPiece
    {
        public EmptyPiece(int y, int x, bool color)
                   : base(y, x, color)
        {
        }

        public override string ToString()
        {
            return " EE ";
        }
    }
}