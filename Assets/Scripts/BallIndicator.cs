using UnityEngine;

public class CircleRenderer : MonoBehaviour
{
    public LineRenderer circleRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 确保有LineRenderer组件
        if (circleRenderer == null)
        {
            circleRenderer = gameObject.GetComponent<LineRenderer>();
        }
        
        // 设置LineRenderer的基本属性
        circleRenderer.positionCount = 0;
        circleRenderer.useWorldSpace = true;
        circleRenderer.loop = true; // 首尾相连形成闭环
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DrawCircle(float radius, int segments)
    {
        // 设置线段数量
        circleRenderer.positionCount = segments + 1;
        
        // 计算每个段的角度
        float deltaTheta = (2f * Mathf.PI) / segments;
        float theta = 0f;
        
        // 生成圆形的点
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
        DrawCircle(radius, 32); // 使用32个段来绘制圆形
        circleRenderer.enabled = true;
    }

    public void HideCircle()
    {
        circleRenderer.enabled = false;
    }
}
