using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CG
{
    public class Scene : MonoBehaviour, IPlayable
    {
        [SerializeField] private float _displaySpeed = 1f;  // 渐变速度

        private Image _background;  // 背景图片
        private Color _color;   // 没有隐藏时的颜色

        public async UniTask Enter(CancellationToken token)
        {
            gameObject.SetActive(true);
            _background.color = Color.clear;

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
                    _background.color = Color.white;
                    break;
                }
                _color.a += _displaySpeed * Time.deltaTime;
                _background.color = _color;
                await UniTask.Yield();
            }
        }

        public async UniTask Exit(CancellationToken token)
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
                    gameObject.SetActive(false);
                    break;
                }
                _color.a -= _displaySpeed * Time.deltaTime;
                _background.color = _color;
                await UniTask.Yield();
            }
        }

        private void Awake()
        {
            _background = GetComponentInChildren<Image>();

            _color = Color.white;
            _color.a = 0f;
            _background.color = _color;
            gameObject.SetActive(false);
        }
    }
}