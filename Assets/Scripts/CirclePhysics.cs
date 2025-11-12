using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CirclePhysics : MonoBehaviour
{
    [Header("Propriedades físicas")]
    public float radius = 1f;
    public float mass = 1f;
    public bool isKinematic = false; // se true, é controlado externamente (ex: segue o mouse)
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 acceleration;

    [Header("Forças e física")]
    public float gravityScale = 1f; // força gravitacional
    public float linearDamping = 0.99f; // amortecimento (resistência do ar)
    public float frictionCoefficient = 0.8f; // atrito ao colidir

    [Header("Visual")]
    public Color color = Color.gray;

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
        if (!isKinematic)
        {
            ApplyForces();
            Integrate();
        }

        DrawCircle();
    }

    // Aplicar forças externas (gravidade)
    private void ApplyForces()
    {
        Vector2 gravity = Physics2D.gravity * gravityScale;
        acceleration = gravity;
    }

    //  Integrar movimento (Euler)
    private void Integrate()
    {
        velocity += acceleration * Time.deltaTime;
        velocity *= linearDamping; // amortecimento
        position += velocity * Time.deltaTime;
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

        // Corrige sobreposição
        float totalMass = mass + other.mass;
        position -= normal * (penetration * (other.mass / totalMass));
        other.position += normal * (penetration * (mass / totalMass));

        // Velocidades relativas
        Vector2 relativeVelocity = other.velocity - velocity;
        float velAlongNormal = Vector2.Dot(relativeVelocity, normal);

        if (velAlongNormal > 0) return; // já se afastando

        // Impulso normal (colisão elástica)
        float j = -(1 + restitution) * velAlongNormal;
        j /= (1 / mass) + (1 / other.mass);
        Vector2 impulse = j * normal;

        velocity -= (1 / mass) * impulse;
        other.velocity += (1 / other.mass) * impulse;

        // ATRITO — cálculo tangencial
        relativeVelocity = other.velocity - velocity;
        Vector2 tangent = relativeVelocity - Vector2.Dot(relativeVelocity, normal) * normal;
        tangent.Normalize();

        float jt = -Vector2.Dot(relativeVelocity, tangent);
        jt /= (1 / mass) + (1 / other.mass);

        float mu = Mathf.Sqrt(frictionCoefficient * other.frictionCoefficient); // coeficiente médio
        Vector2 frictionImpulse;
        if (Mathf.Abs(jt) < j * mu)
            frictionImpulse = jt * tangent; // atrito estático
        else
            frictionImpulse = -j * mu * tangent; // atrito dinâmico

        velocity -= (1 / mass) * frictionImpulse;
        other.velocity += (1 / other.mass) * frictionImpulse;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, radius);
    }
}
