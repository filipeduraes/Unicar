using UnityEngine;
using UnityEngine.UI;

namespace Unicar.UI.Utilities
{
    public class SafeArea : MonoBehaviour
    {
        [SerializeField] private CanvasScaler canvasScaler;
        private float _bottomUnits;
        private float _topUnits;
        
        private void Start()
        {
            ApplyVerticalSafeArea();
        }
            
        public void ApplyVerticalSafeArea()
        {
            float bottomPixels = Screen.safeArea.y;
            float topPixel = Screen.currentResolution.height - (Screen.safeArea.y + Screen.safeArea.height);
        
            float bottomRatio = bottomPixels / Screen.currentResolution.height;
            float topRatio = topPixel / Screen.currentResolution.height;
        
            Vector2 referenceResolution = canvasScaler.referenceResolution;
            _bottomUnits = referenceResolution.y * bottomRatio;
            _topUnits = referenceResolution.y * topRatio;
        
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, _bottomUnits);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -_topUnits);
        }
    }
}
