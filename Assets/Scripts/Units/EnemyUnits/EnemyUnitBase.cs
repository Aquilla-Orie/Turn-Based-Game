using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EnemyUnitBase : UnitBase
{
    private protected Dictionary<Vector2Int, int> _availableTiles = new Dictionary<Vector2Int, int>();
    public virtual void TakeDamage(int amount)
    {
        TotalHealth -= amount;
        CountPerStack = (int)((TotalHealth + HealthPerUnit - 1) / HealthPerUnit);

        if (CountPerStack <= 0)
        {
            UnitManager.Instance.ResloveEnemyDeath(this);
            return;
        }
    }

    public virtual void AttackPlayer(PlayerUnitBase player)
    {
        GameManager.Instance.ChangeGameState(GameState.ActionOccuring);
    }
   
    public virtual void DeductTurn()
    {
        Turns--;

        if (Turns == 0)
        {
            IsTurnPlayed = true;
        }

        GridManager.Instance.TurnAllGridsOff();
        UnitManager.Instance.ResolveEnemyTurn();
    }

    IEnumerator AddArtificialDelay()
    {
        yield return null;
    }

    public virtual void DetermineNextMove()
    {
        //Get all available tiles for the unit
        _availableTiles.Clear();
        OccupiedTile.FindReachableCells(_availableTiles, OccupiedTile.TileCoords, MovementUnits);
        Debug.Log(_availableTiles.Count);

        System.Random rand = new System.Random();

        List<Tile> tiles = new List<Tile>();
        foreach (var coord in _availableTiles.Keys)
        {
            var t = GridManager.Instance.GetTileAtCoord(coord.x, coord.y);
            tiles.Add(t);
        }


        //Determine if there is a player unit on any of the available tiles and attacks if there is
        if (tiles.Any(t => t.OccupiedUnit != null && t.OccupiedUnit.Faction == Faction.PlayerUnit))
        {
            var attackPossibleTiles = tiles.Where(t => t.OccupiedUnit.Faction == Faction.PlayerUnit).ToList();
            var playerToAttack = (PlayerUnitBase)attackPossibleTiles[rand.Next(attackPossibleTiles.Count)].OccupiedUnit;
            AttackPlayer(playerToAttack);
            Debug.Log($"{name} is attacking unit {playerToAttack.UnitName}");
            return;
        }

        //else just move to one of the available tiles
        tiles[rand.Next(tiles.Count)].SetUnit(this, false);
        Debug.Log($"{name} is moving to {OccupiedTile.name}");
        DeductTurn();
    }

    public virtual void ExecuteAbility(PlayerUnitBase player = null)
    {

    }

    public virtual void ExecutePassive()
    {

    }
}
