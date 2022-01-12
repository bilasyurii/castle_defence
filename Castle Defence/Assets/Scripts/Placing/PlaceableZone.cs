using UnityEngine;

public class PlaceableZone : MonoBehaviour
{
    [SerializeField] private GameObject _defaultView;
    [SerializeField] private GameObject _selectedView;

    private bool _isSelected = true;

    public void Select()
    {
        if (_isSelected)
        {
            return;
        }

        _isSelected = true;
        _defaultView.SetActive(false);
        _selectedView.SetActive(true);
    }

    public void Deselect()
    {
        if (_isSelected == false)
        {
            return;
        }

        _isSelected = false;
        _defaultView.SetActive(true);
        _selectedView.SetActive(false);
    }

    private void Awake()
    {
        Deselect();
    }
}
