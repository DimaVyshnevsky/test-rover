using Core.StateMachine;
using UnityEngine;
using Zenject;

namespace Application.BootstrapState
{
    [CreateAssetMenu(fileName = "BootstrapInstaller", menuName = "Installers/BootstrapInstaller")]
    public class BootstrapInstaller : ScriptableObjectInstaller<BootstrapInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();

            Container.Bind<StateMachine>().WithId(BindingConst.ApplicationStateMachine).AsCached();
            Container.Bind<StateMachine>().WithId(BindingConst.GameStateMachine).AsCached();
            Container.BindInterfacesAndSelfTo<Bootstrapper>().AsSingle().NonLazy();

            Container.Bind<AudioSettingsBootstrapController>().AsSingle();
        }
    }
}