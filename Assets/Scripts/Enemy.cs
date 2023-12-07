using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int _spreadPoint;

    private void Start()
    {
        _spreadPoint = 1;
    }

    public void MountParents(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = new Vector3(_spreadPoint, _spreadPoint, _spreadPoint);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
