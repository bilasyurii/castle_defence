using UnityEngine;
using Utils.Extensions;

public class CannonView: MonoBehaviour
{
    [SerializeField] private Transform _body;
    [SerializeField] private float _rotationSpeed = 20f;

    private Quaternion _targetLookRotation;
    private bool _isRotating = false;

    private void Start()
    {
        SetTarget(new Vector3(20f, 4f, -20f));
    }

    public void SetTarget(Vector3 targetPosition)
    {
        var direction = targetPosition - transform.position;
        direction.y = 0;

        _targetLookRotation = Quaternion.LookRotation(direction);
        _isRotating = true;
    }

    private void Update()
    {
        if (_isRotating)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        var rotationAmount = _rotationSpeed * Time.deltaTime;
        var rotation = Quaternion.RotateTowards(_body.rotation, _targetLookRotation, rotationAmount);
        _body.rotation = rotation;

        if (rotation.EqualsApproximately(_targetLookRotation, 0.0001f))
        {
            _isRotating = false;
            _body.rotation = _targetLookRotation;
        }
    }
}
