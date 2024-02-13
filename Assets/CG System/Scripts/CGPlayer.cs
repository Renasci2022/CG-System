using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace CG
{
    /// <summary>
    /// CG 播放器 
    /// </summary>
    public class CGPlayer : MonoBehaviour
    {
        private bool _fastForward = false;  // 是否快进
        private bool _isTyping = false; // 是否正在打字

        private int _currentSceneIndex = 0; // 当前场景索引
        private int _currentTextBlockIndex = 0; // 当前文本块索引

        private Scene[] _scenes;    // 场景数组
        private TextBlock[] _narrations;    // 对话数组

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
        /// 播放下一个文本块
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>  
        [Button]
        public async UniTask NextTextBlock(CancellationToken cancellationToken)
        {
            // TODO: 目前只支持 Narration 的播放
            if (_currentTextBlockIndex >= _narrations.Length)
            {
                return;
            }

            TextBlock textBlock = _narrations[_currentTextBlockIndex];
            await textBlock.PlayBlock(cancellationToken, _fastForward);
            _isTyping = true;
            await textBlock.StartTyping(cancellationToken, _fastForward);
            _isTyping = false;
            _currentTextBlockIndex++;

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
            await UniTask.WhenAll(_narrations.Select(narration => narration.ExitBlock(cancellationToken)));
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
        /// 设置快进模式 
        /// </summary>
        /// <param name="fastForward">是否快进</param>
        /// <param name="cancellationToken">取消令牌</param>
        [Button]
        public void SetFastForward(bool fastForward, CancellationToken cancellationToken)
        {
            if (fastForward == _fastForward)
            {
                return;
            }

            _fastForward = fastForward;
            // TODO: 根据情况是否取消当前的播放，并以快进模式重新播放
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
        public void HideNarrations()
        {
            // TODO: 还未考虑对话的情况
            _narrations.ForEach(narration => narration.HideBlock());
        }

        /// <summary>
        /// 显示所有旁白
        /// </summary> 
        [Button]
        public void ShowNarrations()
        {
            // TODO: 还未考虑对话的情况
            _narrations.ForEach(narration => narration.ShowBlock());
        }

        private void Awake()
        {
            GameObject canvasObject = GameObject.Find("Canvas");
            if (canvasObject == null)
            {
                Debug.LogError("Canvas not found");
                return;
            }
            _scenes = canvasObject.GetComponentsInChildren<Scene>();
            if (_scenes.Length == 0)
            {
                Debug.LogError("No scene found");
                return;
            }
        }
    }
}