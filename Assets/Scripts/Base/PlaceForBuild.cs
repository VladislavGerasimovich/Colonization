using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class PlaceForBuild : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Flag _flag;

    private RaycastHit _hit;
    private Flag _setFlag;

    public Vector3 FlagPosition { get; private set; }
    public bool IsBaseClicked { get; private set; }
    public bool IsBuildingBase { get; private set; }


    private void Start()
    {
        StartCoroutine(ChoosePlace());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsBaseClicked = true;
    }

    public void DestroyFlag()
    {
        Destroy(_setFlag.gameObject);
    }

    public void SetStatus(bool isBaseClicked, bool isBuildingBase)
    {
        IsBaseClicked = isBaseClicked;
        IsBuildingBase = isBuildingBase;
    }

    private IEnumerator ChoosePlace()
    {
        while (true)
        {
            if (IsBaseClicked)
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit);

                if (Input.GetMouseButtonDown(1))
                {
                    IsBuildingBase = true;

                    if (_setFlag != null)
                    {
                        _setFlag.transform.position = _hit.point;
                    }

                    if (_setFlag == null)
                    {
                        _setFlag = Instantiate(_flag, _hit.point, _flag.transform.rotation);
                        FlagPosition = _hit.point;
                    }
                }
            }

            yield return null;
        }
    }
}
