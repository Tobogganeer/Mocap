using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Animations.Rigging;

public class Mocap : MonoBehaviour
{
    [Header("Recording")]
    public Recording recording;
    public SteamVR_Action_Boolean recordButton;
    public Transform[] bonesToRecord;
    bool record;
    float recordTimer;

    [Header("Playback")]
    public Animator animator;
    public RigBuilder rigBuilder;
    //public bool preview;
    public int index;
    float playbackTimer;

    //[Header("Export")]
    //public AnimationClip exportClip;

    private void Start()
    {
        if (Application.isPlaying)
        {
            recordButton.onStateDown += ToggleOn;
            recordButton.onStateUp += ToggleOff;
        }
    }

    private void ToggleOn(SteamVR_Action_Boolean _, SteamVR_Input_Sources __)
    {
        Debug.Log("RECORDING STARTED");
        recording.Clear(bonesToRecord);
        record = true;
    }

    private void ToggleOff(SteamVR_Action_Boolean _, SteamVR_Input_Sources __)
    {
        Debug.Log("RECORDING ENDED");
        record = false;
    }

    // Record
    private void LateUpdate()
    {
        if (!record || !Application.isPlaying) return;

        recordTimer -= Time.deltaTime;

        if (recordTimer < 0)
        {
            recordTimer = 1f / recording.fps;

            for (int i = 0; i < bonesToRecord.Length; i++)
            {
                recording.bones[i].localRotations.Add(bonesToRecord[i].localRotation);
            }
        }
    }

    // Playback
    private void Update()
    {
        // Doesn't work great in editor
        /*
        if (!preview) return;

        if (recording == null || recording.bones == null || recording.bones.Count == 0 ||
            bonesToRecord == null || bonesToRecord.Length == 0) return;

        playbackTimer -= Time.deltaTime;

        if (playbackTimer < 0)
        {
            playbackTimer = 1f / recording.fps;
            AdvanceFrame();
        }
        */
    }

    public void AdvanceFrame()
    {
        index++;
        if (index >= recording.bones[0].localRotations.Count)
            index = 0;
        for (int i = 0; i < bonesToRecord.Length; i++)
        {
            Quaternion rot = recording.bones[i].localRotations[index];
            bonesToRecord[i].localRotation = rot;
        }
    }

    public void Export()
    {
        if (recording.bones == null || recording.bones.Count == 0 ||
            recording.bones[0].localRotations?.Count == 0)
        {
            Debug.LogWarning("Can't export empty recording!");
            return;
        }
        if (recording.exportClip == null)
        {
            Debug.LogWarning("Please assign a clip to the recording!");
            return;
        }

        int frames = recording.bones[0].localRotations.Count;
        recording.exportClip.ClearCurves();

        for(int b = 0; b < recording.bones.Count; b++)
        {
            var bone = recording.bones[b];
            Keyframe[] x = new Keyframe[frames];
            Keyframe[] y = new Keyframe[frames];
            Keyframe[] z = new Keyframe[frames];
            Keyframe[] w = new Keyframe[frames];

            for (int i = 0; i < frames; i++)
            {
                float totalTime = (float)frames / recording.fps;
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

            string name = GetPath(bonesToRecord[b]);
            recording.exportClip.SetCurve(name, typeof(Transform), "localRotation.x", xCurve);
            recording.exportClip.SetCurve(name, typeof(Transform), "localRotation.y", yCurve);
            recording.exportClip.SetCurve(name, typeof(Transform), "localRotation.z", zCurve);
            recording.exportClip.SetCurve(name, typeof(Transform), "localRotation.w", wCurve);
        }

        recording.exportClip.EnsureQuaternionContinuity();
    }

    public string GetPath(Transform bone)
    {
        Stack<Transform> parents = new Stack<Transform>();
        parents.Push(bone);
        while (parents.Peek().parent != null)
            parents.Push(parents.Peek().parent);
        string name = "";
        parents.Pop(); // Remove top - root not required
        while (parents.TryPop(out Transform res))
            name += res.name + "/";
        name = name.Substring(0, name.Length - 1); // Remove last slash
        return name;
    }
}
