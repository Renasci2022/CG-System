using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace CG
{
    /// <summary>
    /// CG 管理器
    /// 调用 CGPlayer 的方法，维护状态机
    /// </summary>
    public class CGManager : MonoBehaviour
    {
        private CGState _state = CGState.None; // 当前状态
        private CGState _lastState = CGState.None; // 上一个状态

        private delegate UniTask PlayMethod(CancellationToken cancellationToken);   // 播放方法
        [ShowInInspector] private PlayMethod[] _playMethods;   // 播放方法数组

        private CancellationTokenSource _cancellationTokenSource;    // 取消令牌

        private CGPlayer _player; // CG 播放器

        [Button]
        public async UniTask Next()
        {
            _state = CGState.Playing;
            _lastState = CGState.None;
            _cancellationTokenSource = new CancellationTokenSource();
            await UniTask.WhenAll(_playMethods.Select(method => method(_cancellationTokenSource.Token)));

            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }
            // TODO: 播放完毕后的处理，发送信号等
            _playMethods = new PlayMethod[] { };
        }

        [Button]
        public void Pause()
        {
            _lastState = _state;
            _state = CGState.Paused;
            _cancellationTokenSource.Cancel();
        }

        [Button]
        public async UniTask Resume()
        {
            if (_lastState == CGState.Playing)
            {
                await Next();
            }
            else if (_lastState == CGState.Waiting)
            {
                await UniTask.DelayFrame(1);
                _lastState = _state;
                _state = CGState.Waiting;
            }
            else
            {
                throw new System.Exception("Invalid state");
            }
        }

        [Button]
        public void SetFastForward(bool fastForward)
        {
            _player.FastForward = fastForward;
            Pause();
            Resume().Forget();
        }

        [Button]
        public void Hide()
        {
            _lastState = _state;
            _state = CGState.Hiding;
            _player.HideTextBlocks();
        }

        [Button]
        public void Show()
        {
            _state = _lastState;
            _lastState = CGState.None;
            _player.ShowTextBlocks();
        }

        [Button]
        public void Test_PlayScene()
        {
            _playMethods = new PlayMethod[] { _player.PlayScene };
        }

        [Button]
        public void Test_NextTextBlock()
        {
            _playMethods = new PlayMethod[] { _player.NextTextBlock };
        }

        [Button]
        public void Test_ClearMethods()
        {
            _playMethods = new PlayMethod[] { };
        }

        private void Awake()
        {
            _player = GameObject.Find("CGPlayer").GetComponent<CGPlayer>();
        }
    }

    public enum CGState
    {
        None,
        Playing,
        Waiting,
        Paused,
        Hiding,
    }
}