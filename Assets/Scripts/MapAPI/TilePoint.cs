using System;
using UnityEngine;

namespace Unicar.MapAPI
{
    public readonly struct TilePoint
    {
        public readonly long x;
        public readonly long y;
        public readonly int zoom;

        public TilePoint(long x, long y, int zoom)
        {
            this.x = x;
            this.y = y;
            this.zoom = zoom;
        }

        public static TilePoint operator +(TilePoint aTilePoint, TilePoint bTilePoint)
        {
            return new TilePoint(aTilePoint.x + bTilePoint.x, aTilePoint.y + bTilePoint.y, aTilePoint.zoom);
        }
        
        public static TilePoint operator +(TilePoint aTilePoint, Vector2Int offset)
        {
            return new TilePoint(aTilePoint.x + offset.x, aTilePoint.y + offset.y, aTilePoint.zoom);
        }

        public override bool Equals(object obj)
        {
            return obj is TilePoint tilePoint && tilePoint.zoom == zoom && tilePoint.x == x && tilePoint.y == y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(zoom.ToString(), x, y);
        }
    }
}