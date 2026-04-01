using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ISettingProvider
    {
        UniTask Initialize();
        void Dispose();
        T Get<T>() where T : BaseSettings;
    }
}