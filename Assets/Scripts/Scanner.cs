using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField] private Plantation _plantation;

    public Enemy Scan()
    {
        Enemy enemy = _plantation.TryGetEnemy();

        if (enemy == null)
        {
            return null;
        }

        return enemy;
    }
}
