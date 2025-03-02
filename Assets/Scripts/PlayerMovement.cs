using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour
{
    private CharacterController _controller;
    
    private Vector3 _input;
    private Vector3 _direction;
    private float smoothTime = 0.05f;
    private float _currentVelocity;
    private float speed = 10f;

    private Vector3 initialPosition;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        initialPosition = transform.position;
        
        _controller.slopeLimit = 45f;  // 最大斜坡角度
        _controller.stepOffset = 0.3f; // 台阶高度
        _controller.minMoveDistance = 0.001f; // 最小移动距离
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        Vector3 groundDirection = new Vector3(moveHorizontal, 0f, moveVertical);
        
        if (groundDirection.sqrMagnitude > 0.0f) 
        {
            float targetAngle = Mathf.Atan2(groundDirection.x, groundDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            Vector3 moveDirection = Vector3.ProjectOnPlane(groundDirection.normalized, Vector3.up);
            _controller.Move(moveDirection * speed * Time.deltaTime);
        }
    }

    public void ResetToInitialPosition()
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }

        if (_controller != null)
        {
            _controller.enabled = false;
            transform.position = initialPosition;
            _controller.enabled = true;
        }
        else
        {
            Debug.LogWarning("CharacterController not found on " + gameObject.name);
        }
    }
}
