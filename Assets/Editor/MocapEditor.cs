using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Mocap))]
public class MocapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Mocap mocap = (Mocap)target;

        if (GUILayout.Button("View Current Index"))
        {
            mocap.index--;
            mocap.AdvanceFrame();
        }
        if (GUILayout.Button("Transfer recording to anim clip"))
        {
            mocap.Export();
        }
        if (GUILayout.Button("Play"))
        {
            //mocap.animator.StartPlayback();
            //AnimationMode.StartAnimationMode();
        }
    }
}
