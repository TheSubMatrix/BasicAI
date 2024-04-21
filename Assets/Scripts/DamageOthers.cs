using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageOthers : MonoBehaviour
{
    uint damage = 20;
    private void OnCollisionEnter(Collision collision)
    {
        IDamagable damagable = collision.collider.transform.root.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.Damage(damage);
            Debug.Log("Found");
        }
    }
}
