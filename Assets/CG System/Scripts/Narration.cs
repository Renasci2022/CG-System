using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CG
{
    public class Narration : TextBlock
    {
        [SerializeField] private float _displaySpeed = 1f; // 渐变速度
        [SerializeField] private float _fastForwardDisplaySpeed = 10f; // 快进时渐变速度

        private Image _image;   // 旁白框
        private Color _color;   // 没有隐藏时的颜色
        private bool _isHiding = false; // 是否隐藏中

        public override async UniTask Play(CancellationToken token)
        {
            gameObject.SetActive(true);

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                if (CGPlayer.Instance.Paused)
                {
                    await UniTask.DelayFrame(1, cancellationToken: token);
                    continue;
                }
                if (_color.a >= 1f)
                {
                    _color.a = 1f;
                    _image.color = _isHiding ? Color.clear : _color;
                    await StartTyping(token);
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    break;
                }

                float speed = CGPlayer.Instance.FastForward ? _fastForwardDisplaySpeed : _displaySpeed;
                _color.a += speed * Time.deltaTime;
                _image.color = _isHiding ? Color.clear : _color;
                await UniTask.Yield();
            }
        }

        public override async UniTask Exit(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                if (CGPlayer.Instance.Paused)
                {
                    await UniTask.DelayFrame(1, cancellationToken: token);
                    continue;
                }
                if (_color.a <= 0f)
                {
                    _color.a = 0f;
                    _image.color = Color.clear;
                    _textMeshPro.color = Color.clear;
                    gameObject.SetActive(false);
                    break;
                }
                float speed = CGPlayer.Instance.FastForward ? _fastForwardDisplaySpeed : _displaySpeed;
                _color.a -= speed * Time.deltaTime;
                _image.color = _isHiding ? Color.clear : _color;
                Color color = _textColor;
                color.a = _color.a;
                _textMeshPro.color = _isHiding ? Color.clear : color;
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