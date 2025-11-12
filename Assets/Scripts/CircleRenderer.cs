using UnityEngine;

public static class CircleRenderer
{
    public static void DrawCircle(Vector2 center, float radius, Color color, int segments = 64)
    {
        GL.Begin(GL.LINE_STRIP);
        GL.Color(color);

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            float x = Mathf.Cos(angle) * radius + center.x;
            float y = Mathf.Sin(angle) * radius + center.y;
            GL.Vertex3(x, y, 0);
        }

        GL.End();
    }
}
