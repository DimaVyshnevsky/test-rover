using Core;
using Core.Compressor;

namespace Application.Services.UserData
{
    public class UserDataProvider
    {
        private readonly IPersistentDataProvider _persistentDataProvider;
        private readonly BaseCompressor _compressor;
        private readonly ILogger _logger;

        private UserData _userData;

        public UserDataProvider(IPersistentDataProvider persistentDataProvider,
            BaseCompressor compressor,
            ILogger logger)
        {
            _persistentDataProvider = persistentDataProvider;
            _compressor = compressor;
            _logger = logger;
        }

        public void Initialize()
        {
#if DEV
            _userData = _persistentDataProvider.Load<UserData>(ConstDataPath.UserDataPath, ConstDataPath.UserDataFileName) ?? new UserData();
#else
            _userData = _persistentDataProvider.Load<UserData>(ConstDataPath.UserDataPath, ConstDataPath.UserDataFileName,null, _compressor) ?? new UserData();
#endif
        }

        public UserData GetUserData()
        {
            return _userData;
        }

        public void SaveUserData()
        {
            if (_userData == null)
            {
                _logger.Error($"{nameof(UserDataProvider)}: user data not initialized. Need to Initialize() before use.");
                return;
            }

#if DEV
            _persistentDataProvider.Save(_userData, ConstDataPath.UserDataPath, ConstDataPath.UserDataFileName);
#else
            _persistentDataProvider.Save(_userData, ConstDataPath.UserDataPath, ConstDataPath.UserDataFileName, null, _compressor);
#endif
        }
    }
}