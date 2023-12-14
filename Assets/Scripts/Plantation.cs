using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plantation : MonoBehaviour
{
    [SerializeField] private Resource _resourcePrefab;

    private Vector3 _startPosition;

    private int _randomPositionX;
    private int _randomPositionZ;

    private WaitForSeconds _delay;

    private List<Resource> _resource;
    private int _maxCountResources;

    private void Start()
    {
        _maxCountResources = 10;
        _resource = new List<Resource>();
        _delay = new WaitForSeconds(1);
        StartCoroutine(CreateResources());
    }

    public Resource GetResource()
    {
        if (_resource.Count == 0)
        {
            return null;
        }

        Resource resource = _resource[0];
        _resource.RemoveAt(0);

        return resource;
    }

    private void CreateResource()
    {
        GenerateRandomPosition();
        Resource resource = Instantiate(_resourcePrefab, _startPosition, Quaternion.identity, transform);
        _resource.Add(resource);
    }

    private void GenerateRandomPosition()
    {
        int _maxPositionX = 90;
        int _maxPositionZ = 60;
        _randomPositionX = Random.Range(0, _maxPositionX);
        _randomPositionZ = Random.Range(0, _maxPositionZ);
        _startPosition = transform.TransformPoint(_randomPositionX, 0, _randomPositionZ);
    }

    private IEnumerator CreateResources()
    {
        while (enabled)
        {
            if(_resource.Count != _maxCountResources)
            {
                CreateResource();
            }

            yield return _delay;
        }
    }
}
