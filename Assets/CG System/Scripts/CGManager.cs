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
                if (_player.TextBlocks.Count > 0)
                {
                    foreach (TextBlock textBlock in _player.TextBlocks)
                    {
                        CGPlayer.PlayMethod exitMethod = textBlock.Exit;
                        _player.PlayMethods.Add(exitMethod);
                    }
                    await _player.Play();
                    _player.TextBlocks.Clear();
                    _player.PlayMethods.Clear();
                }

                _narrations = _scenes[_currentSceneIndex].GetComponentsInChildren<Narration>();
                _currentNarrationIndex = 0;

                CGPlayer.PlayMethod playMethod = _scenes[_currentSceneIndex++].Play;
                _player.PlayMethods.Add(playMethod);
                await _player.Play();
                _player.PlayMethods.Clear();
            }
            else if (_line.Type == LineType.Narration)
            {
                _narrations[_currentNarrationIndex].Text = _line.Text;

                CGPlayer.PlayMethod playMethod = _narrations[_currentNarrationIndex++].Play;
                _player.PlayMethods.Add(playMethod);
                await _player.Play();
                _player.PlayMethods.Clear();
            }
            else if (_line.Type == LineType.Dialog)
            {
                _dialog.Text = _line.Text;
                _dialog.SetImages(_line.DialogInfo);
                _dialog.SetImagesToChange(true);

                CGPlayer.PlayMethod playMethod = _dialog.Play;
                _player.PlayMethods.Add(playMethod);
                await _player.Play();
                _player.PlayMethods.Clear();
            }
            else
            {
                Debug.LogError("Unknown line type");
            }

            // 间隔时间大于等于 0 时，等待一段时间后继续
            if (_line.Interval >= 0f)
            {
                _intervalCts = new();
                await UniTask.Delay(TimeSpan.FromSeconds(_line.Interval), cancellationToken: _intervalCts.Token);
                NextLine().Forget();
            }
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
    }
}