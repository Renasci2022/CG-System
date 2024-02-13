using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CG
{
    /// <summary>
    /// 场景类，控制场景的显示和隐藏
    /// </summary>
    public class Scene : MonoBehaviour
    {
        [SerializeField] private float _duration = 1f;  // 渐变时长

        private Image _background;  // 背景图片
        private float _timer = 0f;  // 计时器

        /// <summary>
        /// 开始或继续播放场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        public async UniTask Play(CancellationToken cancellationToken)
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

        /// <summary>
        /// 退出场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        public async UniTask Exit(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (_timer > _duration)
                {
                    // TODO: 通知外部播放完毕
                    gameObject.SetActive(false);
                    _timer = 0f;
                    break;
                }
                _background.color = Color.Lerp(Color.white, Color.clear, _timer / _duration);
                _timer += Time.deltaTime;
                await UniTask.Yield();
            }
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