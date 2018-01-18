using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapViewer))]
public class MapViewerEditor : Editor {

    public override void OnInspectorGUI() {
        MapViewer mapView = (MapViewer)target;

        if (DrawDefaultInspector()) {
            if (mapView.autoUpdate)
                mapView.DrawMapInEditor();
        }

        if (GUILayout.Button("Generate"))
            mapView.DrawMapInEditor();
    }

}
