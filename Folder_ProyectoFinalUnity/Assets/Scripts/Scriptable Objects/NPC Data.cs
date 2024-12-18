using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC Data", menuName = "ScriptableObjects/Npc Data", order = 2)]
public class NPCData : ScriptableObject
{
    public float watingTime;
    public float forceRotateNpc;
    public string nameCharacter;
    [TextArea(5, 5)]
    public string dialogue;
    public Sprite imageCharacter;
}
