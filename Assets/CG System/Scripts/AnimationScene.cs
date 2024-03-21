using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace CG
{
    public class AnimationScene : Scene
    {
        [SerializeField] private int _fps = 8;
        [SerializeField] private Sprite[] _frames;

        private int _index = 0;

        public override async UniTask Enter(CancellationToken token)
        {
            gameObject.SetActive(true);

            await base.Enter(token);

            float interval = 1f / _fps;
            while (_index < _frames.Length)
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
                _background.sprite = _frames[_index];
                await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
                _index++;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _background = GetComponent<Image>();
            _background.sprite = _frames[0];
        }

    }
}