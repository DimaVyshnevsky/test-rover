using Application.GameState.Menu;
using Application.GameState.RoverSimulation;
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

            BindRoverSimulation();
        }

        private void BindRoverSimulation()
        {
            Container.Bind<RoverSimulationState>().AsSingle();
            Container.Bind<RoverInputModel>().AsSingle();

            Container.Bind<KeyboardRoverInputController>()
                .AsSingle();

            Container.Bind<BaseRoverInputController>()
                .To<KeyboardRoverInputController>()
                .FromResolve();

            Container.Bind<ITickable>()
                .To<KeyboardRoverInputController>()
                .FromResolve();
        }
    }
}