using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CirclePhysics : MonoBehaviour
{
    public float radius = 1f;
    public float mass = 1f;
    public Color color = Color.gray;
    public Vector2 position;
    public Vector2 velocity;
    public bool isKinematic = false; // se for controlado externamente (como o do mouse)

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
        // Atualiza posição se não for controlado manualmente
        if (!isKinematic)
            position += velocity * Time.deltaTime;

        DrawCircle();
    }

    private void DrawCircle()
    {
        lineRenderer.startColor = lineRenderer.endColor = color;
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

    public void ResolveCollision(CirclePhysics other, float restitution = 0.8f)
    {
        Vector2 delta = other.position - position;
        float dist = delta.magnitude;
        if (dist == 0f) return;

        float penetration = (radius + other.radius) - dist;
        Vector2 normal = delta / dist;

        // Corrige a sobreposição
        float totalMass = mass + other.mass;
        position -= normal * (penetration * (other.mass / totalMass));
        other.position += normal * (penetration * (mass / totalMass));

        // Calcula velocidades relativas
        Vector2 relativeVelocity = other.velocity - velocity;
        float velAlongNormal = Vector2.Dot(relativeVelocity, normal);

        if (velAlongNormal > 0) return; // já estão se separando

        // Impulso
        float j = -(1 + restitution) * velAlongNormal;
        j /= (1 / mass) + (1 / other.mass);

        Vector2 impulse = j * normal;

        velocity -= (1 / mass) * impulse;
        other.velocity += (1 / other.mass) * impulse;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, radius);
    }
}
