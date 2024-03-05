using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CG
{
    [RequireComponent(typeof(EventTrigger))]
    public class GestureController : MonoBehaviour, IPointerClickHandler
    {
        public GestureType _gestureType = GestureType.Click;

        public event EventHandler<GestureEventArgs> GestureDetected;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_gestureType != GestureType.Click)
            {
                return;
            }

            Debug.Log($"Click detected on {gameObject.name}");
            GestureDetected?.Invoke(this, new GestureEventArgs(_gestureType));
        }

        private void Start()
        {
            InputManager inputManager = FindObjectOfType<InputManager>();
            GestureDetected += inputManager.OnGestureDetected;
        }
    }

    public class GestureEventArgs : EventArgs
    {
        public GestureType Type { get; private set; }

        public GestureEventArgs(GestureType type)
        {
            Type = type;
        }
    }

    public enum GestureType
    {
        Click,
        Swipe,
        Pinch,
        Rotate
    }
}