using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    private Plantation _plantation;

    private void Awake()
    {
        _plantation = GameObject.FindWithTag("Plantation").GetComponent<Plantation>();
    }

    public Resource Scan()
    {
        Resource resource = _plantation.GetResource();

        if (resource == null)
        {
            return null;
        }

        return resource;
    }
}
