using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CG
{
    public class GestureController : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click detected!");
            CGManager manager = GameObject.Find("CGManager").GetComponent<CGManager>();
            manager.NextLine().Forget();
        }
    }
}