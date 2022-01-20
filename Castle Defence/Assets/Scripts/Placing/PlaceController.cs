using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(SelectionController))]
public class PlaceController : MonoBehaviour
{
    [SerializeField] private InputAction placeAction;
    [SerializeField] private GameObject _placeablePrefab;

    private SelectionController _selectionController;

    private void Start()
    {
        _selectionController = GetComponent<SelectionController>();
        placeAction.started += Place;

        if (!_placeablePrefab.GetComponent<Placeable>())
        {
            throw new ArgumentException("_placeablePrefab doesn't have a required Placeable component attached.");
        }
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

    private void Place(CallbackContext ctx)
    {
        var selectedZone = _selectionController.GetSelected();

        if (selectedZone != null && selectedZone.isFree())
        {
            var placeableGameObject = Instantiate(_placeablePrefab, Vector3.zero, Quaternion.identity);
            var placeable = placeableGameObject.GetComponent<Placeable>();
            selectedZone.Place(placeable);
        }
    }
}
