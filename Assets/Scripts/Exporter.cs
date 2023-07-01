using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exporter : MonoBehaviour
{
    [Header("DEPRECATED - USE Mocap COMPONENT")]

    public AnimationClip clip;
    public MocapPlayback playback;

    [ContextMenu("Export")]
    void Export()
    {
        int frames = playback.recording.DEPRECATED_bones[0].localRotations.Count;
        //playback.index = frames;
        //playback.AdvanceFrame(); // Set to frame 0;
        clip.ClearCurves();

        //for (int i = 0; i < frames; i++)
        //{
        //    clip.SampleAnimation(playback.gameObject, )
        //}

        foreach (var bone in playback.recording.DEPRECATED_bones)
        {
            Keyframe[] x = new Keyframe[frames];
            Keyframe[] y = new Keyframe[frames];
            Keyframe[] z = new Keyframe[frames];
            Keyframe[] w = new Keyframe[frames];

            for (int i = 0; i < frames; i++)
            {
                float totalTime = (float)frames / playback.recording.fps;
                float fac = (float)i / frames * totalTime;
                Quaternion rot = bone.localRotations[i];
                x[i] = new Keyframe(fac, rot.x);
                y[i] = new Keyframe(fac, rot.y);
                z[i] = new Keyframe(fac, rot.z);
                w[i] = new Keyframe(fac, rot.w);
            }

            AnimationCurve xCurve = new AnimationCurve(x);
            AnimationCurve yCurve = new AnimationCurve(y);
            AnimationCurve zCurve = new AnimationCurve(z);
            AnimationCurve wCurve = new AnimationCurve(w);

            string name = playback.bones[(int)bone.type].GetPath();
            clip.SetCurve(name, typeof(Transform), "localRotation.x", xCurve);
            clip.SetCurve(name, typeof(Transform), "localRotation.y", yCurve);
            clip.SetCurve(name, typeof(Transform), "localRotation.z", zCurve);
            clip.SetCurve(name, typeof(Transform), "localRotation.w", wCurve);
        }

        clip.EnsureQuaternionContinuity();
    }
}
