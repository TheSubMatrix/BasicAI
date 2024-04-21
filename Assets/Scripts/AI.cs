using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Health))]
public class AI : MonoBehaviour
{

    [SerializeField] List<Transform> m_patrolPoints = new List<Transform>();
    [SerializeField] AISensor sensor;

    Vector3[] m_patrolPositions;
    private NavMeshAgent m_agent;
    Vector3? m_desiredPosition;
    aiState myState = aiState.patrol;
    enum aiState
    {
        patrol,
        follow,
    }

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        if(m_patrolPoints.Count > 0)
        {
            m_patrolPositions = new Vector3[m_patrolPoints.Count];
            for (int i = 0; i < m_patrolPoints.Count; i++)
            {
                m_patrolPositions[i] = m_patrolPoints[i].position;
            }
        }
    }
    private void Update()
    {
        switch (myState)
        {
            case aiState.patrol:
                Vector3? newPosition;
                newPosition = GetNewPatrolPoint(m_desiredPosition, m_patrolPositions);
                if (m_agent != null && newPosition != m_desiredPosition)
                {
                    m_desiredPosition = newPosition;
                    m_agent.destination = (Vector3)m_desiredPosition;
                    Debug.Log(m_agent.destination);
                }
                break;
            case aiState.follow:
                break;
        }
    }
    Vector3 GetNewPatrolPoint(Vector3? currentDesiredPosition, params Vector3[] points)
    {
        if(points.Length <= 0 || m_agent == null) 
        {
            return (Vector3)currentDesiredPosition;
        }
        int indexOfOldPosition = Array.IndexOf(points, currentDesiredPosition);
        if (currentDesiredPosition == null)
        {
            return GetClosestPoint(points);
        }

        if (indexOfOldPosition != -1)
        {
            if(!m_agent.pathPending && !m_agent.hasPath)
            {
                return points[(indexOfOldPosition + 1) % points.Length];
            }
            else 
            {
                return points[indexOfOldPosition];
            }
        }
        else
        {
            return GetClosestPoint(points);
        }
    }
    public void OnDeath()
    {
        Destroy(gameObject);
    }
    Vector3 GetClosestPoint(Vector3[] points)
    {
        Vector3 closestpoint = points[0];
        foreach (Vector3 point in points)
        {
            if (Vector3.Distance(transform.position, point) < Vector3.Distance(transform.position, closestpoint))
            {
                closestpoint = point;
            }
        }
        return closestpoint;
    }
}
