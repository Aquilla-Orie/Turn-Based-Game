using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;
    private List<PlayerUnitBase> _playerUnits;
    private List<EnemyUnitBase> _enemyUnits;

    public PlayerUnitBase SelectedPlayerUnit;
    public EnemyUnitBase SelectedEnemyUnit;

    private void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        _playerUnits = new List<PlayerUnitBase>();
        _enemyUnits = new List<EnemyUnitBase>();
    }

    public void SpawnPlayerUnits()
    {
        var playerUnits = _units.Where(u => u.Faction == Faction.PlayerUnit).ToList();

        SpawnKnightArcherUnit(playerUnits[0].UnitPrefab, playerUnits[1].UnitPrefab);

        SetPlayerUnitTurns();
    }

    public void SpawnEnemyUnits()
    {
        var enemyUnits = _units.Where(u => u.Faction == Faction.EnemyUnit).ToList();

        SpawnSkeletonUnit(enemyUnits[0].UnitPrefab, true);
        SpawnZombieUnit(enemyUnits[1].UnitPrefab);

        SetEnemyUnitTurns();
    }
    public void SpawnSkeletonUnit(UnitBase skeletonUnitPrefab, bool motherStack, int numPerStack = 50)
    {
        UnitBase spawnedSkeleton;
        Tile randomSpawnTile;
        //4 stacks of 50 Skeleton Units
        if (motherStack)
        {
            for (int i = 0; i < 5; i++)
            {
                spawnedSkeleton = Instantiate(skeletonUnitPrefab);
                randomSpawnTile = GridManager.Instance.GetEnemyUnitSpawnTile();
                randomSpawnTile.SetUnit(spawnedSkeleton, false);
                spawnedSkeleton.Init();
                spawnedSkeleton.transform.GetComponent<SkeletonUnit>().SetMotherStack(motherStack);
                AddEnemyUnit(spawnedSkeleton);
            }
            return;
        }

        spawnedSkeleton = Instantiate(skeletonUnitPrefab);
        randomSpawnTile = GridManager.Instance.GetEnemyUnitSpawnTile();
        randomSpawnTile.SetUnit(spawnedSkeleton, false);
        spawnedSkeleton.CountPerStack = numPerStack;
        spawnedSkeleton.transform.GetComponent<SkeletonUnit>().SetMotherStack(motherStack);
        spawnedSkeleton.Init();
        AddEnemyUnit(spawnedSkeleton);
    }

    public void AddEnemyUnit(UnitBase spawnedUnit)
    {
        _enemyUnits.Add((EnemyUnitBase)spawnedUnit);
    }

    private void SpawnZombieUnit(UnitBase zombieUnitPrefab)
    {
        var spawnedZombie = Instantiate(zombieUnitPrefab);
        var randomSpawnTile = GridManager.Instance.GetEnemyUnitSpawnTile();
        randomSpawnTile.SetUnit(spawnedZombie, false);
        spawnedZombie.Init();
        AddEnemyUnit(spawnedZombie);
    }


    //Spawning three stacks of 20 Knight Units around on stack of 40 Archer Units
    private void SpawnKnightArcherUnit(UnitBase archerUnitPrefab, UnitBase knightUnitPrefab)
    {
        //First Knight Unit Stack
        var spawnedKnight = Instantiate(knightUnitPrefab);
        var randomSpawnTile = GridManager.Instance.GetPlayerUnitSpawnTile();
        randomSpawnTile.SetUnit(spawnedKnight);
        spawnedKnight.Init();
        AddPlayerUnit(spawnedKnight);

        //Second Knight Unit Stack
        spawnedKnight = Instantiate(knightUnitPrefab);
        Vector2 coords = GridManager.Instance.CalculateHexCoord(randomSpawnTile.transform.position);
        randomSpawnTile = GridManager.Instance.GetTileAtCoord((int)coords.x + 1, (int)coords.y + 1);
        randomSpawnTile.SetUnit(spawnedKnight);
        spawnedKnight.Init();
        AddPlayerUnit(spawnedKnight);

        //Third Knight Unit Stack
        spawnedKnight = Instantiate(knightUnitPrefab);
        coords = GridManager.Instance.CalculateHexCoord(randomSpawnTile.transform.position);
        randomSpawnTile = GridManager.Instance.GetTileAtCoord((int)coords.x - 1, (int)coords.y + 1);
        randomSpawnTile.SetUnit(spawnedKnight);
        spawnedKnight.Init();
        AddPlayerUnit(spawnedKnight);

        //Archer Unit Stack
        var spawnedArcher = Instantiate(archerUnitPrefab);
        coords = GridManager.Instance.CalculateHexCoord(randomSpawnTile.transform.position);
        randomSpawnTile = GridManager.Instance.GetTileAtCoord((int)coords.x, (int)coords.y - 1);
        randomSpawnTile.SetUnit(spawnedArcher);
        spawnedArcher.Init();
        AddPlayerUnit(spawnedArcher);


    }

    public void AddPlayerUnit(UnitBase spawnedUnit)
    {
        _playerUnits.Add((PlayerUnitBase)spawnedUnit);
    }

    public void SetSelectedPlayerUnit(PlayerUnitBase unitBase)
    {
        SelectedPlayerUnit = unitBase;
        UIManager.Instance.ShowSelectedPlayerUnit(unitBase);
    }
    public void SetSelectedEnemyUnit(EnemyUnitBase unitBase)
    {
        SelectedEnemyUnit = unitBase;
        UIManager.Instance.ShowSelectedEnemyUnit(unitBase);
    }

    public void SetPlayerUnitTurns()
    {
        foreach (var unit in _playerUnits)
        {
            unit.Turns = 1;
        }
    }

    public void SetEnemyUnitTurns()
    {
        foreach (var unit in _enemyUnits)
        {
            unit.Turns = 1;
        }
    }

    //Checks if any player units still have available moves
    public void ResolvePlayerTurn()
    {
        GridManager.Instance.TurnAllGridsOff();

        var playerUnits = _playerUnits.Where(u => !u.IsTurnPlayed).Count();
        Debug.Log($"Available Player Units: {playerUnits}");
        if (playerUnits <= 0)
        {
            Debug.Log($"Switching To Enemy Turn");
            ResetEnemyUnitTurns(_enemyUnits);
            GameManager.Instance.ChangeGameState(GameState.EnemyTurn);
            return;
        }

    }

    public void ResolveEnemyTurn()
    {
        var enemyUnits = _enemyUnits.Where(u => !u.IsTurnPlayed).Count();
        Debug.Log($"Available Enemy Units: {enemyUnits}");
        if (enemyUnits <= 0)
        {
            Debug.Log($"Switching To Player Turn");
            GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
            ResetPlayerUnitTurns(_playerUnits);
            return;
        }

        var availableUnits = _enemyUnits.Where(u => !u.IsTurnPlayed);
        //Select enemy to move or attack if in range
        var enemySelected = availableUnits.OrderBy(o => Random.value).FirstOrDefault();
        enemySelected.DetermineNextMove();
    }

    private void ResetPlayerUnitTurns(List<PlayerUnitBase> units)
    {
        foreach (var unit in units)
        {
            unit.IsTurnPlayed = false;
        }
    }
    private void ResetEnemyUnitTurns(List<EnemyUnitBase> units)
    {
        foreach (var unit in units)
        {
            unit.IsTurnPlayed = false;
        }
    }

    public void ResloveEnemyDeath(EnemyUnitBase unitBase)
    {
        _enemyUnits.Remove(unitBase);
        Destroy(unitBase.gameObject);
        if (_enemyUnits.Count <= 0)
            GameManager.Instance.ChangeGameState(GameState.Victory);
    }
    public void ReslovePlayerDeath(PlayerUnitBase unitBase)
    {
        _playerUnits.Remove(unitBase);
        Destroy(unitBase.gameObject);
        if (_enemyUnits.Count <= 0)
            GameManager.Instance.ChangeGameState(GameState.Loss);
    }
}
