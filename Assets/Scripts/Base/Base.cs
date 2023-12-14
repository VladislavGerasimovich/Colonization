using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[RequireComponent(typeof(Mining))]
[RequireComponent(typeof(ChoosePlaceForBuild))]
public class Base : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private PlaceForUnits _placeForUnits;
    [SerializeField] private Vector3 _displacementVector;

    private ChoosePlaceForBuild _placeForBuild;
    private Mining _mining;
    private Unit _unitBuilder;

    private Vector3 _unitStartPosition;
    private int _unitCost;
    private int _baseCost;
    private bool _isBuildingBase;

    private int _maxCountOfUnits;
    private List<Unit> _units;
    private int _resources;

    public event UnityAction<Vector3, Unit> UnitCameToBuild;

    private void OnEnable()
    {
        _placeForBuild.FlagSet += StartSendUnitToBuild;
    }

    private void OnDisable()
    {
        _placeForBuild.FlagSet -= StartSendUnitToBuild;

        foreach (var unit in _units)
        {
            unit.BroughtMaterial -= IncreaseResources;
        }
    }

    private void Awake()
    {
        _placeForBuild = GetComponent<ChoosePlaceForBuild>();
        _units = new List<Unit>();   
    }

    private void Start()
    {
        _unitStartPosition = _placeForUnits.transform.position + _displacementVector;
        _mining = GetComponent<Mining>();
        _baseCost = 5;
        _unitCost = 3;
        _maxCountOfUnits = 5;
        CreateUnits(3);
        StartCoroutine(Work());
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
        unit.BroughtMaterial += IncreaseResources;
    }

    private void StartSendUnitToBuild()
    {
        _isBuildingBase = true;
    }

    private void IncreaseResources()
    {
        _resources++;
    }

    private void CreateUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Unit unit = Instantiate(_unitPrefab, _unitStartPosition, Quaternion.identity, transform);
            unit.MountStartPosition(_unitStartPosition);
            _unitStartPosition += _displacementVector;
            unit.BroughtMaterial += IncreaseResources;
            _units.Add(unit);
        }
    }
    
    private void StartCreateBase()
    {
        _unitBuilder.CameToBuild -= StartCreateBase;
        StopCoroutine(_placeForBuild.ChoosePlaceJob);
        UnitCameToBuild?.Invoke(_placeForBuild.FlagPosition, _unitBuilder);
        _placeForBuild.DestroyFlag();
    }

    private IEnumerator Work()
    {
        while (enabled)
        {
            if (_isBuildingBase == false && _resources >= _unitCost && _units.Count < _maxCountOfUnits)
            {
                _mining.SetCount(_unitCost);
                _resources -= _unitCost;
                CreateUnits(1);
            }

            if (_isBuildingBase == true && _resources >= _baseCost)
            {
                _isBuildingBase = false;
                _resources -= _baseCost;
                StartCoroutine(SendUnitToBuild());
            }

            yield return null;
        }
    }

    private IEnumerator SendUnitToBuild()
    {
        while (enabled)
        {
            if (TryGetUnit(out Unit unit))
            {
                _placeForBuild.StopChoosePlace();
                unit.BroughtMaterial -= IncreaseResources;
                _mining.SetCount(_baseCost);
                _units.Remove(unit);
                _unitBuilder = unit;
                _unitBuilder.StartCreateBase(_placeForBuild.FlagPosition);
                _unitBuilder.CameToBuild += StartCreateBase;
                yield break;
            }

            yield return null;
        }
    }
}