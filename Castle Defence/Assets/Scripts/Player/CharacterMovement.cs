using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform _camera;

    [Header("Movement")]
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _gravity = -40f;

    [HeaderAttribute("Rotation")]
    [SerializeField] private float _turnSmoothTime = 0.1f;

    private CharacterController _controller;

    private float velocityY = 0f;
    private bool _isGrounded = false;
    private float _turnSmoothVelocity;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && velocityY < 0f)
        {
            velocityY = -2f;
        }

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;
        var dt = Time.deltaTime;

        var movementVector = Vector3.zero;

        if (direction.sqrMagnitude >= 0.01f)
        {
            var targetAngleRad = Mathf.Atan2(direction.x, direction.z);
            var targetAngleDeg = targetAngleRad * Mathf.Rad2Deg;
            var cameraAngle = _camera.eulerAngles.y;
            var targetAngle = targetAngleDeg + cameraAngle;
            var currentAngle = transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(
                currentAngle,
                targetAngle,
                ref _turnSmoothVelocity,
                _turnSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            var moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            var moveDirectionNormalized = moveDirection.normalized;
            var movement = _speed * dt;
            movementVector = moveDirectionNormalized * movement;;
        }

        if (Input.GetButton("Jump") && _isGrounded)
        {
            velocityY = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        velocityY += _gravity * dt;
        movementVector.y += velocityY * dt;

        _controller.Move(movementVector);
    }
}
