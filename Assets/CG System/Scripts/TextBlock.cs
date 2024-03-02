using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CG
{
    public abstract class TextBlock : MonoBehaviour, IPlayable
    {
        [SerializeField] protected Color _textColor = Color.black; // 文本颜色

        [SerializeField] private float _typeSpeed = 10f;  // 每秒打印字符数
        [SerializeField] private float _fastForwardTypeSpeed = 50f;   // 快进时每秒打印字符数

        protected TextMeshProUGUI _textMeshPro;

        public string Text
        {
            get => _textMeshPro.text;
            set => _textMeshPro.text = value;
        }

        public async UniTask StartTyping(bool fastForward, CancellationToken cancellationToken)
        {
            float speed = fastForward ? _fastForwardTypeSpeed : _typeSpeed;

            while (true)
            {
                int length = _textMeshPro.maxVisibleCharacters;
                if (cancellationToken.IsCancellationRequested)  // 取消
                {
                    break;
                }
                if (length >= _textMeshPro.text.Length) // 文本播放完毕
                {
                    break;
                }

                float delay = 1f / speed;
                _textMeshPro.maxVisibleCharacters = length + 1;
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            }
        }

        public void SkipTyping()
        {
            _textMeshPro.maxVisibleCharacters = _textMeshPro.text.Length;
        }

        protected void Awake()
        {
            _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        }

        protected void Start()
        {
            _textMeshPro.maxVisibleCharacters = 0;
            _textMeshPro.color = _textColor;
        }

        public abstract UniTask Play(bool fastForward, CancellationToken cancellationToken);
        public abstract UniTask Exit(bool fastForward, CancellationToken cancellationToken);
        public abstract void Skip();
        public abstract void Hide();
        public abstract void Show();
    }
}