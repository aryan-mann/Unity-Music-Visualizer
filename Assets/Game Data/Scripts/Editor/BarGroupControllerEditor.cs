using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(BarGroupController))]
public class BarGroupControllerEditor : Editor {

    BarGroupController bgc;
    void OnEnable() {
        bgc = (BarGroupController) target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Play")) {
            if(bgc.Source == null || bgc.Source.isPlaying) { return; }
            bgc.Source.Play();
        }
        if(GUILayout.Button(bgc.Source.isPlaying ? "Pause" : "Resume")) {
            if(bgc.Source == null) { return; }

            if(bgc.Source.isPlaying) { bgc.Source.Pause(); }
            else { bgc.Source.UnPause(); } 
        }
        if(GUILayout.Button("Stop")) {
            if(bgc.Source == null || !bgc.Source.isPlaying) { return; }
            bgc.Source.Stop();
        }
        GUILayout.EndHorizontal();
        
    }

}
