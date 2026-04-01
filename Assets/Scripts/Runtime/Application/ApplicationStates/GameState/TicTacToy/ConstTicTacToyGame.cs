namespace Application.Game.TicTacToy
{
    public class ConstTicTacToyGame
    {
        public const string CrossWinKey = "Player 1 wins";
        public const string CircleWinKey = "Player 2 wins";
        public const string Draw = "Draw";
        
        public static readonly int[][] WinPatterns =
        {
            new []{0,1,2},
            new []{3,4,5},
            new []{6,7,8},
            new []{0,3,6},
            new []{1,4,7},
            new []{2,5,8},
            new []{0,4,8},
            new []{2,4,6},
        };
    }
}