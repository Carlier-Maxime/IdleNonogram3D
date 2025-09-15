using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    public Color gridColor1;
    public Color gridColor2;
    public float lineWidth = 0.05f;
    public void DrawGrid(int width, int height, Vector3 pos)
    {
        var lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.colorGradient = new Gradient
        {
            colorKeys = new GradientColorKey[]
            {
                new(gridColor1, 0.0f),
                new(gridColor2, 1.0f),
            }
        };
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2*(width+height)+3;
        for (var x = 0; x <= width; ++x)
        {
            for (var y = 0; y < 2; ++y)
            {
                var i = x*2 + y;
                var posY = ((x&1) == 0 ? y : 1-y)*height - 0.5f;
                lineRenderer.SetPosition(i, pos + new Vector3(x-0.5f, posY, 0));
            }
        }

        var offsetI = 2*width+1;
        for (var y = 0; y <= height; ++y)
        {
            for (var x = 0; x < 2; ++x)
            {
                var i = offsetI + y*2 + x;
                var posX = ((y&1) == 0 ? 1-x : x)*height - 0.5f;
                lineRenderer.SetPosition(i, pos + new Vector3(posX, y-0.5f, 0));
            }
        }
    }
}
