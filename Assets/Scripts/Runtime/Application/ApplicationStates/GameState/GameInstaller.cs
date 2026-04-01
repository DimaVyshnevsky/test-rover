using Application.Game.Menu;
using Application.Game.TicTacToy;
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

            Container.Bind<MenuStateController>().AsSingle();
            Container.Bind<StartSettingsController>().AsSingle();
            Container.Bind<UserDataStateChangeController>().AsSingle();
            Container.Bind<BotBotTicTacToyStateController>().AsSingle();
            Container.Bind<PlayerBotTicTacToyStateController>().AsSingle();
            Container.Bind<PlayerPlayerTicTacToyStateController>().AsSingle();
            Container.Bind<TicTacToyGameController>().AsSingle();
            Container.Bind<BoardModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<TimerController>().AsSingle();
            Container.Bind<HintController>().AsSingle();
            Container.Bind<BotFactory>().AsSingle();
        }
    }
}