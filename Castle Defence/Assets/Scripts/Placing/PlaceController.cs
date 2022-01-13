using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlaceController : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _lookAccuracy = 0.98f;
    [SerializeField] private float _raycastDistance = 50f;
    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private InputAction placeAction;
    [SerializeField] private GameObject _placeablePrefab;

    private List<PlaceableZone> _zones = new List<PlaceableZone>();
    private PlaceableZone _selectedZone = null;

    private void Start()
    {
        if (!_placeablePrefab.GetComponent<Placeable>())
        {
            throw new ArgumentException("_placeablePrefab doesn't have a required Placeable component attached.");
        }

        placeAction.started += Place;
    }

    private void OnDestroy()
    {
        placeAction.started -= Place;
    }

    private void OnEnable()
    {
        placeAction.Enable();
    }

    private void OnDisable()
    {
        placeAction.Disable();
    }

    private void Update()
    {
        var count = _zones.Count;
        var cameraPosition = _cameraTransform.position;
        var cameraDirection = _cameraTransform.forward.normalized;

        RaycastHit raycastHit;

        var bestDotProduct = float.MinValue;
        PlaceableZone bestZone = null;

        // for debugging
        // Debug.DrawLine(cameraPosition, cameraPosition + cameraDirection * 40f, Color.cyan);

        for (var i = 0; i < count; ++i)
        {
            var zone = _zones[i];
            var zonePosition = zone.transform.position;
            var direction = zonePosition - cameraPosition;
            var directionNormalized = direction.normalized;

            // for debugging
            // Debug.DrawLine(cameraPosition, zonePosition, Color.magenta);

            var isHit = Physics.Raycast(cameraPosition, directionNormalized, out raycastHit, _raycastDistance, _raycastLayers, QueryTriggerInteraction.Ignore);

            if (isHit && raycastHit.collider.gameObject == zone.gameObject)
            {
                var dotProduct = Vector3.Dot(cameraDirection, directionNormalized);

                if (dotProduct > _lookAccuracy && dotProduct > bestDotProduct)
                {
                    bestDotProduct = dotProduct;
                    bestZone = zone;
                }
            }
        }

        if (_selectedZone == bestZone)
        {
            return;
        }

        if (_selectedZone != null)
        {
            _selectedZone.Deselect();
        }

        _selectedZone = bestZone;

        if (bestZone != null)
        {
            bestZone.Select();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        var zone = collider.gameObject.GetComponent<PlaceableZone>();

        if (zone != null)
        {
            _zones.Add(zone);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        var zone = collider.gameObject.GetComponent<PlaceableZone>();

        if (zone != null)
        {
            _zones.Remove(zone);
        }
    }

    private void Place(CallbackContext ctx)
    {
        if (_selectedZone != null && _selectedZone.isFree())
        {
            var placeableGameObject = Instantiate(_placeablePrefab, Vector3.zero, Quaternion.identity);
            var placeable = placeableGameObject.GetComponent<Placeable>();
            _selectedZone.Place(placeable);
        }
    }
}
