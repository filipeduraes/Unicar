using Unicar.UI.RidesScreen.View;
using UnityEngine;

namespace UI.RidesScreen.ViewModel
{
    [CreateAssetMenu(menuName = "Unicar/RideDetailsList", fileName = "Rides Details List", order = 0)]
    public class RideDetailsList : ScriptableObject
    {
        [SerializeField] private RideDetailsData[] rides;

        public RideDetailsData[] Rides => rides;
    }
}