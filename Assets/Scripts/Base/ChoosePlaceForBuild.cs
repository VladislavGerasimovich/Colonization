using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[RequireComponent(typeof(Base))]
public class ChoosePlaceForBuild : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Flag _flag;

    private RaycastHit _hit;
    private Flag _setFlag;

    public event UnityAction FlagSet;
    
    public Coroutine ChoosePlaceJob {  get; private set; }
    public Vector3 FlagPosition { get; private set; }

    public void StopChoosePlace()
    {
        StopCoroutine(ChoosePlaceJob);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ChoosePlaceJob = StartCoroutine(ChoosePlace());
    }

    public void DestroyFlag()
    {
        Destroy(_setFlag.gameObject);
    }

    private IEnumerator ChoosePlace()
    {
        while (enabled)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit);

            if (Input.GetMouseButtonDown(1))
            {
                if (_setFlag != null)
                {
                    _setFlag.transform.position = _hit.point;
                }

                if (_setFlag == null)
                {
                    _setFlag = Instantiate(_flag, _hit.point, _flag.transform.rotation);
                    FlagPosition = _hit.point;
                    FlagSet?.Invoke();
                }
            }

            yield return null;
        }
    }
}
