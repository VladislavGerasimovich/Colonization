using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private int _point;

    private void Start()
    {
        _point = 1;
    }

    public void SetParents(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = new Vector3(_point, _point, _point);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
