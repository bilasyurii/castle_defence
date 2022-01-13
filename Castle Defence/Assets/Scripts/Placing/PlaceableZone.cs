using UnityEngine;

public class PlaceableZone : MonoBehaviour
{
    [SerializeField] private GameObject _defaultView;
    [SerializeField] private GameObject _selectedView;

    private bool _isSelected = true;
    private Placeable _currentPlaceable = null;

    private void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        if (_isSelected)
        {
            return;
        }

        _isSelected = true;

        if (_currentPlaceable == null)
        {
            _defaultView.SetActive(false);
            _selectedView.SetActive(true);
        }
    }

    public void Deselect()
    {
        if (_isSelected == false)
        {
            return;
        }

        _isSelected = false;

        if (_currentPlaceable == null)
        {
            _defaultView.SetActive(true);
            _selectedView.SetActive(false);
        }
    }

    public void Place(Placeable placeable)
    {
        if (_currentPlaceable == null)
        {
            AddPlaceable(placeable);
            HideHelperViews();
        }
    }

    public bool isFree()
    {
        return _currentPlaceable == null;
    }

    private void AddPlaceable(Placeable placeable)
    {
        _currentPlaceable = placeable;
        placeable.gameObject.transform.SetParent(transform, false);
        placeable.OnPlaced();
    }

    private void HideHelperViews()
    {
        _defaultView.SetActive(false);
        _selectedView.SetActive(false);
    }
}
