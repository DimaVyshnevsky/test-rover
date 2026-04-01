using Application.Services.UserData;
using Core.UI;

namespace Application.UI
{
    public class SettingsPopupData : BasePopupData
    {
        private bool _isSoundVolume;
        private bool _isMusicVolume;
        private GameDifficultyMode _gameDifficultyMode;

        public bool IsSoundVolume => _isSoundVolume;
        public bool IsMusicVolume => _isMusicVolume;
        public GameDifficultyMode GameDifficultyMode => _gameDifficultyMode;

        public SettingsPopupData(bool isSoundVolume, bool isMusicVolume, GameDifficultyMode gameDifficultyMode)
        {
            _isSoundVolume = isSoundVolume;
            _isMusicVolume = isMusicVolume;
            _gameDifficultyMode = gameDifficultyMode;
        }
    }
}