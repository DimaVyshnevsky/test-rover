using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public class TicTacToyGameController : BaseController<TicTacToyGameRequest>
    {
        private readonly BoardModel _boardModel;
        private readonly ILogger _logger;

        private CancellationTokenSource _undoCancellationTokenSource;
        private CancellationTokenSource _linkedTokenSource;

        public TicTacToyGameController(BoardModel boardModel, ILogger logger)
        {
            _boardModel = boardModel;
            _logger = logger;
        }

        public override async UniTask Run(TicTacToyGameRequest request, CancellationToken cancellationToken)
        {
            base.Run(request, cancellationToken);

            _boardModel.BoardUpdateEvent += CheckUndoMove;
            _undoCancellationTokenSource = new CancellationTokenSource();
            _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _undoCancellationTokenSource.Token);

            int playerIndex = 0;
            BasePlayerUnit currentPlayer = request.Players[playerIndex];
            _boardModel.SetActivePlayer(currentPlayer.Id);

            while (CurrentState == ControllerState.Run)
            {
                Slot slot = default;
                try
                {
                    slot = await currentPlayer.NextMove(_linkedTokenSource.Token);
                }
                catch (Exception e)
                {
                    if (_undoCancellationTokenSource.IsCancellationRequested)
                    {
                        _undoCancellationTokenSource = new CancellationTokenSource();
                        _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _undoCancellationTokenSource.Token);

                        playerIndex++;
                        if (playerIndex >= request.Players.Length)
                            playerIndex = 0;
                        currentPlayer = request.Players[playerIndex];
                        _boardModel.SetActivePlayer(currentPlayer.Id);

                        continue;
                    }

                    if (cancellationToken.IsCancellationRequested)
                        break;

                    _logger.Error(e.Message);
                    CurrentState = ControllerState.Failed;
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                    break;

                if (CurrentState != ControllerState.Run)
                    return;

                var isValidMove = IsValidMove(slot);
                if (!isValidMove)
                    continue;

                _boardModel.UpdateSlot(slot.Index, slot.Type);
                var gameResult = GetResult();
                if (gameResult != GameResultType.None)
                {
                    _boardModel.SetGameResult(gameResult);
                    break;
                }

                playerIndex++;
                if (playerIndex >= request.Players.Length)
                    playerIndex = 0;

                currentPlayer = request.Players[playerIndex];
                _boardModel.SetActivePlayer(currentPlayer.Id);
            }

            _boardModel.BoardUpdateEvent -= CheckUndoMove;
            CurrentState = ControllerState.Complete;
        }

        public override UniTask Stop()
        {
            base.Stop();
            _boardModel.BoardUpdateEvent -= CheckUndoMove;
            return UniTask.CompletedTask;
        }

        private bool IsValidMove(Slot slot)
        {
            var slotType = _boardModel.GetSlotType(slot.Index);
            return slotType == SlotType.None;
        }

        private GameResultType GetResult()
        {
            foreach (var pattern in ConstTicTacToyGame.WinPatterns)
            {
                int crossMatches = 0;
                int circleMatches = 0;
                foreach (var winIndex in pattern)
                {
                    var slotType = _boardModel.GetSlotType(winIndex);

                    switch (slotType)
                    {
                        case SlotType.Circle:
                            circleMatches++;
                            break;
                        case SlotType.Cross:
                            crossMatches++;
                            break;
                    }
                }

                var gameResult = GameResultType.None;
                
                if (crossMatches >= 3)
                    gameResult = GameResultType.Cross;
                else if (circleMatches >= 3)
                    gameResult = GameResultType.Circle;

                if (gameResult != GameResultType.None)
                {
                    _boardModel.SetWinPattern(pattern);
                    return gameResult;
                }
            }

            bool isNoPossibleSteps = IsNoPossibleSteps();

            return isNoPossibleSteps ? GameResultType.Draw : GameResultType.None;
        }

        private bool IsNoPossibleSteps()
        {
            bool isNoPossibleSteps = true;

            for (int i = 0; i < 9; i++)
            {
                if (_boardModel.GetSlotType(i) != SlotType.None) 
                    continue;

                isNoPossibleSteps = false;
                break;
            }

            return isNoPossibleSteps;
        }

        private void CheckUndoMove(Slot slot)
        {
            if (slot.Type != SlotType.None)
                return;

            _undoCancellationTokenSource?.Cancel();
            _undoCancellationTokenSource?.Dispose();
        }
    }
}