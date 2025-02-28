using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
    public LineRenderer circleRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure LineRenderer component exists
        if (circleRenderer == null)
        {
            circleRenderer = gameObject.GetComponent<LineRenderer>();
        }
        
        // Set basic properties of LineRenderer
        circleRenderer.positionCount = 0;
        circleRenderer.useWorldSpace = true;
        circleRenderer.loop = true; // Connect start and end points to form a closed loop
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DrawCircle(float radius, int segments)
    {
        // Set number of line segments
        circleRenderer.positionCount = segments + 1;
        
        // Calculate angle for each segment
        float deltaTheta = (2f * Mathf.PI) / segments;
        float theta = 0f;
        
        // Generate circle points
        for (int i = 0; i <= segments; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            
            Vector3 pos = transform.position + new Vector3(x, 0f, z);
            circleRenderer.SetPosition(i, pos);
            
            theta += deltaTheta;
        }
    }

    public void ShowCircle(Vector3 position, float radius)
    {
        transform.position = position;
        DrawCircle(radius, 32); // Draw circle with 32 segments
        circleRenderer.enabled = true;
    }

    public void HideCircle()
    {
        circleRenderer.enabled = false;
    }
}
