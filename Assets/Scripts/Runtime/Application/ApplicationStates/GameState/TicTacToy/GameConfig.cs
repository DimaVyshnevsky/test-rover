using Core;
using UnityEngine;

namespace Application.Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig")]
    public class GameConfig : BaseSettings
    {
        public int BotRespondTimeMilliseconds = 700;
    }
}