using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ChangeGameState(GameState.GenerateGrid);
    }

    public void ChangeGameState(GameState newState)
    {
        GameState = newState;

        switch (newState)
        {
            case GameState.GenerateGrid:
                ResolveGridGeneration();
                break;
            case GameState.SpawnPlayerCharacters:
                ResolvePlayerUnitSpawning();
                break;
            case GameState.SpawnEnemyCharacters:
                ResolveEnemyUnitSpawning();
                break;
            case GameState.PlayerTurn:
                ResolvePlayerTurn();
                break;
            case GameState.EnemyTurn:
                ResolveEnemyTurn();
                break;
            case GameState.ActionOccuring:
                ResolveActionOccuring();
                break;
            case GameState.Victory:
                ResolveVictory();
                break;
            case GameState.Loss:
                ResolveLoss();
                break;
            default:
                break;
        }
    }

    private void ResolveGridGeneration()
    {
        GridManager.Instance.Init();
        ChangeGameState(GameState.SpawnPlayerCharacters);
    }

    private void ResolvePlayerUnitSpawning()
    {
        UnitManager.Instance.SpawnPlayerUnits();
        ChangeGameState(GameState.SpawnEnemyCharacters);
    }

    private void ResolveEnemyUnitSpawning()
    {
        UnitManager.Instance.SpawnEnemyUnits();
        ChangeGameState(GameState.PlayerTurn);
    }

    private void ResolvePlayerTurn()
    {
        
    }

    private void ResolveEnemyTurn()
    {
        UnitManager.Instance.ResolveEnemyTurn();
    }

    private void ResolveActionOccuring()
    {
        
    }

    private void ResolveVictory()
    {
        throw new NotImplementedException();
    }

    private void ResolveLoss()
    {
        throw new NotImplementedException();
    }
}

public enum GameState
{
    GenerateGrid = 0,
    SpawnPlayerCharacters = 1,
    SpawnEnemyCharacters = 2,
    PlayerTurn = 3,
    EnemyTurn = 4,
    ActionOccuring = 5,
    Victory = 6,
    Loss = 7
}
