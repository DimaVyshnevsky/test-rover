using Application.Services.UserData;
using Core;

namespace Application.Game.TicTacToy
{
    public class BotFactory
    {
        private readonly ISettingProvider _settingProvider;
        private readonly UserDataProvider _userDataProvider;
        private readonly BoardModel _boardModel;

        public BotFactory(ISettingProvider settingProvider, UserDataProvider userDataProvider, BoardModel boardModel)
        {
            _settingProvider = settingProvider;
            _userDataProvider = userDataProvider;
            _boardModel = boardModel;
        }

        public BasePlayerUnit Create(SlotType slotType)
        {
            var gameConfig = _settingProvider.Get<GameConfig>();
            var settingsData = _userDataProvider.GetUserData().SettingsData;

            BasePlayerUnit bot = settingsData.GameDifficultyMode == GameDifficultyMode.Easy
                ? new DummyBotPlayerUnit(slotType, _boardModel, gameConfig)
                : new AdvancedBotPlayerUnit(slotType, _boardModel, gameConfig);

            return bot;
        }
    }
}