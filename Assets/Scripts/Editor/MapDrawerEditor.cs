using Unicar.UI.RidesScreen.View;
using UnityEditor;
using UnityEngine;

namespace Unicar.Editor
{
    [CustomEditor(typeof(MapDrawer))]
    public class MapDrawerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (target is MapDrawer mapDrawer)
            {
                if (GUILayout.Button("Add Zoom"))
                {
                    mapDrawer.AddZoom(0.2f);
                }
                
                if (GUILayout.Button("Remove Zoom"))
                {
                    mapDrawer.AddZoom(-0.2f);
                }
            }
        }
    }
}