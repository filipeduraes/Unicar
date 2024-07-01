using System;

namespace Unicar.UI.RidesScreen.View
{
    [Serializable]
    public struct RideDetailsData
    {
        public string RiderName;
        public string CourseName;
        public string InitialHour;
        public string FinalHour;
        public int CoursePeriod;
        public bool IsBestRoute;
    }
}