using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBase : MonoBehaviour
{
    [SerializeField] private Base _base;

    private List<Base> _bases;
    private Unit _unitBuilder;

    private void OnDisable()
    {
        foreach (Base mainBase in _bases)
        {
            mainBase.UnitCameToBuild -= StartCreate;
        }
    }

    private void Awake()
    {
        _bases = new List<Base>();
    }

    private void Start()
    {
        Create(new Vector3(110, 0, 60));
    }

    private void Create(Vector3 position)
    {
        Base mainBase = Instantiate(_base, position, _base.transform.rotation);
        mainBase.UnitCameToBuild += StartCreate;

        if(_unitBuilder != null)
        {
            _unitBuilder.transform.SetParent(mainBase.transform);
            mainBase.AddUnit(_unitBuilder);
        }

        _bases.Add(mainBase);
    }

    private void StartCreate(Vector3 position, Unit unit)
    {
        _unitBuilder = unit;
        Create(position);
    }
}
