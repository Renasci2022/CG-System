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
        public bool FastForward { get; set; } = false;    // 是否快进

        public delegate UniTask PlayMethod(bool fastForward, CancellationToken token);    // 播放方法委托

        public List<PlayMethod> PlayMethods = new();    // 播放方法列表

        public List<TextBlock> TextBlocks = new();  // 文本块列表

        public async UniTask Play(CancellationToken token)
        {
            UniTask[] tasks = PlayMethods.Select(method => method(FastForward, token)).ToArray();
            await UniTask.WhenAll(tasks);
        }

        public async UniTask Pause(CancellationToken token)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);
        }

        public async UniTask Stop(CancellationToken token)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);
        }

        public void Skip()
        {
            TextBlocks.ForEach(textBlock => textBlock.Skip());
        }

        public void Hide()
        {
            TextBlocks.ForEach(textBlock => textBlock.Hide());
        }

        public void Show()
        {
            TextBlocks.ForEach(textBlock => textBlock.Show());
        }
    }
}