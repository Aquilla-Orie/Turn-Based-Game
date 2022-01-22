using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _selectedPlayerUnitObject;
    [SerializeField] private GameObject _playerTileObject;
    [SerializeField] private GameObject _playerTileUnitObject;



    [SerializeField] private GameObject _selectedEnemyUnitObject;
    [SerializeField] private GameObject _enemyTileObject;
    [SerializeField] private GameObject _enemyTileUnitObject;

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _playerTileObject.SetActive(false);
            _playerTileUnitObject.SetActive(false);
            return;
        }

        _playerTileObject.GetComponentInChildren<TMP_Text>().text = tile.TileName;
        _playerTileObject.SetActive(true);

        if (tile.OccupiedUnit)
        {
            _playerTileUnitObject.GetComponentInChildren<TMP_Text>().text = tile.OccupiedUnit.UnitName;
            _playerTileUnitObject.SetActive(true);
        }
    }
    public void ShowEnemyTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _enemyTileObject.SetActive(false);
            _enemyTileUnitObject.SetActive(false);
            return;
        }

        _enemyTileObject.GetComponentInChildren<TMP_Text>().text = tile.TileName;
        _enemyTileObject.SetActive(true);

        if (tile.OccupiedUnit)
        {
            _enemyTileUnitObject.GetComponentInChildren<TMP_Text>().text = tile.OccupiedUnit.UnitName;
            _enemyTileUnitObject.SetActive(true);
        }
    }

    public void ShowSelectedPlayerUnit(PlayerUnitBase playerUnit)
    {
        if (playerUnit == null)
        {
            _selectedPlayerUnitObject.SetActive(false);
            return;
        }

        _selectedPlayerUnitObject.GetComponentInChildren<TMP_Text>().text = playerUnit.UnitName;
        _selectedPlayerUnitObject.SetActive(true);
    }

    public void ShowSelectedEnemyUnit(EnemyUnitBase enemyUnit)
    {
        if (enemyUnit == null)
        {
            _selectedEnemyUnitObject.SetActive(false);
            return;
        }

        _selectedEnemyUnitObject.GetComponentInChildren<TMP_Text>().text = enemyUnit.UnitName;
        _selectedEnemyUnitObject.SetActive(true);
    }



    public void ActivatePlayerAbility()
    {
        AbilityManager.Instance.IsAbilitySelected = true;
    }
    public void MovePlayerUnit()
    {
        AbilityManager.Instance.IsAbilitySelected = false;
    }

    public void SkipPlayerTurn()
    {
        UnitManager.Instance.SelectedPlayerUnit.DeductTurn();
        UnitManager.Instance.SetSelectedPlayerUnit(null);
    }

    public void ActivateEnemyAbility()
    {
        AbilityManager.Instance.IsAbilitySelected = true;
    }
    public void MoveEnemyUnit()
    {
        AbilityManager.Instance.IsAbilitySelected = false;
    }

    public void SkipEnemyTurn()
    {
        UnitManager.Instance.SelectedEnemyUnit.DeductTurn();
        UnitManager.Instance.SetSelectedEnemyUnit(null);
    }


}
