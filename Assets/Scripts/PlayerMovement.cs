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
    [SerializeField] private float speed = 5f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        _direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
        if (_direction.sqrMagnitude > 0.0f) {
            float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            _controller.Move(_direction*speed*Time.deltaTime);
        }
    }
}
