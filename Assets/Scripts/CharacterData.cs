using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Character Data", order = 2)]
public class CharacterData : ScriptableObject
{
    [ReadOnly] public uint Id;
    [ReadOnly] public uint Cost;
    [ReadOnly] public float HitPoints;
    [ReadOnly] public float HitRate;
    [ReadOnly] public float Damage;
    [ReadOnly] public float Speed;
    [ReadOnly] public float Range;
    [ReadOnly] public uint Radius;
    [ReadOnly] public bool IsGroundTarget;
    [ReadOnly] public bool IsAirTarget;
    [ReadOnly] public uint NumberOfUnits;
    [ReadOnly] public float HideTimer;
    [ReadOnly] public bool CanMove;
    [ReadOnly] public bool CanRotate;
    [ReadOnly] public GameObject Prefab;
    [ReadOnly] public Color Color;

}
