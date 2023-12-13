using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Mining))]
[RequireComponent(typeof(PlaceForBuild))]
public class Base : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;

    private PlaceForBuild _placeForBuild;
    private Mining _mining;

    private int _resources;

    private int _unitCost;
    private int _baseCost;

    private int _maxCountOfUnits;
    private List<Unit> _units;

    private Vector3 _unitStartPosition;
    private float _spreadPositionX;
    private float _spreadPositionZ;
    private Unit _unitBuilder;

    public event UnityAction<Vector3, Unit> UnitCameToBuild;

    private void OnDisable()
    {
        foreach (var unit in _units)
        {
            unit.BroughtMaterial -= IncreaseCount;
        }
    }

    private void Awake()
    {
        _units = new List<Unit>();   
    }

    private void Start()
    {
        _placeForBuild = GetComponent<PlaceForBuild>();
        _mining = GetComponent<Mining>();
        _baseCost = 5;
        _unitCost = 3;
        _maxCountOfUnits = 5;
        _spreadPositionX = 10f;
        _spreadPositionZ = 5f;
        _unitStartPosition = new Vector3(transform.position.x + _spreadPositionX, transform.position.y, transform.position.z);
        CreateUnits(3);
        StartCoroutine(CreateUnit());
        StartCoroutine(CreateBase());
    }

    public bool TryGetUnit(out Unit unit)
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i].IsBusy == false)
            {
                unit = _units[i];

                return true;
            }
        }

        unit = null;
        return false;
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
        unit.BroughtMaterial += IncreaseCount;
    }

    private void IncreaseCount()
    {
        _resources++;
    }

    private void CreateUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Unit unit = Instantiate(_unitPrefab, _unitStartPosition, Quaternion.identity, transform);
            unit.MountStartPosition(_unitStartPosition);
            unit.BroughtMaterial += IncreaseCount;
            _unitStartPosition.z += _spreadPositionZ;
            _units.Add(unit);
        }
    }

    private IEnumerator CreateBase()
    {
        while (true)
        {
            if (_placeForBuild.IsBuildingBase)
            {
                if (_resources >= _baseCost)
                {
                    if (TryGetUnit(out Unit unit))
                    {
                        _placeForBuild.SetStatus(false, false);
                        unit.BroughtMaterial -= IncreaseCount;
                        _mining.SetCount(_baseCost);
                        _resources -= _baseCost;
                        _units.Remove(unit);
                        _unitBuilder = unit;
                        _unitBuilder.StartCreateBase(_placeForBuild.FlagPosition);
                        _unitBuilder.CameToBuild += StartCreateBase;
                    }
                }
            }

            yield return null;
        }
    }
    
    private void StartCreateBase()
    {
        _unitBuilder.CameToBuild -= StartCreateBase;
        UnitCameToBuild?.Invoke(_unitBuilder.transform.position, _unitBuilder);
        _placeForBuild.DestroyFlag();
    }

    private IEnumerator CreateUnit()
    {
        while (true)
        {
            if (_placeForBuild.IsBuildingBase == false)
            {
                if(_units.Count < _maxCountOfUnits)
                {
                    if(_resources >= _unitCost)
                    {
                        _mining.SetCount(_unitCost);
                        _resources -= _unitCost;
                        CreateUnits(1);
                    }
                }
            }

            yield return null;
        }
    }
}
