using CG;
using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CG
{
    public struct Dialog
    {
        public string dialogBox;
        public string nameStamp;
        public string expression;
        public string text;
    }

    public class DialogPlayer : TextBlock
    {
        private int _currentDialogIndex = 0;

        private Dialog[] _dialogs;
        private Dialog _currentDialog;

        private Image _dialogBox;
        private Image _nameStamp;
        private Image _expression;
        private TextMeshPro _text;

        public override void HideBlock()
        {
            _dialogBox.color = Color.clear;
            _nameStamp.color = Color.clear;
            _expression.color = Color.clear;
            _text.color = Color.clear;
        }

        public override void ShowBlock()
        {
            _dialogBox.color = Color.white;
            _nameStamp.color = Color.white;
            _expression.color = Color.white;
            _text.color = Color.black;
        }

        public override UniTask PlayBlock(CancellationToken cancellationToken, bool fastForward = false)
        {
            throw new System.NotImplementedException();
        }

        public override UniTask ExitBlock(CancellationToken cancellationToken, bool fastForward = false)
        {
            throw new System.NotImplementedException();
        }

        private new void Start()
        {
            gameObject.SetActive(false);
        }
    }
}