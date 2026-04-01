using System.Threading;
using Core;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public class HintController : BaseController<HintControllerRequest>
    {
        private readonly BoardModel _boardModel;
        private readonly ISettingProvider _settingProvider;

        private AdvancedBotPlayerUnit _advancedBotPlayerUnitCross;
        private AdvancedBotPlayerUnit _advancedBotPlayerUnitCircle;

        public HintController(BoardModel boardModel,
            ISettingProvider settingProvider)
        {
            _boardModel = boardModel;
            _settingProvider = settingProvider;
        }

        public override async UniTask Run(HintControllerRequest request, CancellationToken cancellationToken)
        {
            base.Run(request, cancellationToken);

            _advancedBotPlayerUnitCross ??= new AdvancedBotPlayerUnit(SlotType.Cross, _boardModel, _settingProvider.Get<GameConfig>());
            _advancedBotPlayerUnitCircle ??= new AdvancedBotPlayerUnit(SlotType.Circle, _boardModel, _settingProvider.Get<GameConfig>());

            var playerType = _boardModel.GetActivePlayer();
            var buttonIndex = await FindSlot(playerType, cancellationToken);

            request.Screen.HighlightButton(buttonIndex);

            CurrentState = ControllerState.Complete;
        }

        private async UniTask<int> FindSlot(SlotType playerType, CancellationToken cancellationToken)
        {
            var slot = playerType == SlotType.Cross
                ? await _advancedBotPlayerUnitCross.NextMove(cancellationToken)
                : await _advancedBotPlayerUnitCircle.NextMove(cancellationToken);
            return slot.Index;
        }
    }
}