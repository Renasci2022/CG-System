using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CG
{
    [RequireComponent(typeof(EventTrigger))]
    public class GestureController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private GestureType _gestureType = GestureType.Click;

        [SerializeField] private float _maxElapseTime = 1.5f; // 最大持续时间
        [SerializeField] private float _dragSpeedThreshold = 500f; // 速度阈值

        [ShowIf("_gestureType", GestureType.Rotate)]
        [SerializeField] private float _minCircleRadius = 10f; // 最小圆半径
        [ShowIf("_gestureType", GestureType.Rotate)]
        [SerializeField] private float _maxCircleRadius = 50f; // 最大圆半径
        [ShowIf("_gestureType", GestureType.Rotate)]
        [SerializeField] private float _minCircleAngle = 300f; // 最小圆角度

        private Vector2 _startPos;
        private Vector2 _currentPos;
        private float _startTime;

        private bool _isDrawingCircle = false;
        private float _currentAngle;
        private float _maxDistance;

        public event EventHandler<GestureEventArgs> GestureDetected;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPos = eventData.position;
            _startTime = Time.time;
            _currentPos = _startPos;

            if (_gestureType == GestureType.Rotate)
            {
                _isDrawingCircle = true;
                _currentAngle = 0f;
                _maxDistance = 0f;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDrawingCircle)
            {
                Vector2 deltaPos = eventData.position - _currentPos;
                float dragSpeed = deltaPos.magnitude / Time.deltaTime;
                if (dragSpeed > _dragSpeedThreshold)
                {
                    Vector2 direction = eventData.position - _startPos;
                    float angle = Vector2.Angle(Vector2.right, direction);
                    _currentAngle += angle;
                    _currentPos = eventData.position;
                }

                float distance = Vector2.Distance(eventData.position, _startPos);
                _maxDistance = Mathf.Max(_maxDistance, distance);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isDrawingCircle)
            {
                _isDrawingCircle = false;

                float elapseTime = Time.time - _startTime;

                Debug.Log($"Circle radius: {_maxDistance}, angle: {_currentAngle}, time: {elapseTime}");

                if (elapseTime < _maxElapseTime && _currentAngle >= _minCircleAngle &&
                    _maxDistance >= _minCircleRadius && _maxDistance <= _maxCircleRadius)
                {
                    Debug.Log($"Rotate detected on {gameObject.name} with radius: {_maxDistance}, angle: {_currentAngle}");
                    GestureDetected?.Invoke(this, new GestureEventArgs(GestureType.Rotate));
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_gestureType != GestureType.Click)
            {
                return;
            }

            Debug.Log($"Click detected on {gameObject.name}");
            GestureDetected?.Invoke(this, new GestureEventArgs(GestureType.Click));
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

    [EnumPaging]
    public enum GestureType
    {
        Click,
        Swipe,
        Pinch,
        Rotate
    }
}