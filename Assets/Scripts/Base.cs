using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;

    private Scanner _scanner;
    private int _resources;
    private int _maxCountResourcesExtracted;
    private int _count;

    private List<Unit> _units;
    private List<Resource> _resource;

    private int _countOfUnits;
    private Vector3 _unitStartPosition;
    private float _spreadPositionX;
    private float _spreadPositionY;
    private Unit _freeUnit;

    private void OnDisable()
    {
        foreach (var unit in _units)
        {
            unit.BroughtMaterial -= IncreaseCount;
        }
    }

    private void Start()
    {
        _scanner = GetComponent<Scanner>();
        _maxCountResourcesExtracted = 5;
        _resource = new List<Resource>();
        _units = new List<Unit>();   
        _spreadPositionX = 10f;
        _spreadPositionY = 5f;
        _countOfUnits = 3;
        _unitStartPosition = new Vector3(transform.position.x + _spreadPositionX, transform.position.y, transform.position.z);
        CreateUnits();
        StartCoroutine(GiveOrder());
    }

    private void IncreaseCount()
    {
        _resources++;
    }

    private void CreateUnits()
    {
        for (int i = 0; i < _countOfUnits; i++)
        {
            Unit unit = Instantiate(_unitPrefab, _unitStartPosition, Quaternion.identity, transform);
            unit.MountStartPosition(_unitStartPosition);
            unit.BroughtMaterial += IncreaseCount;
            _unitStartPosition.z += _spreadPositionY;
            _units.Add(unit);
        }
    }

    private IEnumerator GiveOrder()
    {
        while(true)
        {
            if(_resource.Count == 0 && _count < _maxCountResourcesExtracted)
            {
                Resource resource = _scanner.Scan();

                if(resource != null)
                {
                    _resource.Add(resource);
                }
            }

            if (_resource.Count > 0)
            {
                if (TryGetUnit(out Unit unit))
                {
                    _count++;
                    unit.StartMove(_resource[0]);
                    _resource.RemoveAt(0);
                }
            }

            yield return null;
        }
    }

    private bool TryGetUnit(out Unit unit)
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i].GetComponent<Unit>().IsBusy == false)
            {
                unit = _units[i];

                return true;
            }
        }

        unit = null;
        return false;
    }
}
