using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace CG
{
    /// <summary>
    /// CG 播放器 
    /// 手动调试时请保证方法调用的正确性，例如在播放场景时，不要调用退出场景方法
    /// CGManager 会自动调用这些方法
    /// </summary>
    public class CGPlayer : MonoBehaviour
    {
        public bool FastForward
        {
            get => _fastForward;
            set => _fastForward = value;
        }

        private bool _fastForward = false;  // 是否快进
        private bool _isTyping = false; // 是否正在打字

        private int _currentSceneIndex = 0; // 当前场景索引
        private int _currentTextBlockIndex = 0; // 当前文本块索引

        private TextManager _textManager;  // 文本管理器

        private Scene[] _scenes;    // 场景数组
        private TextBlock[] _narrations;    // 对话数组
        private (int, int) _narrationIndex = (0, 0);    // 当前对话索引
        private (Line line, bool isDialog) _currentLine;    // 当前文本
        private bool _isLineFinished = true;    // 当前文本是否播放完毕

        /// <summary>
        /// 播放场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        [Button]
        public async UniTask PlayScene(CancellationToken cancellationToken)
        {
            if (_currentSceneIndex >= _scenes.Length)
            {
                return;
            }

            _narrations = _scenes[_currentSceneIndex].GetComponentsInChildren<Narration>();
            _currentTextBlockIndex = 0;
            await _scenes[_currentSceneIndex].Play(cancellationToken);
        }

        /// <summary>
        /// 继续或开始播放下一个文本块
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>  
        [Button]
        public async UniTask NextTextBlock(CancellationToken cancellationToken)
        {
            if (_currentTextBlockIndex >= _narrations.Length)
            {
                Debug.LogError("错误：没有更多文本块，不该调用 NextTextBlock");
                return;
            }

            if (_isLineFinished)
            {
                _currentLine = _textManager.GetNextLine();
                _isLineFinished = false;
            }
            if (_currentLine.isDialog)
            {
                SetNextDialog(_currentLine.line);
                await PlayNextDialog(cancellationToken);
            }
            else
            {
                SetNextNarration(_currentLine.line);
                await PlayNextNarration(cancellationToken);
            }

            if (_currentTextBlockIndex > _narrations.Length)
            {
                // TODO: 通知外部文本块播放完毕
            }
        }

        /// <summary>
        /// 清空所有旁白 
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        [Button]
        public async UniTask ClearNarrations(CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(
                _narrations[_narrationIndex.Item1.._narrationIndex.Item2].Select(
                    narration => narration.ExitBlock(cancellationToken)));
            _narrationIndex = (_currentTextBlockIndex, _currentTextBlockIndex);
        }

        /// <summary>
        /// 退出场景
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        [Button]
        public async UniTask ExitScene(CancellationToken cancellationToken)
        {
            ClearNarrations(cancellationToken).Forget();
            await _scenes[_currentSceneIndex].Exit(cancellationToken);
            _currentSceneIndex++;
            if (_currentSceneIndex > _scenes.Length)
            {
                // TODO: 通知外部场景播放完毕
            }
        }

        /// <summary>
        /// 如果正在打字，则跳过打字
        /// </summary>
        [Button]
        public void SkipTyping()
        {
            if (_isTyping)
            {
                _narrations[_currentTextBlockIndex].SkipTyping();
            }
        }

        /// <summary>
        /// 隐藏所有旁白
        /// </summary>
        [Button]
        public void HideTextBlocks()
        {
            // TODO: 还未考虑对话的情况
            _narrations.ForEach(narration => narration.HideBlock());
        }

        /// <summary>
        /// 显示所有旁白
        /// </summary> 
        [Button]
        public void ShowTextBlocks()
        {
            // TODO: 还未考虑对话的情况
            _narrations.ForEach(narration => narration.ShowBlock());
        }

        private void Awake()
        {
            _scenes = GameObject.Find("Canvas").GetComponentsInChildren<Scene>();
            _textManager = GameObject.Find("TextManager").GetComponent<TextManager>();
        }

        private void SetNextNarration(Line line)
        {
            TextBlock textBlock = _narrations[_currentTextBlockIndex];
            textBlock.SetText(line.English);
        }

        private async UniTask PlayNextNarration(CancellationToken cancellationToken)
        {
            TextBlock textBlock = _narrations[_currentTextBlockIndex];
            await textBlock.PlayBlock(cancellationToken, _fastForward);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            _isTyping = true;

            await textBlock.StartTyping(cancellationToken, _fastForward);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            _isTyping = false;
            _isLineFinished = true;
            _currentTextBlockIndex++;
            _narrationIndex.Item2 = _currentTextBlockIndex;
        }

        private void SetNextDialog(Line line)
        {
            // TODO: 设置对话
        }

        private async UniTask PlayNextDialog(CancellationToken cancellationToken)
        {
            // TODO: 播放对话
            await UniTask.Yield();
            throw new System.NotImplementedException();
        }
    }
}