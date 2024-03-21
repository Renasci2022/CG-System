using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace CG
{
    public class Dialog : TextBlock
    {
        [SerializeField] private float _displaySpeed = 1f;  // 渐变速度
        [SerializeField] private float _fastForwardDisplaySpeed = 10f; // 快进时渐变速度

        [SerializeField] private AssetReference _expressionReference;   // 表情资源
        [SerializeField] private AssetReference _nameStampReference;    // 名章资源
        [SerializeField] private Sprite[] _dialogBoxes; // 对话框数组

        private Image _dialogBox;   // 对话框
        private Image _expression;  // 表情
        private Image _nameStamp;   // 名章
        private Image[] _imagesToChange;   // 需要改变的图片
        private Color _color;   // 没有隐藏时的颜色

        private bool _isEntering = false;   // 是否正在进入
        private bool _isExiting = false;    // 是否正在退出

        // TODO: 简化逻辑，封装一个 Initialize 方法给外部调用
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

        public void SetImages((DialogBoxType, string, string, EffectType) dialogInfo)
        {
            (DialogBoxType dialogBox, string character, string expression, EffectType effect) = dialogInfo;

            _dialogBox.sprite = _dialogBoxes[(int)dialogBox];
            // ! AssetReference 中不能包含中文
            // string imageReference = $"{_expressionReference}/{line.Character}/{line.Character}-{line.Expression}.png";
            string imageReference = $"Assets/CG System/Art/角色表情图/{character}/{character}-{expression}.png";
            Addressables.LoadAssetAsync<Sprite>(imageReference).Completed += handle => _expression.sprite = handle.Result;
            imageReference = $"Assets/CG System/Art/名字/名字-{character}.png";
            Addressables.LoadAssetAsync<Sprite>(imageReference).Completed += handle => _nameStamp.sprite = handle.Result;
        }

        // FIXME: 修复显示错误
        public override async UniTask Enter(CancellationToken token)
        {
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
                    Array.ForEach(_imagesToChange, image => image.color = CGPlayer.Instance.Hiding ? Color.clear : _color);
                    _textMeshPro.color = _textColor;

                    await StartTyping(token);
                    break;
                }
                float speed = CGPlayer.Instance.FastForward ? _fastForwardDisplaySpeed : _displaySpeed;
                _color.a += speed * Time.deltaTime;
                Array.ForEach(_imagesToChange, image => image.color = CGPlayer.Instance.Hiding ? Color.clear : _color);
                // ? Why is next line not working?
                // _imagesToChange.ForEach(image => image.color = CGPlayer.Instance.Hiding ? Color.clear : _color);
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
                    Array.ForEach(_imagesToChange, image => image.color = Color.clear);
                    _textMeshPro.color = Color.clear;
                    break;
                }
                float speed = CGPlayer.Instance.FastForward ? _fastForwardDisplaySpeed : _displaySpeed;
                _color.a -= speed * Time.deltaTime;
                Array.ForEach(_imagesToChange, image => image.color = _color);
                Color color = _textMeshPro.color;
                color.a = _color.a;
                _textMeshPro.color = color;
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
            _dialogBox.color = Color.clear;
            _expression.color = Color.clear;
            _nameStamp.color = Color.clear;
            _textMeshPro.color = Color.clear;
        }

        public override void Show()
        {
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
            _imagesToChange = new Image[] { _dialogBox, _expression, _nameStamp };
            Array.ForEach(_imagesToChange, image => image.color = Color.clear);
        }
    }
}