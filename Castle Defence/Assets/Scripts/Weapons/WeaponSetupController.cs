using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(SelectionController))]
public class WeaponSetupController : MonoBehaviour
{
    [SerializeField] private InputAction _setupAction;

    private SelectionController _selectionController;

    void Start()
    {
        _selectionController = GetComponent<SelectionController>();
        _setupAction.started += SetupWeapon;
    }

    private void OnDestroy()
    {
        _setupAction.started -= SetupWeapon;
    }

    private void OnEnable()
    {
        _setupAction.Enable();
    }

    private void OnDisable()
    {
        _setupAction.Disable();
    }

    private void SetupWeapon(CallbackContext ctx)
    {
        var selectedZone = _selectionController.GetSelected();

        if (selectedZone != null && selectedZone.isFree() == false)
        {
            print(1);
        }
    }
}
