using Application.GameState.Menu;
using UnityEngine;
using Zenject;

namespace Application.GameState
{
    [CreateAssetMenu(fileName = "GameInstaller", menuName = "Installers/GameInstaller")]
    public class GameInstaller : ScriptableObjectInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameState>().AsSingle();

            Container.Bind<MenuState>().AsSingle();
            Container.Bind<StartSettingsController>().AsSingle();
            Container.Bind<UserDataStateChangeController>().AsSingle();

            Container.Bind<LevelsService>().AsSingle();
        }
    }
}