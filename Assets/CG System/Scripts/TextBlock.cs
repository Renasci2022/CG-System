using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CG
{
    /// <summary>
    /// 文本块基类，控制文本的打印效果
    /// </summary>
    public abstract class TextBlock : MonoBehaviour
    {
        [SerializeField] protected Color _textColor = Color.black; // 文本颜色

        [SerializeField] private float _typeSpeed = 10f; // 每秒打印字符数
        [SerializeField] private float _fastForwardTypeSpeed = 50f; // 快进时每秒打印字符数

        protected TextMeshProUGUI _textMeshPro;

        /// <summary>
        /// 开始或继续打印文本
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="fastForward">是否快进打印</param>
        public async UniTask StartTyping(CancellationToken cancellationToken, bool fastForward = false)
        {
            float speed = fastForward ? _fastForwardTypeSpeed : _typeSpeed;

            while (true)
            {
                int length = _textMeshPro.maxVisibleCharacters;
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (length >= _textMeshPro.text.Length)
                {
                    // TODO: 通知外部文本打印结束
                    break;
                }

                float delay = 1f / speed;
                _textMeshPro.maxVisibleCharacters = length + 1;
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// 快进打印文本
        /// </summary>
        public void SkipTyping()
        {
            _textMeshPro.maxVisibleCharacters = _textMeshPro.text.Length;
        }

        public void SetText(string text)
        {
            _textMeshPro.text = text;
        }

        /// <summary>
        /// 播放文本块
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="fastForward">是否快进播放</param>
        public abstract UniTask PlayBlock(CancellationToken cancellationToken, bool fastForward = false);

        /// <summary>
        /// 退出文本块
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="fastForward">是否快进播放</param>
        public abstract UniTask ExitBlock(CancellationToken cancellationToken, bool fastForward = false);

        /// <summary>
        /// 隐藏文本块
        /// </summary>
        public abstract void HideBlock();

        /// <summary>
        /// 展示文本块
        /// </summary>
        public abstract void ShowBlock();

        protected void Awake()
        {
            _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        }

        protected void Start()
        {
            _textMeshPro.maxVisibleCharacters = 0;
            _textMeshPro.color = _textColor;
        }
    }
}