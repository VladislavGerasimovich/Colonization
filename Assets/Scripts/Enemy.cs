using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Unit unit))
        {
            transform.SetParent(unit.transform);
            transform.localPosition = new Vector3(5f, 10f, 5f);
            unit.AchieveGoal();
            Debug.Log("соприкоснулись");
        }
    }
}
