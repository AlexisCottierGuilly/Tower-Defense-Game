using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Monster
{
    Goblin,
    Troll
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Waves/WaveData")]
public class WaveData : ScriptableObject
{
    public List<WavePart> waveParts;
}

[System.Serializable]
public class WavePart
{
    public Monster monster;
    public int amount;
    public float interval=1f;
}
