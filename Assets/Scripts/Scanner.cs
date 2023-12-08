using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField] private Plantation _plantation;

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
