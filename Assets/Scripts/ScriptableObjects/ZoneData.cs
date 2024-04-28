using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "ZoneData", menuName = "Projectiles/ZoneData")]
public class ZoneData : ScriptableObject
{
    public int damage;
    public float interval;
    public float radius;
    public float duration;
    public float speedModifier = 1f;
    public AudioClip sound;
}
