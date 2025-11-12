using UnityEngine;

public class CircleManager : MonoBehaviour
{
    private CirclePhysics circleA;
    private CirclePhysics circleB;
    private LineRenderer linkLine;

    [Header("Configuração global")]
    public float restitution = 0.8f;
    public float groundY = -3f; // nível do "chão"

    void Start()
    {
        Physics2D.gravity = new Vector2(0f, -9.81f); // ativa gravidade global

        // cria dois círculos
        circleA = new GameObject("CircleA").AddComponent<CirclePhysics>();
        circleB = new GameObject("CircleB").AddComponent<CirclePhysics>();

        circleA.radius = 1f;
        circleB.radius = 1f;
        circleA.mass = 2f;
        circleB.mass = 1f;
        circleA.position = new Vector2(-2f, 2f);
        circleB.position = new Vector2(2f, 4f);
        circleA.velocity = new Vector2(2f, 0f);

        circleB.isKinematic = true; // segue o mouse

        // cria linha entre eles
        GameObject lineObj = new GameObject("LinkLine");
        linkLine = lineObj.AddComponent<LineRenderer>();
        linkLine.material = new Material(Shader.Find("Sprites/Default"));
        linkLine.widthMultiplier = 0.05f;
        linkLine.positionCount = 2;
        linkLine.startColor = linkLine.endColor = Color.blue;
    }

    void Update()
    {
        // mover círculo B com o mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        circleB.position = mousePos;

        // detectar e resolver colisão entre os dois
        bool colliding = circleA.IsColliding(circleB);
        if (colliding)
            circleA.ResolveCollision(circleB, restitution);

        // colisão com o chão
        HandleGroundCollision(circleA);
        HandleGroundCollision(circleB);

        // atualizar cor e linha
        circleA.color = colliding ? Color.red : Color.gray;
        circleB.color = colliding ? Color.red : Color.gray;
        linkLine.startColor = linkLine.endColor = colliding ? Color.red : Color.blue;

        linkLine.SetPosition(0, circleA.position);
        linkLine.SetPosition(1, circleB.position);
    }

    void HandleGroundCollision(CirclePhysics c)
    {
        if (c.isKinematic) return;

        if (c.position.y - c.radius < groundY)
        {
            c.position.y = groundY + c.radius;
            c.velocity.y *= -restitution;
            c.velocity.x *= c.frictionCoefficient; // atrito com o chão
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-10, groundY, 0), new Vector3(10, groundY, 0));
    }
}
