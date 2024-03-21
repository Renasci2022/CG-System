using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
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

        private int _currentSceneIndex = 0;
        private int _currentNarrationIndex = 0;

        public async UniTask NextLine()
        {
            _line = _reader.GetNextLine();

            if (_line == null)
            {
                Debug.Log("End of the script");
                await ClearTextBlocks();
                await ClearScene(_scenes[_currentSceneIndex - 1]);
                return;
            }
            else
            {
                Debug.Log($"Type: {_line.Type}, Text: {_line.Text}");
            }

            _dialog.IsPlaying = _line.Type == LineType.Dialog;

            if (_line.Type == LineType.Scene)
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
                _dialog.DialogInfo = _line.DialogInfo;

                await ClearTextBlocks();
                await PlayTextBlock(_dialog);
            }
            else
            {
                Debug.LogError("Unknown line type");
            }

            _player.NextAfterInterval(_line.Interval).Forget();
        }

        private void Awake()
        {
            GameObject canvas = GameObject.Find("Canvas");
            _scenes = canvas.GetComponentsInChildren<Scene>(true);
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
            if (_currentSceneIndex != 0)
            {
                await ClearScene(_scenes[_currentSceneIndex - 1]);
            }
            CGPlayer.PlayMethod playMethod = scene.Enter;
            _player.PlayMethods.Add(playMethod);
            await _player.Play();
            _player.PlayMethods.Clear();
        }

        private async UniTask ClearScene(Scene scene)
        {
            CGPlayer.PlayMethod exitMethod = scene.Exit;
            _player.PlayMethods.Add(exitMethod);
            await _player.Play();
            _player.PlayMethods.Clear();
        }

        private async UniTask PlayTextBlock(TextBlock textBlock)
        {
            CGPlayer.PlayMethod playMethod = textBlock.Enter;
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
    }
}