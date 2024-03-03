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

        public static CGPlayer Instance { get; private set; }
        public PlayState PlayState { get; private set; } = PlayState.Stopped;    // 播放状态
        public bool FastForward { get; private set; } = false;    // 是否快进
        public bool AutoPlay { get; private set; } = false;    // 是否自动播放
        public bool Paused { get; private set; } = false;    // 是否暂停
        public Language Language { get; private set; } = Language.English;    // 语言

        private PlayState _previousPlayState;   // 上一个播放状态
        private CancellationTokenSource _cancellationTokenSource;    // 取消令牌源

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

        [Button("Pause")]
        public async UniTask Pause()
        {
            _previousPlayState = PlayState;
            PlayState = PlayState.Paused;
            Paused = true;
            await UniTask.DelayFrame(1);
        }

        [Button("Resume")]
        public async UniTask Resume()
        {
            PlayState = _previousPlayState;
            Paused = false;
            await UniTask.DelayFrame(1);
        }

        [Button("Skip")]
        public void Skip()
        {
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

        private void Awake()
        {
            Instance = this;
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