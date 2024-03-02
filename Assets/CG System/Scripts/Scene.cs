using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CG
{
    public class Scene : MonoBehaviour, IPlayable
    {
        [SerializeField] private float _duration = 1f;  // 渐变时长

        private Image _background;  // 背景图片
        private float _timer = 0f;  // 计时器

        public async UniTask Play(bool fastForward, CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
            _background.color = Color.clear;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (_timer > _duration)
                {
                    _background.color = Color.white;
                    _timer = 0f;
                    break;
                }
                _background.color = Color.Lerp(Color.clear, Color.white, _timer / _duration);
                _timer += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        public async UniTask Exit(bool fastForward, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (_timer > _duration)
                {
                    gameObject.SetActive(false);
                    _timer = 0f;
                    break;
                }
                _background.color = Color.Lerp(Color.white, Color.clear, _timer / _duration);
                _timer += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        public void Skip()
        {
        }

        public void Hide()
        {
        }

        public void Show()
        {
        }

        private void Awake()
        {
            _background = GetComponentInChildren<Image>();
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }
    }
}