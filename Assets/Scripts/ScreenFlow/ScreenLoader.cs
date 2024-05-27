using UnityEngine;

namespace Unicar.ScreenFlow
{
    public class ScreenLoader : MonoBehaviour
    {
        [SerializeField] private int requestedIndex = -1;
        
        public void SendLoadRequest()
        {
            ScreenManager.LoadRequestedScreen(requestedIndex);
        }
    }
}