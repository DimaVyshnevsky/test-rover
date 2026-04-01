using System.Threading;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class NoInternetConnectionPopup : BasePopup
    {
        [SerializeField] private Button _okButton;

        public override async UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _okButton.onClick.AddListener(DestroyPopup);

            await Show(data, cancellationToken);
        }
    }
}