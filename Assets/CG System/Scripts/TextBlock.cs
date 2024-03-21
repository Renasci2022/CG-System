using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CG
{
    public abstract class TextBlock : MonoBehaviour, IPlayable, ISkipable
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

        public async UniTask StartTyping(CancellationToken token)
        {
            while (true)
            {
                int length = _textMeshPro.maxVisibleCharacters;
                if (token.IsCancellationRequested)  // 取消
                {
                    break;
                }
                if (CGPlayer.Instance.Paused)  // 暂停
                {
                    await UniTask.DelayFrame(1, cancellationToken: token);
                    continue;
                }
                if (length >= _textMeshPro.text.Length) // 文本播放完毕
                {
                    break;
                }

                float speed = CGPlayer.Instance.FastForward ? _fastForwardTypeSpeed : _typeSpeed;
                float delay = 1f / speed;
                _textMeshPro.maxVisibleCharacters = length + 1;
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
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
            // TODO: 正确设置字体资产

            InitializeText();
        }

        public abstract UniTask Enter(CancellationToken cancellationToken);
        public abstract UniTask Exit(CancellationToken cancellationToken);
        public abstract void Skip();
        public abstract void Hide();
        public abstract void Show();

        protected void InitializeText()
        {
            _textMeshPro.maxVisibleCharacters = 0;
            _textMeshPro.color = _textColor;
        }
    }
}