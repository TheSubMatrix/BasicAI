using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    LaserProjectile projectile;
    [SerializeField]
    Transform m_firePoint;
    [SerializeField]
    float m_distnaceToShoot = 100;
    [SerializeField]
    uint m_damage = 20;
    Camera cam => Camera.main;
    Quaternion additionalWeaponRotation;
    private void Awake()
    {
        additionalWeaponRotation = transform.localRotation;
    }
    public void Fire()
    {
        Vector3 endPosition = m_firePoint.position + m_firePoint.transform.forward * m_distnaceToShoot;
        if (Physics.Raycast(m_firePoint.position, m_firePoint.transform.forward, out RaycastHit hitInfo, m_distnaceToShoot))
        {
            endPosition = hitInfo.point;
            IDamagable damagable = hitInfo.collider.transform.root.gameObject.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.Damage(m_damage);
                Debug.Log(hitInfo.collider.gameObject.name);
            }
        }
        LaserProjectile newProjectile = Instantiate(projectile, m_firePoint.position, m_firePoint.transform.rotation);
        newProjectile.OnFired(m_firePoint.position, endPosition);
    }
    private void Update()
    {
        RotateWeaponToMatchCameraForward();
    }
    void RotateWeaponToMatchCameraForward()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * m_distnaceToShoot, Color.green);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hitInfo, m_distnaceToShoot))
        {
            transform.LookAt(hitInfo.point);
            transform.localRotation *= additionalWeaponRotation;
        }
        else
        {
            transform.localRotation = additionalWeaponRotation;
        }
    }
}
