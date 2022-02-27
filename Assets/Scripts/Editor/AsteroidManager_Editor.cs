using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroidManager))]
public class AsteroidManager_Editor : Editor
{

    AsteroidManager manager;
    private void OnEnable()
    {
        manager = (AsteroidManager)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Asteroids"))
        {
            manager.GenerateAsteroids();
        }
        if(GUILayout.Button("Clear All Asteroids"))
        {
            manager.ClearAsteroids();
        }
    }

    private void OnSceneGUI()
    {
        foreach(GameObject asteroid in manager.asteroids)
        {
            if(asteroid != null)
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc(asteroid.transform.position, (Camera.current.transform.position - asteroid.transform.position).normalized, asteroid.transform.localScale.x);
            }
        }
    }
}
