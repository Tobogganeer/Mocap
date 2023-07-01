using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MocapPlayback : MonoBehaviour
{
    [Header("DEPRECATED - USE Mocap COMPONENT")]

    public Recording recording;

    public MocapRecorder.BoneBinding[] bones;

    public bool play;
    float timer;
    public int index;

    private void Update()
    {
        if (!play) return;

        if (recording == null || bones == null || bones.Length < 6) return;

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = 1f / recording.fps;
            AdvanceFrame();
        }
    }

    public void AdvanceFrame()
    {
        index++;
        if (index >= recording.DEPRECATED_bones[0].localRotations.Count)
            index = 0;
        foreach (MocapRecorder.BoneBinding bone in bones)
        {
            Quaternion rot = recording.DEPRECATED_bones[(int)bone.type].localRotations[index];
            bone.transform.localRotation = rot;
        }
    }
}
