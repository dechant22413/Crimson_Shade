using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    //SoundCLip
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
}

[Serializable]
public class SoundPatch
{
    public string name;
    public List<Sound> sounds = new List<Sound>();
}

[Serializable]
public class SoundGroup
{
    public string groupName;
    public List<Sound> sounds = new List<Sound>();
    public List<SoundPatch> patches = new List<SoundPatch>();
}
