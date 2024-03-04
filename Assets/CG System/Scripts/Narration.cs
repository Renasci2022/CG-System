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

        private bool _isEntering = false;   // 是否正在进入
        private bool _isExiting = false;    // 是否正在退出

        public override async UniTask Enter(CancellationToken token)
        {
            gameObject.SetActive(true);
            _isEntering = true;

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
                    _isEntering = false;

                    _color.a = 1f;
                    _image.color = CGPlayer.Instance.Hiding ? Color.clear : _color;

                    await StartTyping(token);
                    break;
                }

                float speed = CGPlayer.Instance.FastForward ? _fastForwardDisplaySpeed : _displaySpeed;
                _color.a += speed * Time.deltaTime;
                _image.color = CGPlayer.Instance.Hiding ? Color.clear : _color;
                await UniTask.Yield();
            }
        }

        public override async UniTask Exit(CancellationToken token)
        {
            _isExiting = true;

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
                    _isExiting = false;
                    _color.a = 0f;
                    _image.color = Color.clear;
                    _textMeshPro.color = Color.clear;
                    gameObject.SetActive(false);
                    break;
                }
                float speed = CGPlayer.Instance.FastForward ? _fastForwardDisplaySpeed : _displaySpeed;
                _color.a -= speed * Time.deltaTime;
                _image.color = CGPlayer.Instance.Hiding ? Color.clear : _color;
                Color color = _textColor;
                color.a = _color.a;
                _textMeshPro.color = CGPlayer.Instance.Hiding ? Color.clear : color;
                await UniTask.Yield();
            }
        }

        public override void Skip()
        {
            if (_isEntering)
            {
                _color.a = 1f;
            }
            if (_isExiting)
            {
                _color.a = 0f;
            }

            SkipTyping();
        }

        public override void Hide()
        {
            _image.color = Color.clear;
            _textMeshPro.color = Color.clear;
        }

        public override void Show()
        {
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