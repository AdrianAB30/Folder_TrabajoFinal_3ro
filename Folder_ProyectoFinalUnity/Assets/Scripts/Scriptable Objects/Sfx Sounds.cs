using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sfx Sounds", menuName = "ScriptableObjects/Sfx Sounds", order = 3)]

public class SfxSounds : ScriptableObject
{
    public AudioClip[] soundSfx;
}
