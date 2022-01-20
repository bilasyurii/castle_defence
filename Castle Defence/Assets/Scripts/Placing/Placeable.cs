using UnityEngine;

public class Placeable : MonoBehaviour
{
    [SerializeField] private Transform _setupCameraTransform;

    public Transform GetSetupCameraTransform()
    {
        return _setupCameraTransform;
    }

    public void OnPlaced()
    {
        // TODO
    }
}
