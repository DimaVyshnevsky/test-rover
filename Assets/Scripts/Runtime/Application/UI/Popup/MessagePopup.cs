using System.Threading;
using Core.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class MessagePopup : BasePopup
    {
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private TextMeshProUGUI _message;

        public override async UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            MessagePopupData messagePopupData = data as MessagePopupData;

            _message.text = messagePopupData.Message;

            if (messagePopupData.ShowButton)
            {
                _okButton.gameObject.SetActive(true);
                _okButton.onClick.AddListener(DestroyPopup);
                _backgroundButton.gameObject.SetActive(false);
            }
            else
            {
                _okButton.gameObject.SetActive(false);
                _backgroundButton.onClick.AddListener(DestroyPopup);
            }

            await base.Show(data, cancellationToken);
        }
    }
}