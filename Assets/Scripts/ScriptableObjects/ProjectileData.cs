using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Projectiles/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float hitRadius = 0.5f;
    public int impactDamage;
    public bool followTarget;
    public bool rotate = true;
    public bool angleFromGravity = false;
    [Space]
    public GameObject impactZone;
    [Space]
    public bool skipSameTypeAsSender = false;
    [Space]
    public AudioClip sound;
    public float volume = 1f;
}
