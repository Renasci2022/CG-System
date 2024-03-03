using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CG
{
    public class CGManager : MonoBehaviour
    {
        private XMLLine _line;

        private Scene[] _scenes;
        private Narration[] _narrations;
        private Dialog _dialog;
        private CGPlayer _player;
        private XMLReader _reader;

        private CancellationTokenSource _intervalCts;

        private int _currentSceneIndex = 0;
        private int _currentNarrationIndex = 0;

        public async UniTask NextLine()
        {
            _line = _reader.GetNextLine();
            Debug.Log($"Type: {_line.Type}, Text: {_line.Text}");

            if (_line == null)
            {
                Debug.Log("End of the script");
                return;
            }
            else if (_line.Type == LineType.Scene)
            {
                _narrations = _scenes[_currentSceneIndex].GetComponentsInChildren<Narration>();
                _currentNarrationIndex = 0;

                await ClearTextBlocks();
                await PlayScene(_scenes[_currentSceneIndex]);
                _currentSceneIndex++;
            }
            else if (_line.Type == LineType.Narration)
            {
                _narrations[_currentNarrationIndex].Text = _line.Text;

                await PlayTextBlock(_narrations[_currentNarrationIndex]);
                _currentNarrationIndex++;
            }
            else if (_line.Type == LineType.Dialog)
            {
                _dialog.Text = _line.Text;
                _dialog.SetImages(_line.DialogInfo);
                _dialog.SetImagesToChange(true);

                await ClearTextBlocks();
                await PlayTextBlock(_dialog);
            }
            else
            {
                Debug.LogError("Unknown line type");
            }

            NextLineAfterInterval(_line.Interval).Forget();
        }

        private void Awake()
        {
            GameObject canvas = GameObject.Find("Canvas");
            _scenes = canvas.GetComponentsInChildren<Scene>();
            _dialog = canvas.GetComponentInChildren<Dialog>();
            _player = GameObject.Find("CGPlayer").GetComponent<CGPlayer>();
            _reader = GetComponent<XMLReader>();
        }

        private void Start()
        {
            NextLine().Forget();
        }

        private async UniTask PlayScene(Scene scene)
        {
            CGPlayer.PlayMethod playMethod = scene.Play;
            _player.PlayMethods.Add(playMethod);
            await _player.Play();
            _player.PlayMethods.Clear();
        }

        private async UniTask PlayTextBlock(TextBlock textBlock)
        {
            CGPlayer.PlayMethod playMethod = textBlock.Play;
            _player.PlayMethods.Add(playMethod);
            _player.TextBlocks.Add(textBlock);
            await _player.Play();
            _player.PlayMethods.Clear();
        }

        private async UniTask ClearTextBlocks()
        {
            if (_player.TextBlocks.Count == 0)
            {
                return;
            }

            foreach (TextBlock textBlock in _player.TextBlocks)
            {
                CGPlayer.PlayMethod exitMethod = textBlock.Exit;
                _player.PlayMethods.Add(exitMethod);
            }
            await _player.Play();
            _player.TextBlocks.Clear();
            _player.PlayMethods.Clear();
        }

        private async UniTask NextLineAfterInterval(float interval)
        {
            _intervalCts = new();
            if (interval >= 0f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: _intervalCts.Token);
                NextLine().Forget();
            }
            else if (_player.AutoPlay)
            {
                // TODO: Implement auto play
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: _intervalCts.Token);
                NextLine().Forget();
            }
        }
    }
}