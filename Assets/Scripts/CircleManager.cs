using UnityEngine;

public class CircleManager : MonoBehaviour
{
    private CirclePhysics circleA;
    private CirclePhysics circleB;
    private LineRenderer linkLine;

    void Start()
    {
        circleA = new GameObject("CircleA").AddComponent<CirclePhysics>();
        circleB = new GameObject("CircleB").AddComponent<CirclePhysics>();

        circleA.radius = 1f;
        circleB.radius = 1f;
        circleA.mass = 2f;
        circleB.mass = 1f;
        circleA.position = new Vector2(-2f, 0f);
        circleB.position = new Vector2(2f, 0f);
        circleA.velocity = new Vector2(2f, 0f); // círculo A se move
        circleB.isKinematic = true; // círculo B segue o mouse

        // linha entre eles
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

        // detectar colisão
        bool colliding = circleA.IsColliding(circleB);
        if (colliding)
            circleA.ResolveCollision(circleB);

        // cores e linha
        circleA.color = colliding ? Color.red : Color.gray;
        circleB.color = colliding ? Color.red : Color.gray;
        linkLine.startColor = linkLine.endColor = colliding ? Color.red : Color.blue;

        // atualizar linha
        linkLine.SetPosition(0, circleA.position);
        linkLine.SetPosition(1, circleB.position);
    }
}
