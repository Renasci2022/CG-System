using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CG
{
    /// <summary>
    /// 旁白类，控制旁白的显示和隐藏
    /// </summary>
    public class Narration : TextBlock
    {
        [SerializeField] private float _duration = 1f; // 渐变时长
        [SerializeField] private float _fastForwardDuration = 0.1f; // 快进时渐变时长

        private Image _image;   // 旁白框
        private Color _color;   // 没有隐藏时的颜色
        private bool _entered = false;  // 是否已经进入
        private bool _isHiding = false; // 是否隐藏中
        private float _timer = 0f;  // 计时器

        /// <summary>
        /// 开始或继续播放旁白
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="fastForward">是否快进播放</param>
        public override async UniTask PlayBlock(CancellationToken cancellationToken, bool fastForward = true)
        {
            if (_entered)
            {
                return;
            }

            gameObject.SetActive(true);
            float duration = fastForward ? _fastForwardDuration : _duration;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (_timer > duration)
                {
                    StartTyping(cancellationToken, fastForward).Forget();
                    _timer = 0f;
                    _entered = true;
                    break;
                }
                _color.a = Mathf.Lerp(0f, 1f, _timer / duration);
                _image.color = _isHiding ? Color.clear : _color;
                _timer += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// 退出旁白
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="fastForward">是否快进播放</param>
        public override async UniTask ExitBlock(CancellationToken cancellationToken, bool fastForward = false)
        {
            float duration = fastForward ? _fastForwardDuration : _duration;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (_timer > duration)
                {
                    // TODO: 通知外部旁白播放结束
                    gameObject.SetActive(false);
                    _timer = 0f;
                    _entered = false;
                    break;
                }
                _color.a = Mathf.Lerp(1f, 0f, _timer / duration);
                _image.color = _isHiding ? Color.clear : _color;
                Color color = _textColor;
                color.a = _color.a;
                _textMeshPro.color = color;
                _timer += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// 隐藏旁白框及文字，展示背景
        /// </summary>
        public override void HideBlock()
        {
            _isHiding = true;
            _image.color = Color.clear;
            _textMeshPro.color = Color.clear;
        }

        /// <summary>
        /// 显示旁白框及文字
        /// </summary>
        public override void ShowBlock()
        {
            _isHiding = false;
            _image.color = _color;
            _textMeshPro.color = _textColor;
        }

        private new void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
        }

        private new void Start()
        {
            base.Start();
            gameObject.SetActive(false);
            _color = _image.color;
            _color.a = 0f;
        }
    }
}