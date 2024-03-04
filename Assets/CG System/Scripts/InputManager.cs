using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace CG
{
    public class InputManager : MonoBehaviour
    {
        CGManager _manager;

        private void Start()
        {
            _manager = FindObjectOfType<CGManager>();
            var gestureController = FindObjectOfType<GestureController>();
            if (gestureController != null)
            {
                gestureController.GestureDetected += OnGestureDetected;
            }
        }

        private void OnGestureDetected(object sender, GestureEventArgs e)
        {
            switch (e.Type)
            {
                case GestureType.Click:
                    Debug.Log("Click detected");
                    _manager.NextLine().Forget();
                    break;
                default:
                    Debug.Log("Unknown gesture detected");
                    break;
            }
        }
    }
}