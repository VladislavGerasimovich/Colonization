using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    private Coroutine _moveJob;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _speed;
    private bool _isAchieved;
    private float _permissibleValue;

    private Resource _resource;

    public event UnityAction BroughtMaterial;

    public bool IsBusy { get; private set; }

    private void Start()
    {
        _speed = 15f;
        _permissibleValue = 0.3f;
    }

    public void StartMove(Resource resource)
    {
        _resource = resource;
        IsBusy = true;
        _targetPosition = _resource.transform.position;
        _moveJob = StartCoroutine(Move());
    }

    public void MountStartPosition(Vector3 position)
    {
        _startPosition = position;
    }

    private IEnumerator Move()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);

            if(Vector3.Distance(_targetPosition, transform.position) < _permissibleValue)
            {
                _isAchieved = true;
                _targetPosition = _startPosition;
                _resource.SetParents(transform);
            }

            if(_isAchieved == true && transform.position == _startPosition)
            {
                IsBusy = false;
                _isAchieved = false;
                GatheringResources();
                StopCoroutine(_moveJob);
            }

            yield return null;
        }
    }

    private void GatheringResources()
    {
        BroughtMaterial?.Invoke();
        _resource.Die();
    }
}
