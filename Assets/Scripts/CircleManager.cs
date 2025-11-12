using UnityEngine;

public class CircleManager : MonoBehaviour
{
    private CirclePhysics circleA;
    private CirclePhysics circleB;
    private LineRenderer linkLine;

    void Start()
    {
        // cria os dois círculos
        circleA = new GameObject("CircleA").AddComponent<CirclePhysics>();
        circleB = new GameObject("CircleB").AddComponent<CirclePhysics>();

        circleA.radius = 1f;
        circleB.radius = 1f;
        circleA.position = new Vector2(-2f, 0f);
        circleB.position = new Vector2(2f, 0f);

        // cria a linha entre eles
        GameObject lineObj = new GameObject("LinkLine");
        linkLine = lineObj.AddComponent<LineRenderer>();
        linkLine.material = new Material(Shader.Find("Sprites/Default"));
        linkLine.widthMultiplier = 0.05f;
        linkLine.positionCount = 2;
        linkLine.startColor = linkLine.endColor = Color.blue;
    }

    void Update()
    {
        // mover o círculo B com o mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        circleB.position = new Vector2(mousePos.x, mousePos.y);

        // detectar colisão
        bool colliding = circleA.IsColliding(circleB);
        circleA.color = colliding ? Color.red : Color.gray;
        circleB.color = colliding ? Color.red : Color.gray;
        linkLine.startColor = linkLine.endColor = colliding ? Color.red : Color.blue;

        // atualizar linha entre eles
        linkLine.SetPosition(0, circleA.position);
        linkLine.SetPosition(1, circleB.position);
    }
}
