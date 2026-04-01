using System;

namespace Application.Game.TicTacToy
{
    public class BoardModel
    {
        private SlotType[] _board = new SlotType[9];
        private GameResultType _gameResult;
        private SlotType _activePlayerId;
        private int[] _winPattern;

        public event Action<Slot> BoardUpdateEvent;

        public GameResultType GameResult => _gameResult;

        public void UpdateSlot(int index, SlotType slotType, bool isNotify = true)
        {
            if(index < 0 || index > 8)
                return;

            _board[index] = slotType;

            if(isNotify)
                BoardUpdateEvent?.Invoke(new Slot(index, slotType));
        }

        public SlotType GetSlotType(int index)
        {
            return _board[index];
        }

        public void ResetBoard()
        {
            _activePlayerId = SlotType.None;
            _winPattern = null;
            _gameResult = GameResultType.None;

            for (int i = 0; i < _board.Length; i++)
                _board[i] = SlotType.None;
        }

        public void SetGameResult(GameResultType gameResult)
        {
            _gameResult = gameResult;
        }

        public int[] GetWinPattern()
        {
            return _winPattern;
        }
        
        public void SetWinPattern(int[] winPattern)
        {
            _winPattern = winPattern;
        }
        
        public SlotType GetActivePlayer()
        {
            return _activePlayerId;
        }
        
        public void SetActivePlayer(SlotType activePlayerId)
        {
            _activePlayerId = activePlayerId;
        }
    }

    public struct Slot
    {
        private int _index;
        private SlotType _type;

        public int Index => _index;
        public SlotType Type => _type;

        public Slot(int index, SlotType type)
        {
            _index = index;
            _type = type;
        }
    }

    public enum SlotType
    {
        None,
        Cross,
        Circle
    }
    
    public enum GameResultType
    {
        None,
        Draw,
        Cross,
        Circle
    }
}