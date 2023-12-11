using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Development : MonoBehaviour
{
    [SerializeField] private Base _base;

    private List<Base> _bases;
    private bool _isCanBuild;
    private Unit _unitBuilder;

    private Vector3 _positionOfBase;

    private void OnDisable()
    {
        foreach (Base mainBase in _bases)
        {
            mainBase.UnitCameToBuild -= StartCreateBase;
        }
    }

    private void Awake()
    {
        _bases = new List<Base>();
    }

    private void Start()
    {
        _positionOfBase = new Vector3(110, 0, 60);
        CreateMainBase();
        StartCoroutine(CreateBase());
    }

    private IEnumerator CreateBase()
    {
        while (true)
        {
            if (_isCanBuild)
            {
                CreateMainBase();
                _isCanBuild = false;
            }

            yield return null;
        }
    }

    private void CreateMainBase()
    {
        Base mainBase = Instantiate(_base, _positionOfBase, _base.transform.rotation);
        mainBase.UnitCameToBuild += StartCreateBase;

        if(_unitBuilder != null)
        {
            _unitBuilder.transform.SetParent(mainBase.transform);
            mainBase.AddUnit(_unitBuilder);
        }

        _bases.Add(mainBase);
    }

    private void StartCreateBase(Vector3 position, Unit unit)
    {
        _unitBuilder = unit;
        _positionOfBase = position;
        _isCanBuild = true;
    }
}
