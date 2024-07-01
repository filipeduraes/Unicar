using Unicar.MapAPI;
using UnityEngine.UI;

namespace Unicar.UI.RidesScreen.View
{
    public struct MapCell
    {
        public RawImage Image;
        public TilePoint TilePoint;

        public void SetTilePoint(TilePoint tilePoint)
        {
            TilePoint = tilePoint;
        }
    }
}