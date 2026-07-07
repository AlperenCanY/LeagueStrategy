using UnityEngine;

public class MapClickHandler : MonoBehaviour
{
    public ProvinceMapPicker provinceMapPicker;
    public SelectionManager selectionManager;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }
    }

    private void HandleLeftClick()
    {
        if (provinceMapPicker == null || selectionManager == null)
        {
            Debug.LogError("MapClickHandler bağlantıları eksik.");
            return;
        }

        ArmyMarkerView armyMarker = GetArmyMarkerUnderMouse();

        if (armyMarker != null && armyMarker.army != null)
        {
            selectionManager.SelectArmy(armyMarker.army.armyId);
            return;
        }

        if (provinceMapPicker.TryPickProvince(Input.mousePosition, out int provinceId))
        {
            selectionManager.SelectProvince(provinceId);
        }
    }

    private ArmyMarkerView GetArmyMarkerUnderMouse()
    {
        Camera cam = provinceMapPicker.mainCamera;

        if (cam == null)
            cam = Camera.main;

        Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = new Vector2(worldPosition.x, worldPosition.y);

        Collider2D hit = Physics2D.OverlapPoint(point);

        if (hit == null)
            return null;

        return hit.GetComponent<ArmyMarkerView>();
    }
}