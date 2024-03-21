using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace CG
{
    public class CGPlayer : MonoBehaviour
    {
        public delegate UniTask PlayMethod(CancellationToken token);    // 播放方法委托
        public List<PlayMethod> PlayMethods = new();    // 播放方法列表
        public List<TextBlock> TextBlocks = new();  // 文本块列表

        // TODO: 用事件代替 Instance
        public static CGPlayer Instance { get; private set; }
        public PlayState PlayState { get; private set; } = PlayState.Stopped;    // 播放状态
        public bool FastForward { get; private set; } = false;    // 是否快进
        public bool AutoPlay { get; private set; } = false;    // 是否自动播放
        public bool Paused => PlayState == PlayState.Paused;    // 是否暂停
        public bool Hiding => PlayState == PlayState.Hiding;    // 是否隐藏
        public Language Language { get; private set; } = Language.Chinese;    // 语言

        private CGManager _manager;    // CG 管理器

        private PlayState _previousPlayState;   // 上一个播放状态
        private CancellationTokenSource _cancellationTokenSource;    // 取消令牌源

        private float _timer;   // 计时器

        [Button("Play")]
        public async UniTask Play()
        {
            PlayState = PlayState.Playing;
            _cancellationTokenSource = new();
            UniTask[] tasks = PlayMethods.Select(method => method(_cancellationTokenSource.Token)).ToArray();
            await UniTask.WhenAll(tasks);
        }

        [Button("Stop")]
        public void Stop()
        {
            PlayState = PlayState.Stopped;
            _cancellationTokenSource?.Cancel();
        }

        [Button("Next")]
        public void Next()
        {
            if (PlayState != PlayState.Waiting)
            {
                return;
            }
            _manager.NextLine().Forget();
        }

        [Button("Pause")]
        public async UniTask Pause()
        {
            _previousPlayState = PlayState;
            PlayState = PlayState.Paused;
            await UniTask.DelayFrame(1);
        }

        [Button("Resume")]
        public async UniTask Resume()
        {
            PlayState = _previousPlayState;
            await UniTask.DelayFrame(1);
        }

        [Button("Skip")]
        public void Skip()
        {
            _timer = 0f;
            TextBlocks.ForEach(textBlock => textBlock.Skip());
        }

        [Button("Hide")]
        public void Hide()
        {
            _previousPlayState = PlayState;
            PlayState = PlayState.Hiding;
            TextBlocks.ForEach(textBlock => textBlock.Hide());
        }

        [Button("Show")]
        public void Show()
        {
            PlayState = _previousPlayState;
            TextBlocks.ForEach(textBlock => textBlock.Show());
        }

        [Button("Set Fast Forward")]
        public void SetFastForward(bool fastForward)
        {
            AutoPlay = fastForward;
            FastForward = fastForward;
        }

        [Button("Set Auto Play")]
        public void SetAutoPlay(bool autoPlay)
        {
            AutoPlay = autoPlay;
        }

        public async UniTask NextAfterInterval(float interval)
        {
            if (interval < 0)
            {
                PlayState = PlayState.Waiting;
                return;
            }

            _timer = interval;
            while (_timer > 0)
            {
                if (PlayState == PlayState.Paused)
                {
                    await UniTask.DelayFrame(1);
                    continue;
                }
                await UniTask.DelayFrame(1);
                _timer -= Time.deltaTime;
            }
            _manager.NextLine().Forget();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _manager = FindObjectOfType<CGManager>();
        }
    }

    public enum PlayState
    {
        Stopped,
        Playing,
        Waiting,
        Paused,
        Hiding,
    }
}