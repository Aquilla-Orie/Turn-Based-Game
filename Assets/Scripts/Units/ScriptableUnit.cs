using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Unit", menuName ="Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public UnitBase UnitPrefab;
}

public enum Faction
{
    PlayerUnit = 0,
    EnemyUnit = 1
}
