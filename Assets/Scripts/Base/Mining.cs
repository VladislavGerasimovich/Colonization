using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Base))]
public class Mining : MonoBehaviour
{
    private Scanner _scanner;
    private Base _base;
    private int _maxCountResourcesExtracted;
    private List<Resource> _resource;

    public int Count { get; private set; }

    private void Start()
    {
        _maxCountResourcesExtracted = 5;
        _resource = new List<Resource>();
        _scanner = GetComponent<Scanner>();
        _base = GetComponent<Base>();
        StartCoroutine(Work());
    }

    public void SetCount(int count)
    {
        Count -= count
;   }

    private IEnumerator Work()
    {
        while (enabled)
        {
            if (_resource.Count == 0 && Count < _maxCountResourcesExtracted)
            {
                Resource resource = _scanner.Scan();

                if (resource != null)
                {
                    _resource.Add(resource);
                }
            }

            if (_resource.Count > 0)
            {
                if (_base.TryGetUnit(out Unit unit))
                {
                    Count++;
                    unit.StartMining(_resource[0]);
                    _resource.RemoveAt(0);
                }
            }

            yield return null;
        }
    }
}
