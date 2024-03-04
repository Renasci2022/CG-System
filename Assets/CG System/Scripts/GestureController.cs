using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CG
{
    public class GestureController : MonoBehaviour, IPointerClickHandler
    {
        public event EventHandler<GestureEventArgs> GestureDetected;

        public void OnPointerClick(PointerEventData eventData)
        {
            GestureDetected?.Invoke(this, new GestureEventArgs(GestureType.Click));
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