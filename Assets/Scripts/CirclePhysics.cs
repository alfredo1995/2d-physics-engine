using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CirclePhysics : MonoBehaviour
{
    public float radius = 1f;
    public Color color = Color.gray;
    public Vector2 position;

    private LineRenderer lineRenderer;
    private const int segments = 64;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;
        lineRenderer.useWorldSpace = true;
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineRenderer.endColor = color;
    }

    void Update()
    {
        DrawCircle();
    }

    private void DrawCircle()
    {
        lineRenderer.startColor = lineRenderer.endColor = color;
        lineRenderer.positionCount = segments;
        Vector3[] positions = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 2 * Mathf.PI;
            float x = Mathf.Cos(angle) * radius + position.x;
            float y = Mathf.Sin(angle) * radius + position.y;
            positions[i] = new Vector3(x, y, 0f);
        }

        lineRenderer.SetPositions(positions);
    }

    public bool IsColliding(CirclePhysics other)
    {
        float dist = Vector2.Distance(position, other.position);
        return dist < (radius + other.radius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, radius);
    }
}
