using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CG
{
    public class DialogBox : TextBlock
    {
        [SerializeField] private float _duration = 1f; // 渐变时长
        [SerializeField] private float _fastForwardDuration = 0.1f; // 快进时渐变时长

        private Image _dialogBox;   // 对话框
        private Image _expression;  // 表情
        private Image _nameStamp;   // 名章
        private Image[] _imagesToChange;   // 需要改变的图片
        private Color _color;   // 没有隐藏时的颜色
        private bool _isHiding = false; // 是否隐藏中
        private float _timer = 0f;  // 计时器

        public void SetImagesToChange(bool needChangeDialogBox)
        {
            if (needChangeDialogBox)
            {
                _imagesToChange = new Image[] { _dialogBox, _expression, _nameStamp };
            }
            else
            {
                _imagesToChange = new Image[] { _expression, _nameStamp };
            }
        }

        public void SetDialog(Image dialogBox, Image expression, Image nameStamp, string text)
        {
            _dialogBox = dialogBox;
            _expression = expression;
            _nameStamp = nameStamp;
            _textMeshPro.text = text;
        }

        public override async UniTask PlayBlock(CancellationToken cancellationToken, bool fastForward = false)
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
                    Array.ForEach(_imagesToChange, image => image.color = _isHiding ? Color.clear : _color);
                    _textMeshPro.color = _textColor;
                    await StartTyping(cancellationToken, fastForward);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    _timer = 0f;
                    break;
                }
                _color.a = Mathf.Lerp(0f, 1f, _timer / duration);
                Array.ForEach(_imagesToChange, image => image.color = _isHiding ? Color.clear : _color);
                // ? Why is next line not working?
                // _imagesToChange.ForEach(image => image.color = _isHiding ? Color.clear : _color);
                _timer += Time.deltaTime;
                await UniTask.Yield();
            }
        }

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
                    // TODO: 通知外部对话播放结束
                    Array.ForEach(_imagesToChange, image => image.color = Color.clear);
                    _textMeshPro.color = Color.clear;
                    _timer = 0f;
                    break;
                }
                _color.a = Mathf.Lerp(1f, 0f, _timer / duration);
                Array.ForEach(_imagesToChange, image => image.color = _color);
                Color color = _textMeshPro.color;
                color.a = _color.a;
                _textMeshPro.color = color;
                _timer += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        public override void HideBlock()
        {
            _isHiding = true;
            _dialogBox.color = Color.clear;
            _expression.color = Color.clear;
            _nameStamp.color = Color.clear;
            _textMeshPro.color = Color.clear;
        }

        public override void ShowBlock()
        {
            _isHiding = false;
            _dialogBox.color = _color;
            _expression.color = _color;
            _nameStamp.color = _color;
            _textMeshPro.color = _textColor;
        }

        private new void Awake()
        {
            base.Awake();
            _dialogBox = GameObject.Find("DialogBox").GetComponent<Image>();
            _expression = GameObject.Find("Expression").GetComponent<Image>();
            _nameStamp = GameObject.Find("NameStamp").GetComponent<Image>();
        }

        private new void Start()
        {
            base.Start();
            _color = Color.white;
            _color.a = 0f;
            Array.ForEach(_imagesToChange, image => image.color = Color.clear);
            _imagesToChange = new Image[] { _dialogBox, _expression, _nameStamp };
        }
    }
}