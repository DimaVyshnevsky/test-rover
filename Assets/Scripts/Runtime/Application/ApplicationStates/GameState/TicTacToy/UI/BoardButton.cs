using System;
using System.Threading;
using Application.Game.TicTacToy;
using Application.Services.Audio;
using Core.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Application.UI
{
    public class BoardButton : MonoBehaviour
    {
        [SerializeField] private int _index;
        [SerializeField] private Sprite _noneSprite;
        [SerializeField] private Sprite _crossSprite;
        [SerializeField] private Sprite _cirleSprite;
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;
        [SerializeField] private Animation _winAnimation;
        [SerializeField] private GameObject _highlightButtonAnimation;
        [SerializeField] private int _winAnimationDurationMilliseconds = 2000;

        private IAudioService _audioService;

        public Button Button => _button;

        public int Index => _index;

        [Inject]
        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public void UpdateView(SlotType slotType)
        {
            Sprite sprite = null;
            switch (slotType)
            {
                case SlotType.None:
                    sprite = _noneSprite;
                    break;
                case SlotType.Cross:
                    sprite = _crossSprite;
                    break;
                case SlotType.Circle:
                    sprite = _cirleSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slotType), slotType, "handler not presented for this type");
            }

            _image.sprite = sprite;
            _audioService.PlaySound(ConstAudio.DisplaySound);
        }

        public async UniTask PlayWinAnimation(CancellationToken cancellationToken)
        {
            _winAnimation.Play();
            await UniTask.Delay(_winAnimationDurationMilliseconds, cancellationToken: cancellationToken);
        }

        public void PlayHighlightAnimation()
        {
            _highlightButtonAnimation.SetActive(true);
        }

        public void StopAllAnimations()
        {
            _highlightButtonAnimation.SetActive(false);
            _winAnimation.Stop();
        }
    }
}