using System.Threading;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public class RealPlayerUnit : BasePlayerUnit
    {
        private bool _readInput;
        private int _slotIndex = -1;

        public RealPlayerUnit(SlotType slotType) : base(slotType)
        {
        }

        public override async UniTask<Slot> NextMove(CancellationToken cancellationToken)
        {
            _readInput = true;
            await UniTask.WaitWhile(IsSlotNotPressed, cancellationToken: cancellationToken);
            var slotIndexToSend = _slotIndex;
            _readInput = false;
            _slotIndex = -1;
            return new Slot(slotIndexToSend, PlayerId);
        }

        private bool IsSlotNotPressed()
        {
            return _slotIndex == -1;
        }

        public void OnSlotPress(int slotIndex)
        {
            if(!_readInput)
                return;

            _slotIndex = slotIndex;
        }
    }
}