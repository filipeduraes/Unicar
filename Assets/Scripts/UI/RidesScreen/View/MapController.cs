using Unicar.Inputs;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Unicar.UI.RidesScreen.View
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private MapDrawer mapDrawer;
        
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
            
            if(!touch0.isInProgress || !RectTransformUtility.RectangleContainsScreenPoint((RectTransform)mapDrawer.transform, touch0.position))
                return;
            
            TouchState touch1 = _mobileInputs.Touch.Touch1.ReadValue<TouchState>();
            
            if (touch1.isInProgress)
            {
                Vector2 previousTouch0Position = touch0.position - touch0.delta;
                Vector2 previousTouch1Position = touch1.position - touch1.delta;
                
                float initialDistance = (previousTouch0Position - previousTouch1Position).magnitude;
                float finalDistance = (touch0.position - touch1.position).magnitude;
                float differenceDistance = finalDistance - initialDistance;

                mapDrawer.AddZoom((1f - 1f / differenceDistance) * Time.deltaTime);
            }
            else if (touch0.delta.sqrMagnitude > 10f)
            {
                mapDrawer.MoveMap(touch0.delta);
            }
        }
    }
}