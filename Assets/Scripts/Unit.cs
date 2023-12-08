using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    public event UnityAction BroughtMaterial;

    private bool _isBusy;

    private Coroutine MoveJob;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _speed;
    private bool _isAchieved;
    private float _permissibleValue;

    private Enemy _enemy;

    private void Start()
    {
        _speed = 15f;
        _permissibleValue = 0.3f;
    }

    public bool CheckAvailability()
    {
        return _isBusy == false;
    }

    public void StartMove(Enemy enemy)
    {
        _enemy = enemy;
        _isBusy = true;
        _targetPosition = _enemy.transform.position;
        MoveJob = StartCoroutine(Move());
    }

    public void MountStartPosition(Vector3 position)
    {
        _startPosition = position;
    }

    private void AchieveGoal()
    {
        _isAchieved = true;
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
                _enemy.MountParents(transform);
            }

            if(_isAchieved == true && transform.position == _startPosition)
            {
                _isBusy = false;
                _isAchieved = false;
                GatheringEnemies();
                StopCoroutine(MoveJob);
            }

            yield return null;
        }
    }

    private void GatheringEnemies()
    {
        BroughtMaterial?.Invoke();
        _enemy.Die();
    }
}
