using Application.Services.UserData;
using Application.UI;
using Core;
using Core.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public class BotBotTicTacToyStateController : BaseTicTacToyStateController
    {
        private readonly BotFactory _botFactory;

        public BotBotTicTacToyStateController(ILogger logger,
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
            BasePlayerUnit crossPlayer = _botFactory.Create(SlotType.Cross);
            BasePlayerUnit circlePlayer = _botFactory.Create(SlotType.Circle);

            var players = new BasePlayerUnit[]
            {
                crossPlayer,
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
            GoTo<BotBotTicTacToyStateController>().Forget();
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