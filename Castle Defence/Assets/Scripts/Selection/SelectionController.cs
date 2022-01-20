using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _lookAccuracy = 0.98f;
    [SerializeField] private float _raycastDistance = 50f;
    [SerializeField] private LayerMask _raycastLayers;

    private List<PlaceableZone> _zones = new List<PlaceableZone>();
    private PlaceableZone _selectedZone = null;

    public PlaceableZone GetSelected()
    {
        return _selectedZone;
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
}
