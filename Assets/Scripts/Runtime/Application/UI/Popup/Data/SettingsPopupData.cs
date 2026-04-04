using Core.UI;

namespace Application.UI
{
    public class SettingsPopupData : BasePopupData
    {
        private bool _isSoundVolume;
        private bool _isMusicVolume;
        private bool _isJoystickEnable;

        public bool IsSoundVolume => _isSoundVolume;
        public bool IsMusicVolume => _isMusicVolume;
        public bool IsJoystickEnable => _isJoystickEnable;

        public SettingsPopupData(bool isSoundVolume, bool isMusicVolume, bool isJoystickEnable)
        {
            _isSoundVolume = isSoundVolume;
            _isMusicVolume = isMusicVolume;
            _isJoystickEnable = isJoystickEnable;
        }
    }
}