using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public class DummyBotPlayerUnit : BasePlayerUnit
    {
        private BoardModel _boardModel;
        private GameConfig _gameConfig;

        public DummyBotPlayerUnit(SlotType slotType, BoardModel model, GameConfig gameConfig) : base(slotType)
        {
            _boardModel = model;
            _gameConfig = gameConfig;
        }

        public override async UniTask<Slot> NextMove(CancellationToken cancellationToken)
        {
            List<int> freeSlotsIndexes = new List<int>(9);
            for (int i = 0; i < 9; i++)
            {
                var slotType = _boardModel.GetSlotType(i);
                if(slotType == SlotType.None)
                    freeSlotsIndexes.Add(i);
            }

            if (freeSlotsIndexes.Count == 0)
                throw new Exception("something went wrong, no empty slots");

            await UniTask.Delay(_gameConfig.BotRespondTimeMilliseconds, cancellationToken: cancellationToken);
            var index = freeSlotsIndexes[UnityEngine.Random.Range(0, freeSlotsIndexes.Count)];
            return new Slot(index, PlayerId);
        }
    }
}