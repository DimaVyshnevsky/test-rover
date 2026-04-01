using Application.Services.UserData;
using Application.UI;
using Core;
using Core.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public class PlayerBotTicTacToyStateController : BaseTicTacToyStateController
    {
        private readonly BotFactory _botFactory;

        private RealPlayerUnit _crossPlayer;

        public PlayerBotTicTacToyStateController(ILogger logger,
            TicTacToyGameController ticTacToyGameController,
            IUiService uiService,
            BoardModel boardModel,
            UserDataProvider userDataProvider,
            IAudioService audioService,
            TimerController timerController,
            BotFactory botFactory) : base(logger, ticTacToyGameController, uiService, boardModel,
            userDataProvider, audioService, timerController)
        {
            _botFactory = botFactory;
        }

        protected override TicTacToyGameRequest PlayerInitialization()
        {
            _crossPlayer = new RealPlayerUnit(SlotType.Cross);
            _screen.SlotPressEvent += _crossPlayer.OnSlotPress;

            BasePlayerUnit circlePlayer = _botFactory.Create(SlotType.Circle);

            var players = new BasePlayerUnit[]
            {
                _crossPlayer,
                circlePlayer
            };

            var request = new TicTacToyGameRequest
            {
                Players = players
            };

            return request;
        }

        protected override void RestartGame()
        {
            GoTo<PlayerBotTicTacToyStateController>().Forget();
        }

        protected override TicTacToyScreen CreateScreen()
        {
            var screen = base.CreateScreen();
            screen.EnableHintButton(false);
            screen.EnableUndoButton(false);
            return screen;
        }
    }
}