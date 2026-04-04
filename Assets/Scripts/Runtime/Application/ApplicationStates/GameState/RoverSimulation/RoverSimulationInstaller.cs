using UnityEngine;
using Zenject;

namespace Application.GameState.RoverSimulation
{
    [CreateAssetMenu(fileName = "RoverSimulationInstaller", menuName = "Installers/RoverSimulationInstaller")]
    public class RoverSimulationInstaller : ScriptableObjectInstaller<RoverSimulationInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<RoverSimulationState>().AsSingle();
            Container.Bind<RoverInputModel>().AsSingle();

            Container.Bind<RoverInputController>().AsSingle();
            Container.BindInterfacesAndSelfTo<KeyboardRoverInputController>().AsSingle();
            Container.BindInterfacesAndSelfTo<JoystickRoverInputController>().AsSingle();
        }
    }
}