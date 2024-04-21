using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class LaserProjectile : MonoBehaviour
{
    LineRenderer m_lineRenderer;
    [SerializeField] float m_projectileLifetime = 1;
    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
    }
    public void OnFired(Vector3 startPoint, Vector3 endPoint)
    {
        m_lineRenderer.SetPosition(0, startPoint);
        m_lineRenderer.SetPosition(1, endPoint);
        m_lineRenderer.enabled = true;
        StartCoroutine(DestroyProjectile());
    }
    IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(m_projectileLifetime);
        Destroy(gameObject);
    }
}
