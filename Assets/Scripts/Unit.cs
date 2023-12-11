using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class Unit : MonoBehaviour
{
    private Coroutine _miningJob;
    private Coroutine _createBaseJob;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _speed;
    private bool _isAchieved;
    private float _permissibleValue;

    private Resource _resource;

    public event UnityAction BroughtMaterial;
    public event UnityAction CameToBuild;

    public bool IsBusy { get; private set; }

    private void Start()
    {
        _speed = 30f;
        _permissibleValue = 0.3f;
    }

    public void StartMining(Resource resource)
    {
        _resource = resource;
        IsBusy = true;
        _targetPosition = _resource.transform.position;
        _miningJob = StartCoroutine(Mining());
    }

    public void StartCreateBase(Vector3 position)
    {
        IsBusy = true;
        _targetPosition = position;
        _createBaseJob = StartCoroutine(CreateBase());
    }

    public void MountStartPosition(Vector3 position)
    {
        _startPosition = position;
    }

    private IEnumerator Mining()
    {
        while (true)
        {
            Move(_targetPosition);

            if(Vector3.Distance(_targetPosition, transform.position) < _permissibleValue)
            {
                _isAchieved = true;
                _targetPosition = _startPosition;
                _resource.SetParents(transform);
            }

            if(_isAchieved == true && Vector3.Distance(transform.position, _startPosition) < _permissibleValue)
            {
                IsBusy = false;
                _isAchieved = false;
                GatheringResources();
                StopCoroutine(_miningJob);
            }

            yield return null;
        }
    }

    private IEnumerator CreateBase()
    {
        while (true)
        {
            Move(_targetPosition);

            if (Vector3.Distance(_targetPosition, transform.position) < _permissibleValue)
            {
                IsBusy = false;
                CameToBuild?.Invoke();
                StopCoroutine(_createBaseJob);
            }

            yield return null;
        }
    }

    private void Move(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);
    }

    private void GatheringResources()
    {
        BroughtMaterial?.Invoke();
        _resource.Die();
    }
}
