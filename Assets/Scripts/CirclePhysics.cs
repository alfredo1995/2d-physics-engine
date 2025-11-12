using UnityEngine;

public class CirclePhysics : MonoBehaviour
{
    public Vector2 position;
    public Vector2 velocity;
    public float radius = 0.5f;
    public Color color = Color.white;

    private LineRenderer lr;

    void Awake()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.widthMultiplier = 0.05f;
        lr.positionCount = 0;
        lr.loop = true;
        lr.useWorldSpace = true;
        lr.startColor = lr.endColor = color;
    }

    public void UpdateVisual()
    {
        // desenha o círculo via LineRenderer
        int segments = 32;
        lr.positionCount = segments;
        lr.startColor = lr.endColor = color;

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius + (Vector3)position;
            lr.SetPosition(i, point);
        }
    }

    public bool IsColliding(CirclePhysics other)
    {
        float dist = Vector2.Distance(position, other.position);
        return dist < (radius + other.radius);
    }

    public void ResolveCollision(CirclePhysics other)
    {
        Vector2 delta = other.position - position;
        float dist = delta.magnitude;
        if (dist == 0f) return;

        float overlap = (radius + other.radius) - dist;
        Vector2 normal = delta.normalized;

        // empurra cada círculo metade da penetração
        position -= normal * (overlap * 0.5f);
        other.position += normal * (overlap * 0.5f);

        // troca componentes de velocidade ao longo da normal (colisão elástica)
        Vector2 relVel = velocity - other.velocity;
        float velAlongNormal = Vector2.Dot(relVel, normal);
        if (velAlongNormal > 0) return;

        float restitution = 1f; // colisão perfeitamente elástica
        float impulse = -(1f + restitution) * velAlongNormal / 2f;
        Vector2 impulseVec = impulse * normal;

        velocity += impulseVec;
        other.velocity -= impulseVec;
    }
}
