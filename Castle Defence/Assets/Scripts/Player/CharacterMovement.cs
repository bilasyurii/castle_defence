using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform _camera;

    [Header("Movement")]
    [SerializeField] private float _gravity = -40f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float _movementSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Raycast")]
    [SerializeField] private float _floorOffsetY = 0.9f;
    [SerializeField] private float _raycastOffsetVertical = 0f;
    [SerializeField] private float _raycastOffsetHorizontal = 0.5f;
    [SerializeField] private float _floorRaycastDistance = 1.2f;
    [SerializeField] private float _groundCheckRaycastDistance = 1f;

    private Rigidbody _rigidbody;

    private float _inputX;
    private float _inputY;

    private Vector3 _movementDirection;
    private RaycastHit _slopeHit;

    [Header("Info")]
    [ReadOnly] [SerializeField] private float _velocityY;
    [ReadOnly] [SerializeField] private bool _isGrounded;
    [ReadOnly] [SerializeField] private Vector3 _floorPosition;
    [ReadOnly] [SerializeField] private Vector3 _combinedRaycast;
    [ReadOnly] [SerializeField] private Vector3 _combinedSlopeNormal;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
        _inputY = Input.GetAxisRaw("Vertical");

        var movementForward = _camera.forward * _inputY;
        var movementSide = _camera.right * _inputX;
        var movementCombined = movementForward + movementSide;
        var movementNormalized = movementCombined.normalized;

        _movementDirection = new Vector3(movementNormalized.x, 0f, movementNormalized.z);

        //  if there is any movement, interpolate rotation of a character to look at movement direction
        if (_movementDirection != Vector3.zero)
        {
            var rotationAmount = Time.deltaTime * _rotationSpeed;
            var targetRotation = Quaternion.LookRotation(_movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationAmount);
        }

        if (Input.GetButton("Jump") && _isGrounded)
        {
            print("jump");
            _velocityY = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            var velocity = _rigidbody.velocity;
            _rigidbody.velocity = new Vector3(velocity.x, _velocityY, velocity.z);
        }
    }

    private void FixedUpdate()
    {
        // raycasting downwards to check, if character is on ground
        var isGrounded = RaycastFloor(0f, 0f, _groundCheckRaycastDistance, out _);

        if (isGrounded)
        {
            FindFloorParameters();

            var root = transform.TransformPoint(Vector3.down * _floorOffsetY);
            Debug.DrawLine(root, root + _combinedSlopeNormal * 4f, Color.yellow);
        }

        Vector3 velocity = _movementDirection * _movementSpeed;

        // sliding
        if (isGrounded && _combinedSlopeNormal != Vector3.zero)
        {
            // project the velocity onto the slope normal
            velocity = Vector3.ProjectOnPlane(velocity, _combinedSlopeNormal);
        }

        // if character is in mid-air (not grounded), then add velocity downwards
        if (isGrounded == false)
        {
            _velocityY += _gravity * Time.fixedDeltaTime;
            velocity.y += _velocityY;
        }

        _rigidbody.velocity = velocity;

        if (isGrounded)
        {
            // if wasn't grounded in previous FixedUpdate call, reset velocity on y
            if (_isGrounded == false)
            {
                _velocityY = 0f;
                velocity.y = 0;
                _rigidbody.velocity = velocity;
            }

            // calculating position of character as if it was on floor
            var positionY = _combinedRaycast.y + _floorOffsetY;
            var position = _rigidbody.position;
            _floorPosition = new Vector3(position.x, positionY, position.z);

            // sticking character to floor to fix jittering
            if (_floorPosition != position)
            {
                _rigidbody.position = _floorPosition;
            }
        }

        _isGrounded = isGrounded;
    }

    // make a raycast to find intersection with floor
    private bool RaycastFloor(float offsetX, float offsetZ, float length, out RaycastHit hit)
    {
        // getting raycast origin in world coordinates
        var raycastFloorPosition = transform.TransformPoint(offsetX, _raycastOffsetVertical, offsetZ);

        // for debugging
        if (Mathf.Approximately(length, _groundCheckRaycastDistance) == false)
        {
            Debug.DrawLine(raycastFloorPosition, raycastFloorPosition + Vector3.down * length, Color.magenta);
        }

        // sending raycast
        return Physics.Raycast(raycastFloorPosition, Vector3.down, out hit, length);
    }

    // calculate average of 5 raycasts (hit point & slope normal)
    private void FindFloorParameters()
    {
        // reset combined raycast & slope normal vectors
        _combinedRaycast = Vector3.zero;
        _combinedSlopeNormal = Vector3.zero;

        // raycast in the center
        CombineRaycastTo(0f, 0f);

        // count of raycasts, that have hit the ground
        // raycast in center 
        var hitCount = 1;

        // 4 side raycasts
        if (CombineRaycastTo(_raycastOffsetHorizontal, 0f))
        {
            ++hitCount;
        }

        if (CombineRaycastTo(-_raycastOffsetHorizontal, 0f))
        {
            ++hitCount;
        }

        if (CombineRaycastTo(0f, _raycastOffsetHorizontal))
        {
            ++hitCount;
        }

        if (CombineRaycastTo(0f, -_raycastOffsetHorizontal))
        {
            ++hitCount;
        }

        _combinedRaycast /= hitCount;
        _combinedSlopeNormal /= hitCount;
    }

    // make a raycast and combine hit point with the _combinedRaycast vector
    // returns boolean, that indicates, whether the cast has hit ground
    private bool CombineRaycastTo(float offsetX, float offsetZ)
    {
        var hasHit = RaycastFloor(offsetX, offsetZ, _floorRaycastDistance, out _slopeHit);

        if (hasHit)
        {
            _combinedRaycast += _slopeHit.point;
            _combinedSlopeNormal += _slopeHit.normal;
            return true;
        }

        return false;
    }
}
