using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private GameObject _unit;
    [SerializeField] private Plantation _plantation;

    private int _whiteEnemies;
    private int _maxCountWhiteEnemies;
    private int _hypotheticalCounter;

    private List<GameObject> _units;
    private List<Enemy> _enemies;

    private int _countOfUnits;
    private Vector3 _unitStartPosition;
    private float _spreadPositionX;
    private float _spreadPositionY;
    private Unit _freeUnit;

    private void OnDisable()
    {
        foreach (var unit in _units)
        {
            unit.GetComponent<Unit>().BroughtMaterial -= IncreaseCount;
        }
    }

    private void Start()
    {
        _maxCountWhiteEnemies = 5;
        _enemies = new List<Enemy>();
        _units = new List<GameObject>();   
        _spreadPositionX = 10f;
        _spreadPositionY = 5f;
        _countOfUnits = 3;
        _unitStartPosition = new Vector3(transform.position.x + _spreadPositionX, transform.position.y, transform.position.z);
        CreateUnits();
        StartCoroutine(GiveOrder());
    }

    private void IncreaseCount()
    {
        _whiteEnemies++;
    }

    private void CreateUnits()
    {
        for (int i = 0; i < _countOfUnits; i++)
        {
            GameObject unit = Instantiate(_unit, _unitStartPosition, Quaternion.identity);
            unit.transform.SetParent(transform);
            unit.GetComponent<Unit>().MountStartPosition(_unitStartPosition);
            unit.GetComponent<Unit>().BroughtMaterial += IncreaseCount;
            _unitStartPosition.z += _spreadPositionY;
            _units.Add(unit);
        }
    }

    private IEnumerator GiveOrder()
    {
        while(true)
        {
            if(_enemies.Count == 0 && _hypotheticalCounter < _maxCountWhiteEnemies)
            {
                ScanPlantation();
            }

            if(_enemies.Count > 0)
            {
                if (TryGetUnit())
                {
                    _hypotheticalCounter++;
                    _freeUnit.StartMove(_enemies[0]);
                    _enemies.RemoveAt(0);
                }
            }

            yield return null;
        }
    }

    private bool TryGetUnit()
    {
        bool isUnit = false;

        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i].GetComponent<Unit>().CheckAvailability())
            {
                _freeUnit = _units[i].GetComponent<Unit>();
                isUnit = true;

                break;
            }
        }

        return isUnit;
    }

    private void ScanPlantation()
    {
        Enemy enemy = _plantation.TryGetEnemy();

        if(enemy == null)
        {
            return;
        }

        _enemies.Add(enemy);
    }
}
