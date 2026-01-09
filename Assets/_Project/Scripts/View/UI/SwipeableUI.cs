using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ARMarker
{

    public class SwipeableUI : MonoBehaviour, 
        IPointerDownHandler, IPointerUpHandler, IDragHandler
    {

        [SerializeField]
        private float swipeThreshold = 30f; // How far the user must drag to be considered a swipe

        [Space]

        [SerializeField]
        private UnityEvent OnSwipeUp;

        [SerializeField]
        private UnityEvent OnSwipeDown;

        [SerializeField]
        private UnityEvent OnSwipeLeft;

        [SerializeField]
        private UnityEvent OnSwipeRight;

        private bool isPressed = false;
        private Vector2 startPointerPosition;

        private bool isSwipedDown = false;

        public bool IsSwipedDown() => isSwipedDown;

        public void RegisterSwipeListener(Action listener, SwipeDirection direction)
        {
            if (listener == null)
            {
                return;
            }

            switch (direction)
            {
                case SwipeDirection.Left:
                    {
                        OnSwipeLeft.AddListener(listener.Invoke);
                        break;
                    }
                case SwipeDirection.Right:
                    {
                        OnSwipeRight.AddListener(listener.Invoke);
                        break;
                    }
                case SwipeDirection.Up:
                    {
                        OnSwipeUp.AddListener(listener.Invoke);
                        break;
                    }
                case SwipeDirection.Down:
                default:
                    {
                        OnSwipeDown.AddListener(listener.Invoke);
                        break;
                    }
            }
        }

        public void ManuallyTriggerSwipe(SwipeDirection direction)
        {
            switch (direction)
            {
                case SwipeDirection.Left:
                    {
                        OnSwipeLeft?.Invoke();
                        break;
                    }
                case SwipeDirection.Right:
                    {
                        OnSwipeRight?.Invoke();
                        break;
                    }
                case SwipeDirection.Up:
                    {
                        OnSwipeUp?.Invoke();
                        break;
                    }
                case SwipeDirection.Down:
                default:
                    {
                        OnSwipeDown?.Invoke();
                        break;
                    }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            isSwipedDown = false;
            startPointerPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isPressed)
            {
                return;
            }

            Vector2 dragVector = eventData.position - startPointerPosition;

            if (dragVector.magnitude >= swipeThreshold)
            {
                if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
                {
                    // Horizontal swipe
                    if (dragVector.x > 0)
                    {
                        OnSwipeRight?.Invoke();
                    }
                    else
                    {
                        OnSwipeLeft?.Invoke();
                    }
                }
                else
                {
                    // Vertical swipe
                    if (dragVector.y > 0)
                    {
                        OnSwipeUp?.Invoke();
                    }
                    else
                    {
                        OnSwipeDown?.Invoke();
                    }
                }

                // Optional: prevent firing multiple times per press
                isPressed = false;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
        }
    }

}