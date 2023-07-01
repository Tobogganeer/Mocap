using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MocapRecorder : MonoBehaviour
{
    [Header("DEPRECATED - USE Mocap COMPONENT")]

    public SteamVR_Action_Boolean button;

    [Space]
    public Recording recording;
    bool record;

    float timer;

    public BoneBinding[] bones;

    private void Start()
    {
        button.onStateDown += ToggleOn;
        button.onStateUp += ToggleOff;
    }

    private void ToggleOff(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("RECORDING ENDED");
        record = false;
    }

    private void ToggleOn(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("RECORDING STARTED");
        recording.DEPRECATED_Clear();
        record = true;
    }

    private void LateUpdate()
    {
        if (!record) return;

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = 1f / recording.fps;
            foreach (BoneBinding bone in bones)
            {
                recording.DEPRECATED_bones[(int)bone.type].localRotations.Add(bone.transform.localRotation);
            }
        }
    }

    [System.Serializable]
    public class BoneBinding
    {
        public Transform transform;
        public Recording.DEP_Bone.Type type;

        public string GetPath()
        {
            Stack<Transform> parents = new Stack<Transform>();
            parents.Push(transform);
            while (parents.Peek().parent != null)
                parents.Push(parents.Peek().parent);
            string name = "";
            parents.Pop();
            while (parents.TryPop(out Transform res))
                name += res.name + "/";
            name = name.Substring(0, name.Length - 1); // Remove last /
            return name;
        }
    }
}

public static class BoneTypeExtensions
{
    public static string GetObjectName(this Recording.DEP_Bone.Type type)
    {
        return type switch
        {
            Recording.DEP_Bone.Type.LUpperArm => "upper_arm.L",
            Recording.DEP_Bone.Type.LForearm => "forearm.L",
            Recording.DEP_Bone.Type.LHand => "hand.L",
            Recording.DEP_Bone.Type.RUpperArm => "upper_arm.R",
            Recording.DEP_Bone.Type.RForearm => "forearm.R",
            Recording.DEP_Bone.Type.RHand => "hand.R",
            _ => throw new System.NotImplementedException(),
        };
    }
}