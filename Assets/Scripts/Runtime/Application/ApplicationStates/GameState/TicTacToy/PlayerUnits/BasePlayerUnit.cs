using System.Threading;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public abstract class BasePlayerUnit
    {
        protected SlotType PlayerId;

        public SlotType Id => PlayerId;

        protected BasePlayerUnit(SlotType playerId)
        {
            PlayerId = playerId;
        }

        public abstract UniTask<Slot> NextMove(CancellationToken cancellationToken);
    }
}