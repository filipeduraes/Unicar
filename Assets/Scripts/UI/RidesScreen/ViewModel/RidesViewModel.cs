using Unicar.UI.RidesScreen.View;
using UnityEngine;

namespace UI.RidesScreen.ViewModel
{
    public class RidesViewModel : MonoBehaviour
    {
        [SerializeField] private RidesView ridesView;
        [SerializeField] private RideDetailsList rideDetails;

        private void Awake()
        {
            ridesView.Populate(rideDetails.Rides);
        }
    }
}