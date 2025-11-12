using UnityEngine;

public class CircleManager : MonoBehaviour
{
    public int circleCount = 10;
    public float areaSize = 6f;
    public float speed = 2f;

    private CirclePhysics[] circles;

    void Start()
    {
        circles = new CirclePhysics[circleCount];

        for (int i = 0; i < circleCount; i++)
        {
            GameObject cObj = new GameObject("Circle_" + i);
            CirclePhysics c = cObj.AddComponent<CirclePhysics>();

            c.radius = 0.4f;
            c.position = new Vector2(
                Random.Range(-areaSize * 0.8f, areaSize * 0.8f),
                Random.Range(-areaSize * 0.8f, areaSize * 0.8f)
            );
            c.velocity = Random.insideUnitCircle * speed;
            c.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.9f, 1f);
            circles[i] = c;
        }
    }

    void Update()
    {
        // movimentação e colisão com bordas
        foreach (var c in circles)
        {
            c.position += c.velocity * Time.deltaTime;

            if (c.position.x + c.radius > areaSize)
            {
                c.position.x = areaSize - c.radius;
                c.velocity.x *= -1;
            }
            else if (c.position.x - c.radius < -areaSize)
            {
                c.position.x = -areaSize + c.radius;
                c.velocity.x *= -1;
            }

            if (c.position.y + c.radius > areaSize)
            {
                c.position.y = areaSize - c.radius;
                c.velocity.y *= -1;
            }
            else if (c.position.y - c.radius < -areaSize)
            {
                c.position.y = -areaSize + c.radius;
                c.velocity.y *= -1;
            }
        }

        // colisões n×n
        for (int i = 0; i < circles.Length; i++)
        {
            for (int j = i + 1; j < circles.Length; j++)
            {
                if (circles[i].IsColliding(circles[j]))
                    circles[i].ResolveCollision(circles[j]);
            }
        }

        // atualizar visuais
        foreach (var c in circles)
            c.UpdateVisual();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * areaSize * 2f);
    }
}
