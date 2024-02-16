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

        private TextManager _textManager;  // 文本管理器

        private Scene[] _scenes;    // 场景数组
        private TextBlock[] _narrations;    // 旁白数组
        private DialogBox _dialog;    // 对话框

        private int _currentNarrationIndex = 0;    // 当前旁白索引
        private (int, int) _displayingNarrationIndexes = (0, 0);    // 正在显示的旁白索引范围
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
            _currentNarrationIndex = 0;
            _displayingNarrationIndexes = (0, 0);
            await _scenes[_currentSceneIndex].Play(cancellationToken);
        }

        /// <summary>
        /// 继续或开始播放下一个文本块
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>  
        [Button]
        public async UniTask NextTextBlock(CancellationToken cancellationToken)
        {
            if (_isLineFinished)
            {
                _currentLine = _textManager.GetNextLine();
                _isLineFinished = false;
            }

            if (_currentLine.line == null)
            {
                Debug.Log("没有下一行");
                return;
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
        }

        /// <summary>
        /// 清空所有旁白 
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        [Button]
        public async UniTask ClearNarrations(CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(_narrations.Select(narration => narration.ExitBlock(cancellationToken)));
            // FIXME: 注释代码有问题，现有代码会推出所有旁白，导致显示问题
            // await UniTask.WhenAll(
            //     _narrations[_displayingNarrationIndexes.Item1.._displayingNarrationIndexes.Item2].Select(
            //         narration => narration.ExitBlock(cancellationToken)));
            _displayingNarrationIndexes = (_currentNarrationIndex, _currentNarrationIndex);
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
                _narrations[_currentNarrationIndex].SkipTyping();
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
            GameObject canvas = GameObject.Find("Canvas");
            _scenes = canvas.GetComponentsInChildren<Scene>();
            _dialog = canvas.GetComponentInChildren<DialogBox>();
            _textManager = GameObject.Find("TextManager").GetComponent<TextManager>();
        }

        private void SetNextNarration(Line line)
        {
            TextBlock textBlock = _narrations[_currentNarrationIndex];
            textBlock.SetText(line.English);
        }

        private async UniTask PlayNextNarration(CancellationToken cancellationToken)
        {
            TextBlock textBlock = _narrations[_currentNarrationIndex];
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
            _currentNarrationIndex++;
            _displayingNarrationIndexes.Item2 = _currentNarrationIndex;
        }

        private void SetNextDialog(Line line)
        {
            _dialog.SetImages(line);
            _dialog.SetText(line.English);
        }

        private async UniTask PlayNextDialog(CancellationToken cancellationToken)
        {
            await _dialog.PlayBlock(cancellationToken, _fastForward);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            _isTyping = true;

            await _dialog.StartTyping(cancellationToken, _fastForward);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            _isTyping = false;
            _isLineFinished = true;
        }
    }
}