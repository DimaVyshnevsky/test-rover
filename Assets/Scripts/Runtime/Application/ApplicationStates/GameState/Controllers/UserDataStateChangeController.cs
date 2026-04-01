using System.Threading;
using Application.Services.ApplicationState;
using Application.Services.UserData;
using Core;
using Cysharp.Threading.Tasks;

namespace Application.GameState
{
    public class UserDataStateChangeController : BaseController
    {
        private readonly ApplicationStateService _applicationStateService;
        private readonly UserDataProvider _userDataProvider;

        public UserDataStateChangeController(ApplicationStateService applicationStateService,
            UserDataProvider userDataProvider)
        {
            _applicationStateService = applicationStateService;
            _userDataProvider = userDataProvider;
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);

            _applicationStateService.Initialize();

            _applicationStateService.ApplicationQuitEvent += OnQuitApplicationHandler;
            _applicationStateService.ApplicationPauseEvent += OnPauseApplicationHandler;

            return UniTask.CompletedTask;
        }

        public override UniTask Stop()
        {
            base.Stop();

            _applicationStateService.ApplicationQuitEvent -= OnQuitApplicationHandler;
            _applicationStateService.ApplicationPauseEvent -= OnPauseApplicationHandler;

            _applicationStateService.Dispose();

            return UniTask.CompletedTask;
        }

        private void OnQuitApplicationHandler()
        {
            _userDataProvider.SaveUserData();
        }

        private void OnPauseApplicationHandler(bool isPause)
        {
            if (isPause)
                _userDataProvider.SaveUserData();
        }
    }
}