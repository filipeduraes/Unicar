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
        private bool _checkMove;
        private bool _checkZoom;
        private Vector2 _currentZoomTouch0Position;
        private Vector2 _currentZoomTouch1Position;
        
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
            _mobileInputs.Touch.Touch1.started += StartSecondTouch;
            _mobileInputs.Touch.Touch1.canceled += StopSecondTouch;
        }

        private void OnDisable()
        {
            _mobileInputs.Touch.Touch0.started -= StartCheckingTouch;
            _mobileInputs.Touch.Touch0.canceled -= StopCheckingTouch;
            _mobileInputs.Touch.Touch1.started -= StartSecondTouch;
            _mobileInputs.Touch.Touch1.canceled -= StopSecondTouch;
        }

        private void Update()
        {
            if(!_checkMove)
                return;
            
            TouchState touch0 = _mobileInputs.Touch.Touch0.ReadValue<TouchState>();
            
            if (_checkZoom)
            {
                TouchState touch1 = _mobileInputs.Touch.Touch1.ReadValue<TouchState>();

                float initialDistance = (_currentZoomTouch0Position - _currentZoomTouch1Position).magnitude;
                float finalDistance = (touch0.position - touch1.position).magnitude;
                float differenceDistance = finalDistance - initialDistance;

                if (Mathf.Abs(differenceDistance) > 5f)
                {
                    mapDrawer.AddZoom(differenceDistance);
                    _currentZoomTouch0Position = touch0.position;
                    _currentZoomTouch1Position = touch1.position;
                }
            }
            else if (touch0.delta.sqrMagnitude > 10f)
            {
                mapDrawer.MoveMap(touch0.delta);
            }
        }

        private void StartCheckingTouch(InputAction.CallbackContext context)
        {
            _currentZoomTouch0Position = context.ReadValue<TouchState>().startPosition;
            _checkMove = true;
        }
        
        private void StopCheckingTouch(InputAction.CallbackContext context)
        {
            _checkMove = false;
        }
        
        private void StartSecondTouch(InputAction.CallbackContext context)
        {
            _currentZoomTouch1Position = context.ReadValue<TouchState>().startPosition;
            _checkZoom = true;
        }
        
        private void StopSecondTouch(InputAction.CallbackContext context)
        {
            _checkZoom = false;
        }
    }
}