using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace CG
{
    public class InputManager : MonoBehaviour
    {
        private CGPlayer _player;

        private void Start()
        {
            _player = FindObjectOfType<CGPlayer>();
            var gestureController = FindObjectOfType<GestureController>();
            if (gestureController != null)
            {
                gestureController.GestureDetected += OnGestureDetected;
            }
        }

        private void OnGestureDetected(object sender, GestureEventArgs e)
        {
            if (e.Type == GestureType.Click)
            {
                OnClicked();
            }
        }

        private void OnClicked()
        {
            switch (_player.PlayState)
            {
                case PlayState.Playing:
                    _player.Skip();
                    break;
                case PlayState.Waiting:
                    _player.Next();
                    break;
                case PlayState.Hiding:
                    _player.Show();
                    break;
                default:
                    Debug.Log("Nothing to do");
                    break;
            }
        }
    }
}