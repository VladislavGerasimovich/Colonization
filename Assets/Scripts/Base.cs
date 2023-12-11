using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Base : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private Flag _flag;

    private RaycastHit _hit;
    private Flag _setFlag;

    private Scanner _scanner;
    private int _resources;
    private int _maxCountResourcesExtracted;
    private int _count;

    private int _unitCost;
    private int _baseCost;

    private int _maxCountOfUnits;
    private List<Unit> _units;
    private List<Resource> _resource;

    private Vector3 _unitStartPosition;
    private float _spreadPositionX;
    private float _spreadPositionZ;
    private Unit _unitBuilder;
    private bool _isBaseClicked;
    private bool _isBuildingBase;

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
        _baseCost = 5;
        _unitCost = 3;
        _maxCountOfUnits = 5;
        _scanner = GetComponent<Scanner>();
        _maxCountResourcesExtracted = 5;
        _resource = new List<Resource>();
        _spreadPositionX = 10f;
        _spreadPositionZ = 5f;
        _unitStartPosition = new Vector3(transform.position.x + _spreadPositionX, transform.position.y, transform.position.z);
        CreateUnits(3);
        StartCoroutine(Mining());
        StartCoroutine(ChoosePlace());
        StartCoroutine(CreateUnit());
        StartCoroutine(CreateBase());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isBaseClicked = true;
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

    private IEnumerator ChoosePlace()
    {
        while (true)
        {
            if (_isBaseClicked)
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit);

                if (Input.GetMouseButtonDown(1))
                {
                    _isBuildingBase = true;

                    if (_setFlag != null)
                    {
                        _setFlag.transform.position = _hit.point;
                    }

                    if (_setFlag == null)
                    {
                        _setFlag = Instantiate(_flag, _hit.point, _flag.transform.rotation);
                    }
                }
            }

            yield return null;
        }
    }

    private IEnumerator CreateBase()
    {
        while (true)
        {
            if (_isBuildingBase)
            {
                if (_resources >= _baseCost)
                {
                    if (TryGetUnit(out Unit unit))
                    {
                        _isBaseClicked = false;
                        _isBuildingBase = false;
                        unit.BroughtMaterial -= IncreaseCount;
                        _count -= _baseCost;
                        _resources -= _baseCost;
                        _units.Remove(unit);
                        _unitBuilder = unit;
                        _unitBuilder.StartCreateBase(_setFlag.transform.position);
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
        Destroy(_setFlag.gameObject);
    }

    private IEnumerator CreateUnit()
    {
        while (true)
        {
            if (_isBuildingBase == false)
            {
                if(_units.Count < _maxCountOfUnits)
                {
                    if(_resources >= _unitCost)
                    {
                        _count -= _unitCost;
                        _resources -= _unitCost;
                        CreateUnits(1);
                    }
                }
            }

            yield return null;
        }
    }

    private IEnumerator Mining()
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
                    unit.StartMining(_resource[0]);
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
