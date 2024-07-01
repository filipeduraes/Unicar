using UnityEngine;

namespace Unicar.UI.RidesScreen.View
{
    public class RidesView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private RideDetails detailsPrefab;

        public void Populate(RideDetailsData[] detailsData)
        {
            foreach (RideDetailsData data in detailsData)
            {
                RideDetails details = Instantiate(detailsPrefab, content);
                details.Populate(data);
            }
        }
    }
}