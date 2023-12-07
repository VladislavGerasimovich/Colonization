using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plantation : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;

    private Vector3 _startPosition;

    private int _maxPositionX;
    private int _maxPositionZ;
    private int _randomPositionX;
    private int _randomPositionZ;

    private int _wait;
    private object _delay;

    private List<Enemy> _enemies;

    private void Start()
    {
        _enemies = new List<Enemy>();
        _wait = 3;
        _delay = new WaitForSeconds(_wait);

        StartCoroutine(CreateEnemies());
    }

    public Enemy TryGetEnemy()
    {
        Enemy[] enemies = _enemies.ToArray();

        if (enemies.Length == 0)
        {
            return null;
        }

        _enemies.RemoveAt(0);
        return enemies[0];
    }

    private IEnumerator CreateEnemies()
    {
        while (true)
        {
            CreateEnemy();

            yield return _delay;
        }
    }

    private void CreateEnemy()
    {
        GenerateRandomPosition();
        GameObject enemy = Instantiate(_enemy, transform.position, Quaternion.identity);
        enemy.transform.SetParent(transform);
        enemy.transform.localPosition = _startPosition;
        _enemies.Add(enemy.GetComponent<Enemy>());
    }

    private void GenerateRandomPosition()
    {
        _maxPositionX = 90;
        _maxPositionZ = 60;
        _randomPositionX = Random.Range(0, _maxPositionX);
        _randomPositionZ = Random.Range(0, _maxPositionZ);
        _startPosition = new Vector3(_randomPositionX, 0, _randomPositionZ);
    }
}
