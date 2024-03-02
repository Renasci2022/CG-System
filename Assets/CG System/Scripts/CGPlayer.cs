using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        private PlayState _previousPlayState;   // 上一个播放状态

        public async UniTask Play(CancellationToken token)
        {
            PlayState = PlayState.Playing;
            UniTask[] tasks = PlayMethods.Select(method => method(token)).ToArray();
            await UniTask.WhenAll(tasks);
        }

        public void Stop()
        {
            PlayState = PlayState.Stopped;
        }

        public async UniTask Pause()
        {
            _previousPlayState = PlayState;
            PlayState = PlayState.Paused;
            Paused = true;
            await UniTask.DelayFrame(1);
        }

        public async UniTask Resume()
        {
            PlayState = _previousPlayState;
            Paused = false;
            await UniTask.DelayFrame(1);
        }

        public void Skip()
        {
            TextBlocks.ForEach(textBlock => textBlock.Skip());
        }

        public void Hide()
        {
            _previousPlayState = PlayState;
            PlayState = PlayState.Hiding;
            TextBlocks.ForEach(textBlock => textBlock.Hide());
        }

        public void Show()
        {
            PlayState = _previousPlayState;
            TextBlocks.ForEach(textBlock => textBlock.Show());
        }

        public void SetFastForward(bool fastForward)
        {
            AutoPlay = fastForward;
            FastForward = fastForward;
        }

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