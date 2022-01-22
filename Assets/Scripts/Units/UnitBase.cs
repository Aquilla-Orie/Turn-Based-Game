using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public int CountPerStack;
    public int HealthPerUnit;
    public int TotalHealth;
    public int[] Damage = new int[2];
    public int Turns;
    public int MovementUnits;
    public bool IsTurnPlayed;
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;

    public void Init()
    {
        TotalHealth = CountPerStack * HealthPerUnit;
    }
}
