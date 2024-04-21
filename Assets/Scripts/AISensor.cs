using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshCollider)), RequireComponent(typeof(Rigidbody))]
public class AISensor : MonoBehaviour
{
    public UnityEvent<GameObject> FoundNewObject = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> LostObject = new UnityEvent<GameObject>();
    [SerializeField][HideInInspector] bool m_shouldDrawGizmos = true;
    [SerializeField][HideInInspector] float m_sightDistance = 20;
    [SerializeField][HideInInspector] float m_horizontalSightAngle = 20;
    [SerializeField][HideInInspector] float m_verticalSightAngle = 20;
    [SerializeField]List<GameObject> m_objectsInSight = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!m_objectsInSight.Contains(other.gameObject))
        {
            FoundNewObject.Invoke(other.gameObject);
            m_objectsInSight.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (m_objectsInSight.Contains(other.gameObject))
        {
            LostObject.Invoke(other.gameObject);
            m_objectsInSight.Remove(other.gameObject);
        }
    }
    Mesh m_fovMesh;
    MeshFilter m_filter;
    MeshCollider m_collider;
    Rigidbody m_rigidBody;
    public void CheckComponents()
    {
        if(m_rigidBody == null)
        {
            m_rigidBody = GetComponent<Rigidbody>();
            m_rigidBody.useGravity = false;
            m_rigidBody.isKinematic = true;
        }
        if (m_filter == null)
        {
            m_filter = transform.GetComponent<MeshFilter>();
        }
        if (m_collider == null)
        {
            m_collider = transform.GetComponent<MeshCollider>();
            m_collider.convex = true;
            m_collider.isTrigger = true;
        }
        m_fovMesh = new Mesh();
        m_filter.mesh = m_fovMesh;
        CreateShape();
        m_collider.sharedMesh = m_fovMesh;
    }
    public void CreateShape()
    {
        Vector3 startingpoint = Vector3.zero;
        Vector3 farFOVCenter = startingpoint + Vector3.forward * m_sightDistance;


        Vector3 topRight = CalculateTopRightPoint(farFOVCenter);
        Vector3 topLeft = CalculateTopLeftPoint(farFOVCenter);
        Vector3 bottomRight = CalculateBottomRightPoint(farFOVCenter);
        Vector3 bottomLeft = CalculateBottomLeftPoint(farFOVCenter);
        Vector3[] verticies = new Vector3[]
        {
            startingpoint,
            topLeft,
            topRight,
            bottomLeft,
            bottomRight,
        };
        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 4,
            0, 4, 3,
            0, 3, 1,
            2, 1, 3,
            2, 3, 4
        };
        m_fovMesh.Clear();
        m_fovMesh.vertices = verticies;
        m_fovMesh.triangles = triangles;
        m_fovMesh.RecalculateNormals();
    }
    private void OnDrawGizmos()
    {
        if (m_shouldDrawGizmos)
        {
            CheckComponents();
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawMesh(m_fovMesh);
        }
    }
    Vector3 CalculateTopRightPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + ((Mathf.Sin(m_horizontalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.right) + ((Mathf.Sin(m_verticalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.up);
    }
    Vector3 CalculateTopLeftPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + ((Mathf.Sin(m_horizontalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.left) + ((Mathf.Sin(m_verticalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.up);
    }
    Vector3 CalculateBottomRightPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + ((Mathf.Sin(m_horizontalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.right) + ((Mathf.Sin(m_verticalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.down);
    }
    Vector3 CalculateBottomLeftPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + ((Mathf.Sin(m_horizontalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.left) + ((Mathf.Sin(m_verticalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.down);
    }
    bool CheckInRange<T>(T[,] arrayToCheck, Vector2Int coords)
    {
        if(arrayToCheck == null) return false;
        if(coords.x < arrayToCheck.GetLength(0) && coords.x >= 0 && coords.y < arrayToCheck.GetLength(1) && coords.y >= 0)
        {
            return true;
        }
        return false;
    }
}
[CustomEditor(typeof(AISensor))]
public class AISensorInspector : Editor
{
    SerializedProperty m_shouldDrawGizmos;
    SerializedProperty m_sightDistance;
    SerializedProperty m_horizontalSightAngle;
    SerializedProperty m_verticalSightAngle;
    private void OnEnable()
    {
        m_shouldDrawGizmos = serializedObject.FindProperty(nameof(m_shouldDrawGizmos));
        m_sightDistance = serializedObject.FindProperty(nameof(m_sightDistance));
        m_horizontalSightAngle = serializedObject.FindProperty(nameof(m_horizontalSightAngle));
        m_verticalSightAngle = serializedObject.FindProperty(nameof(m_verticalSightAngle));
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        AISensor scriptToUpdate = target as AISensor;
        EditorGUI.BeginChangeCheck();
        bool newshouldDrawGizmos = EditorGUILayout.Toggle("Should Draw Gizmos?", m_shouldDrawGizmos.boolValue);
        float newSightDistance = EditorGUILayout.FloatField("Sight Distance", m_sightDistance.floatValue);
        float newHorizontalSightAngle = EditorGUILayout.Slider("Horizontal Angle", m_horizontalSightAngle.floatValue, 0.01f, 180);
        float newVerticalSightAngle = EditorGUILayout.Slider("Horizontal Angle", m_verticalSightAngle.floatValue, 0.01f, 180);
        if (EditorGUI.EndChangeCheck())
        {
            m_shouldDrawGizmos.boolValue = newshouldDrawGizmos;
            m_sightDistance.floatValue = Mathf.Clamp(newSightDistance, 0.01f, float.MaxValue);
            m_horizontalSightAngle.floatValue = newHorizontalSightAngle;
            m_verticalSightAngle.floatValue = newVerticalSightAngle;
            serializedObject.ApplyModifiedProperties();
            scriptToUpdate.CheckComponents();
        }
    }
}

