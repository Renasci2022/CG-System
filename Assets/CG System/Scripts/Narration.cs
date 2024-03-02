using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CG
{
    public class Narration : TextBlock
    {
        [SerializeField] private float _duration = 1f; // 渐变时长
        [SerializeField] private float _fastForwardDuration = 0.1f; // 快进时渐变时长

        private Image _image;   // 旁白框
        private Color _color;   // 没有隐藏时的颜色
        private bool _isHiding = false; // 是否隐藏中
        private float _timer = 0f;  // 计时器

        public override async UniTask Play(bool fastForward, CancellationToken cancellationToken)
        {
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
                    _image.color = _isHiding ? Color.clear : _color;
                    await StartTyping(fastForward, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    _timer = 0f;
                    break;
                }
                _color.a = Mathf.Lerp(0f, 1f, _timer / duration);
                _image.color = _isHiding ? Color.clear : _color;
                _timer += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        public override async UniTask Exit(bool fastForward, CancellationToken cancellationToken)
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
                    _image.color = Color.clear;
                    _textMeshPro.color = Color.clear;
                    gameObject.SetActive(false);
                    _timer = 0f;
                    break;
                }
                _color.a = Mathf.Lerp(1f, 0f, _timer / duration);
                _image.color = _isHiding ? Color.clear : _color;
                Color color = _textColor;
                color.a = _color.a;
                _textMeshPro.color = _isHiding ? Color.clear : color;
                _timer += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        public override void Skip()
        {
            SkipTyping();
        }

        public override void Hide()
        {
            _isHiding = true;
            _image.color = Color.clear;
            _textMeshPro.color = Color.clear;
        }

        public override void Show()
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
            _color = Color.white;
            _color.a = 0f;
            _image.color = Color.clear;
        }
    }
}