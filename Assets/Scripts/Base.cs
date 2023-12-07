using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private GameObject _unit;
    [SerializeField] private Plantation _plantation;

    private List<GameObject> _units;
    private List<Enemy> _enemies;

    private int _countOfUnit;
    private Vector3 _unitStartPosition;
    private float _spreadPositionX;
    private float _spreadPositionY;
    private Unit _freeUnit;

    private void Start()
    {
        _enemies = new List<Enemy>();
        _units = new List<GameObject>();   
        _spreadPositionX = 10f;
        _spreadPositionY = 5f;
        _countOfUnit = 3;
        _unitStartPosition = new Vector3(transform.position.x + _spreadPositionX, transform.position.y, transform.position.z);
        CreateUnits();
        StartCoroutine(GiveOrder());
    }

    private void CreateUnits()
    {
        for (int i = 0; i < _countOfUnit; i++)
        {
            GameObject unit = Instantiate(_unit, _unitStartPosition, Quaternion.identity);
            unit.transform.SetParent(transform);
            unit.GetComponent<Unit>().MountStartPosition(_unitStartPosition);
            _unitStartPosition.z += _spreadPositionY;
            _units.Add(unit);
        }
    }

    private IEnumerator GiveOrder()
    {
        while(true)
        {
            ScanPlantation();

            if (_enemies.Count > 0)
            {
                if (TryGetUnit())
                {
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
