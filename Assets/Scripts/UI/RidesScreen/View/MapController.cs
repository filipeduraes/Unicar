using Unicar.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Unicar.UI.RidesScreen.View
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private MapDrawer mapDrawer;
        [SerializeField] private float zoomUpdateSpeed = 0.5f;
        
        private MobileInputs _mobileInputs;
        private bool _checkTouch;
        private float _lastZoomUpdateTime;
        private Vector2 _lastMoveDelta;

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
            
            TouchState touch0 = _mobileInputs.Touch.Touch0.ReadValue<TouchState>();
            
            if (Vector2.SqrMagnitude(touch0.delta - _lastMoveDelta) > 10f)
            {
                mapDrawer.MoveMap(touch0.delta);
                _lastMoveDelta = touch0.delta;
            }
            
            if (_mobileInputs.Touch.Touch1.IsInProgress())
            {
                TouchState touch1 = _mobileInputs.Touch.Touch1.ReadValue<TouchState>();

                if (touch1.delta.sqrMagnitude > 10f)
                {
                    Vector2 touch0InitialPosition = touch0.position - touch0.delta;
                    Vector2 touch1InitialPosition = touch1.position - touch1.delta;

                    Vector2 touch1ToTouch0 = touch0InitialPosition - touch1InitialPosition;
                    float touchDot = Vector2.Dot(touch1ToTouch0, touch1.delta);

                    if (Mathf.Abs(touchDot) > 0.2f)
                        mapDrawer.AddZoom(-Mathf.Sign(touchDot) * Time.deltaTime * zoomUpdateSpeed);
                }
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