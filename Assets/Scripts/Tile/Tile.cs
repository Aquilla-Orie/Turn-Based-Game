using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Dictionary<Vector2Int, int> _reachableTilePositions = new Dictionary<Vector2Int, int>();
    public Vector2Int TileCoords 
    {
        get
        {
            return CalulateTileGridPosition();
        }
    }

    public string TileName;

    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _invalidHighlight;

    public bool IsWalkable;

    public UnitBase OccupiedUnit;
    public bool Walkable => IsWalkable && OccupiedUnit == null;

    private void OnMouseEnter()
    {
        SetHighlight(true);

        if (GameManager.Instance.GameState == GameState.PlayerTurn)
            UIManager.Instance.ShowTileInfo(this);

        if (GameManager.Instance.GameState == GameState.EnemyTurn)
            UIManager.Instance.ShowEnemyTileInfo(this);
    }

    private void OnMouseExit()
    {
        //if (_reachableTilePositions.ContainsKey(CalulateTileGridPosition())) return;

        SetHighlight(false);

        if (GameManager.Instance.GameState == GameState.PlayerTurn)
            UIManager.Instance.ShowTileInfo(null);
        if (GameManager.Instance.GameState == GameState.EnemyTurn)
            UIManager.Instance.ShowEnemyTileInfo(null);
    }

    public void SetHighlight(bool state)
    {
        _highlight.SetActive(state);
    }

    private void OnMouseDown()
    {

        if (GameManager.Instance.GameState == GameState.PlayerTurn) 
        {
            GetPlayerUnitInput();
        }

        if (GameManager.Instance.GameState == GameState.EnemyTurn) 
        {
            //GetEnemyUnitInput();
        }
    }


    private void GetPlayerUnitInput()
    {
        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction == Faction.PlayerUnit && !OccupiedUnit.IsTurnPlayed)
            {
                UnitManager.Instance.SetSelectedPlayerUnit((PlayerUnitBase)OccupiedUnit);
                FindReachableCells(_reachableTilePositions, TileCoords, OccupiedUnit.MovementUnits);
            }
            else
            {
                if (UnitManager.Instance.SelectedPlayerUnit != null)
                {
                    var enemy = (EnemyUnitBase)OccupiedUnit;
                    AbilityManager.Instance.PerformAbility(UnitManager.Instance.SelectedPlayerUnit, enemy);
                    UnitManager.Instance.SetSelectedPlayerUnit(null);
                    ClearReachableList(_reachableTilePositions);

                }
            }
        }
        else
        {
            if (UnitManager.Instance.SelectedPlayerUnit != null)
            {

                if (!Walkable || UnitManager.Instance.SelectedPlayerUnit.IsTurnPlayed/* || !_reachableTilePositions.ContainsKey(_tileCoords)*/)
                    return;
                GameManager.Instance.ChangeGameState(GameState.ActionOccuring);
                SetUnit(UnitManager.Instance.SelectedPlayerUnit);
                AbilityManager.Instance.PerformAbility(UnitManager.Instance.SelectedPlayerUnit);
                UnitManager.Instance.SelectedPlayerUnit.ExecutePassive();
                GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
                UnitManager.Instance.SelectedPlayerUnit.DeductTurn();
                UnitManager.Instance.SetSelectedPlayerUnit(null);
            }
        }
    }

    private void GetEnemyUnitInput()
    {
        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction == Faction.EnemyUnit && !OccupiedUnit.IsTurnPlayed)
            {
                UnitManager.Instance.SetSelectedEnemyUnit((EnemyUnitBase)OccupiedUnit);
                FindReachableCells(_reachableTilePositions, TileCoords, OccupiedUnit.MovementUnits);
            }
            else
            {
                if (UnitManager.Instance.SelectedEnemyUnit != null)
                {
                    var player = (PlayerUnitBase)OccupiedUnit;
                    AbilityManager.Instance.PerformEnemyAbility(UnitManager.Instance.SelectedEnemyUnit, player);
                    UnitManager.Instance.SetSelectedEnemyUnit(null);
                    ClearReachableList(_reachableTilePositions);

                }
            }
        }
        else
        {
            if (UnitManager.Instance.SelectedEnemyUnit != null)
            {

                if (!Walkable || UnitManager.Instance.SelectedEnemyUnit.IsTurnPlayed/* || !_reachableTilePositions.ContainsKey(_tileCoords)*/)
                    return;
                GameManager.Instance.ChangeGameState(GameState.ActionOccuring);
                SetUnit(UnitManager.Instance.SelectedEnemyUnit, false);
                AbilityManager.Instance.PerformEnemyAbility(UnitManager.Instance.SelectedEnemyUnit);
                UnitManager.Instance.SelectedEnemyUnit.ExecutePassive();
                GameManager.Instance.ChangeGameState(GameState.EnemyTurn);
                UnitManager.Instance.SelectedEnemyUnit.DeductTurn();
                UnitManager.Instance.SetSelectedEnemyUnit(null);
            }
        }
    }


    private Vector2Int CalulateTileGridPosition()
    {
        Vector2 tileCoords = GridManager.Instance.CalculateHexCoord(transform.position);
        Vector2Int tCoords = new Vector2Int((int)tileCoords.x, (int)tileCoords.y);
        return tCoords;
    }

    public void SetUnit(UnitBase unit, bool isOffset = true)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        float offset = isOffset ? .69282f / 2 : 0;
        var pos = new Vector2(transform.position.x, transform.position.y - offset);
        unit.transform.position = pos;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    // Simple record for book-keeping about our search in progress.
    struct PathfindingNode
    {
        public readonly Vector2Int position;
        public readonly int distance;

        public PathfindingNode(Vector2Int position, int distance)
        {
            this.position = position;
            this.distance = distance;
        }
    }

    // Stores the nodes at the frontier of our search.
    // The Queue type ensures we visit nodes in a breadth-first order.
    Queue<PathfindingNode> _openSet = new Queue<PathfindingNode>();

    // Populates a dictionary of reachable tiles.
    public void FindReachableCells(Dictionary<Vector2Int, int> reachable, Vector2Int startPoint, int maxSteps)
    {
        // Clear the results of any previous pathfinding.
        ClearReachableList(reachable);

        // Seed the pathfinding with our start point.
        reachable.Add(startPoint, 0);
        _openSet.Enqueue(new PathfindingNode(startPoint, 0));

        // As long as there are nodes in the frontier...
        while (_openSet.Count > 0)
        {
            // Pop the oldest one out of the collection.
            PathfindingNode parent = _openSet.Dequeue();

            // Check each neighbouring cell for reachability,
            // and add it to the collection if we can reach it.  
            float childDistance = parent.distance + 1;
            CheckChild(parent.position.x - 1, parent.position.y, (int)childDistance, maxSteps, reachable);
            CheckChild(parent.position.x + 1, parent.position.y, (int)childDistance, maxSteps, reachable);
            CheckChild(parent.position.x, parent.position.y - 1, (int)childDistance, maxSteps, reachable);
            CheckChild(parent.position.x, parent.position.y + 1, (int)childDistance, maxSteps, reachable);
        }


        foreach (var item in reachable)
        {
            Tile t = GridManager.Instance.GetTileAtCoord((int)item.Key.x, (int)item.Key.y);
            t._highlight.SetActive(true);
        }
    }

    private static void ClearReachableList(Dictionary<Vector2Int, int> reachable)
    {
        foreach (var item in reachable)
        {
            Tile t = GridManager.Instance.GetTileAtCoord((int)item.Key.x, (int)item.Key.y);
            t?._highlight.SetActive(false);
        }

        reachable.Clear();
    }

    void CheckChild(int x, int y, int distance, int maxDistance, Dictionary<Vector2Int, int> reachable)
    {
        var position = new Vector2Int(x, y);
        var tile = GridManager.Instance.GetTileAtCoord(x, y);

        // Exclude nodes that are out of bounds or not walkable.        
        if (GridManager.Instance.IsInBounds(position) == false || tile.Walkable == false)
        {
            return;
        }

        // Exclude nodes we've already visited by some other route.
        if (reachable.ContainsKey(position))
        {
            return;
        }

        // Aha! We found a new walkable node in range. Add it to our reachable set.
        reachable.Add(position, distance);


        // And if we have room to go further, add it to our frontier
        // to later explore its neighbouring nodes.
        if (distance < maxDistance)
            _openSet.Enqueue(new PathfindingNode(position, distance));
    }
}
