using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private bool IsBusy;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _speed = 6f;
    private bool _isAchieved;

    public bool CheckAvailability()
    {
        return IsBusy == false;
    }

    public void AchieveGoal()
    {
        _isAchieved = true;
    }

    public void StartMove(Enemy target)
    {
        IsBusy = true;
        _targetPosition = target.transform.position;
        StartCoroutine(Move());
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

            if(transform.position == _targetPosition)
            {
                _targetPosition = _startPosition;
            }
            if(_isAchieved == true)
            {
                _targetPosition = _startPosition;
                _isAchieved = false;
            }

            yield return null;
        }
    }
}
