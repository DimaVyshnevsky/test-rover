using UnityEngine;

namespace Core.UI
{
    public sealed class UiServiceViewContainer : MonoBehaviour
    {
        [SerializeField] private Transform _screenParent;

        public Transform ScreenParent => _screenParent;
    }
}