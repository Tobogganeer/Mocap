using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recording")]
public class Recording : ScriptableObject
{
    public int fps = 30;
    public AnimationClip exportClip;
    [HideInInspector]
    public DEP_Bone[] DEPRECATED_bones;
    public List<Bone> bones;

    public void DEPRECATED_Clear()
    {
        foreach (DEP_Bone bone in DEPRECATED_bones)
        {
            bone.localRotations = new List<Quaternion>();
        }
    }

    public void Clear(Transform[] initWith)
    {
        bones.Clear();
        for (int i = 0; i < initWith.Length; i++)
        {
            bones.Add(new Bone(initWith[i].name));
        }
    }

    [System.Serializable]
    public class Bone
    {
        public string name;
        public List<Quaternion> localRotations;

        public Bone(string name)
        {
            this.name = name;
            localRotations = new List<Quaternion>();
        }
    }

    [System.Serializable]
    public class DEP_Bone
    {
        public Type type;
        //public List<Vector3> localPositions;
        public List<Quaternion> localRotations;

        public enum Type
        {
            LUpperArm,
            LForearm,
            LHand,
            RUpperArm,
            RForearm,
            RHand,
        }
    }
}
