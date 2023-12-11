using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Objects/Game Data", order = 1)]
public class GameData : ScriptableObject
{
    [ReadOnly] public float ElixerRate;
    [ReadOnly] public uint ElixerLimit;
    [ReadOnly] public uint TotalCharacters;
    [ReadOnly] public uint MaximumCharactersInDeck;
    [ReadOnly] public float Duration;
}
