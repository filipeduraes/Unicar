using TMPro;
using UnityEngine;

namespace Unicar.UI.RidesScreen.View
{
    public class RideDetails : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text riderName;
        [SerializeField] private TMP_Text course;
        [SerializeField] private TMP_Text timePeriod;
        [SerializeField] private RectTransform bestRouteLabel;

        [Header("Label")] 
        [SerializeField] private string label = "º Período";

        public void Populate(RideDetailsData rideDetails)
        {
            riderName.SetText(rideDetails.RiderName);
            course.SetText($"{rideDetails.CourseName} - {rideDetails.CoursePeriod}{label}");
            timePeriod.SetText($"{rideDetails.InitialHour} - {rideDetails.FinalHour}");
            bestRouteLabel.gameObject.SetActive(rideDetails.IsBestRoute);
        }
    }
}