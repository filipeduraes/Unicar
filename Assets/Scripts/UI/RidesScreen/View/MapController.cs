using Unicar.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unicar.UI.RidesScreen.View
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private MapDrawer mapDrawer;
        [SerializeField] private float zoomUpdateSpeed = 0.5f;
        
        private MobileInputs _mobileInputs;
        private bool _checkTouch;
        private float _lastZoomUpdateTime;

        private void Awake()
        {
            _mobileInputs = new MobileInputs();
            _mobileInputs.Enable();
        }

        private void OnDestroy()
        {
            _mobileInputs.Dispose();
        }
        
        private void OnEnable()
        {
            _mobileInputs.Touch.Touch0.started += StartCheckingTouch;
            _mobileInputs.Touch.Touch0.canceled += StopCheckingTouch;
        }

        private void OnDisable()
        {
            _mobileInputs.Touch.Touch0.started -= StartCheckingTouch;
            _mobileInputs.Touch.Touch0.canceled -= StopCheckingTouch;
        }

        private void Update()
        {
            if(!_checkTouch)
                return;

            Touch touch0 = _mobileInputs.Touch.Touch0.ReadValue<Touch>();
            
            if (_mobileInputs.Touch.Touch1.IsInProgress())
            {
                //Pinch
                Touch touch1 = _mobileInputs.Touch.Touch1.ReadValue<Touch>();
                Vector2 touch0InitialPosition = touch0.position - touch0.deltaPosition;
                Vector2 touch1InitialPosition = touch1.position - touch1.deltaPosition;

                Vector2 touch1ToTouch0 = touch0InitialPosition - touch1InitialPosition;
                float touchDot = Vector2.Dot(touch1ToTouch0, touch1.deltaPosition);

                if (Mathf.Abs(touchDot) > 0.2f && Time.time - _lastZoomUpdateTime > zoomUpdateSpeed)
                {
                    mapDrawer.AddZoom((int) Mathf.Sign(touchDot));
                    _lastZoomUpdateTime = Time.time;
                }
            }
            else
            {
                mapDrawer.MoveMap(touch0.deltaPosition);
            }
        }

        private void StartCheckingTouch(InputAction.CallbackContext context)
        {
            _checkTouch = true;
        }
        
        private void StopCheckingTouch(InputAction.CallbackContext context)
        {
            _checkTouch = false;
        }
    }
}