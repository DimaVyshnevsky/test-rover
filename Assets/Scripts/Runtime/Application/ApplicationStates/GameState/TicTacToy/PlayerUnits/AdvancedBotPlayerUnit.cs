using System.Threading;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public class AdvancedBotPlayerUnit : BasePlayerUnit
    {
        private BoardModel _board;
        private GameConfig _gameConfig;

        public AdvancedBotPlayerUnit(SlotType slotType, BoardModel model, GameConfig gameConfig) : base(slotType)
        {
            _board = model;
            _gameConfig = gameConfig;
        }

        public override async UniTask<Slot> NextMove(CancellationToken cancellationToken)
        {
            var index = MakeMove(PlayerId);
            await UniTask.Delay(_gameConfig.BotRespondTimeMilliseconds, cancellationToken: cancellationToken);
            return new Slot(index, PlayerId);
        }
        
        private int MakeMove(SlotType player)
        {
            //check if we can win on the next move
            for (int i = 0; i < 9; i++)
            {
                if (_board.GetSlotType(i) == SlotType.None)
                {
                    _board.UpdateSlot(i, player, false);
                    if (CheckWin() == (player == SlotType.Cross ? GameResultType.Cross : GameResultType.Circle))
                    {
                        _board.UpdateSlot(i, SlotType.None, false);
                        return i;
                    }
                    _board.UpdateSlot(i, SlotType.None, false);
                }
            }

            //check if we can block the opponent's winning move
            SlotType opponent = player == SlotType.Cross ? SlotType.Circle : SlotType.Cross;
            for (int i = 0; i < 9; i++)
            {
                if (_board.GetSlotType(i) == SlotType.None)
                {
                    _board.UpdateSlot(i, opponent, false);

                    if (CheckWin() == (opponent == SlotType.Cross ? GameResultType.Cross : GameResultType.Circle))
                    {
                        _board.UpdateSlot(i, SlotType.None, false);
                        return i; 
                    }
                    _board.UpdateSlot(i, SlotType.None, false);
                }
            }

            //try get center
            if (_board.GetSlotType(4) == SlotType.None)
                return 4;

            //if there are no winning or blocking moves, make a random move
            for (int i = 0; i < 9; i++)
            {
                if (_board.GetSlotType(i) == SlotType.None)
                    return i;
            }

            return -1;
        }
        
        private GameResultType CheckWin()
        {
            foreach (var pattern in ConstTicTacToyGame.WinPatterns)
            {
                if (_board.GetSlotType(pattern[0]) != SlotType.None &&
                    _board.GetSlotType(pattern[0]) == _board.GetSlotType(pattern[1]) &&
                    _board.GetSlotType(pattern[1]) == _board.GetSlotType(pattern[2]))
                {
                    return _board.GetSlotType(pattern[0]) == SlotType.Cross ? GameResultType.Cross : GameResultType.Circle;
                }
            }

            return IsNoPossibleSteps() ? GameResultType.Draw : GameResultType.None;
        }
        
        private bool IsNoPossibleSteps()
        {
            bool isNoPossibleSteps = true;

            for (int i = 0; i < 9; i++)
            {
                if (_board.GetSlotType(i) != SlotType.None) 
                    continue;

                isNoPossibleSteps = false;
                break;
            }

            return isNoPossibleSteps;
        }
    }
}