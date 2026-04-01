using System;
using System.Collections.Generic;
using System.Threading;
using Application.Game.TicTacToy;
using Core.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Application.UI
{
    public class TicTacToyScreen : UiScreen
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _undoButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _hintButton;
        [SerializeField] private BoardButton[] _playButtons;
        [SerializeField] private TextMeshProUGUI _timerText;

        private BoardModel _boardModel;

        public event Action BackButtonPressEvent;
        public event Action UndoPressEvent;
        public event Action RestartPressEvent;
        public event Action HintPressEvent;
        public event Action<int> SlotPressEvent;

        [Inject]
        public void Construct(BoardModel boardModel)
        {
            _boardModel = boardModel;
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveAllListeners();
            _undoButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
            _hintButton.onClick.RemoveAllListeners();

            foreach (var boardButton in _playButtons)
                boardButton.Button.onClick.RemoveAllListeners();

            _boardModel.BoardUpdateEvent -= UpdateSlotView;
        }

        public void Initialize()
        {
            _backButton.onClick.AddListener(OnBackButtonPress);
            _undoButton.onClick.AddListener(OnUndoButtonPress);
            _restartButton.onClick.AddListener(OnRestartButtonPress);
            _hintButton.onClick.AddListener(OnHintButtonPress);

            for (int i = 0; i < _playButtons.Length; i++)
            {
                int index = _playButtons[i].Index;
                _playButtons[i].Button.onClick.AddListener(() => OnSlotButtonPress(index));
            }

            _boardModel.BoardUpdateEvent += UpdateSlotView;
        }

        public async UniTask PlayWinAnimation(int[] pattern, CancellationToken cancellationToken)
        {
            List<UniTask> tasks = new List<UniTask>(3);

            foreach (var index in pattern)
            {
                foreach (var playButton in _playButtons)
                {
                    if (playButton.Index == index)
                    {
                        tasks.Add(playButton.PlayWinAnimation(cancellationToken));
                        break;
                    }
                }
            }

            await UniTask.WhenAll(tasks);
        }

        public void HighlightButton(int buttonIndex)
        {
            _playButtons[buttonIndex].PlayHighlightAnimation();
        }

        public void EnableInput(bool enable)
        {
            _backButton.interactable = enable;
            _undoButton.interactable = enable;
            _restartButton.interactable = enable;
            _hintButton.interactable = enable;

            for (int i = 0; i < _playButtons.Length; i++)
                _playButtons[i].Button.interactable = enable;
        }

        public void UpdateTimerText(string time)
        {
            _timerText.text = time;
        }

        public void EnableUndoButton(bool active)
        {
            _undoButton.gameObject.SetActive(active);
        }
        
        public void EnableHintButton(bool active)
        {
            _hintButton.gameObject.SetActive(active);
        }

        private void UpdateSlotView(Slot slotData)
        {
            _playButtons[slotData.Index].UpdateView(slotData.Type);
        }

        private void OnBackButtonPress()
        {
            BackButtonPressEvent?.Invoke();
        }

        private void OnUndoButtonPress()
        {
            UndoPressEvent?.Invoke();
        }

        private void OnRestartButtonPress()
        {
            RestartPressEvent?.Invoke();
        }

        private void OnSlotButtonPress(int index)
        {
            foreach (var button in _playButtons)
                button.StopAllAnimations();

            SlotPressEvent?.Invoke(index);
        }

        private void OnHintButtonPress()
        {
            HintPressEvent?.Invoke();
        }
    }
}