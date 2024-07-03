using Unicar.Inputs;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Unicar.UI.RidesScreen.View
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private MapDrawer mapDrawer;
        [SerializeField] private RectTransform controlsArea;
        
        private MobileInputs _mobileInputs;
        private Vector2 _touch0StartPosition;
        private Vector2 _touch1StartPosition;
        
        private void Awake()
        {
            _mobileInputs = new MobileInputs();
            _mobileInputs.Enable();
        }

        private void OnDestroy()
        {
            _mobileInputs.Dispose();
        }

        private void Update()
        {
            TouchState touch0 = _mobileInputs.Touch.Touch0.ReadValue<TouchState>();
            
            if(!touch0.isInProgress || !RectTransformUtility.RectangleContainsScreenPoint(controlsArea, touch0.position))
                return;
            
            TouchState touch1 = _mobileInputs.Touch.Touch1.ReadValue<TouchState>();
            
            if (touch1.isInProgress)
            {
                Vector2 previousTouch0Position = touch0.position - touch0.delta;
                Vector2 previousTouch1Position = touch1.position - touch1.delta;
                
                Vector2 normalizedPreviousTouch0Position = new(previousTouch0Position.x / Screen.width, previousTouch0Position.y / Screen.height);
                Vector2 normalizedPreviousTouch1Position = new(previousTouch1Position.x / Screen.width, previousTouch1Position.y / Screen.height);
                
                Vector2 normalizedTouch0Position = new(touch0.position.x / Screen.width, touch0.position.y / Screen.height);
                Vector2 normalizedTouch1Position = new(touch1.position.x / Screen.width, touch1.position.y / Screen.height);
                
                float initialDistance = (normalizedPreviousTouch0Position - normalizedPreviousTouch1Position).magnitude;
                float finalDistance = (normalizedTouch0Position - normalizedTouch1Position).magnitude;
                float differenceDistance = finalDistance - initialDistance;

                if (Mathf.Abs(differenceDistance) > 0.01f)
                    mapDrawer.AddZoom(differenceDistance * 5.0f);
            }
            else if (touch0.delta.sqrMagnitude > 10f)
            {
                mapDrawer.MoveMap(touch0.delta);
            }
        }
    }
}