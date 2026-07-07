using UnityEngine;

public class WarManager : MonoBehaviour
{
    public ArmyManager armyManager;
    public ProvinceManager provinceManager;
    public CountryManager countryManager;
    public SelectionManager selectionManager;

    private void OnEnable()
    {
        if (armyManager != null)
        {
            armyManager.OnArmyArrived += HandleArmyArrived;
        }
    }

    private void OnDisable()
    {
        if (armyManager != null)
        {
            armyManager.OnArmyArrived -= HandleArmyArrived;
        }
    }

    private void HandleArmyArrived(ArmyData army)
    {
        if (army == null)
            return;

        ProvinceData province = provinceManager.GetProvinceById(army.currentProvinceId);

        if (province == null)
            return;

        if (province.ownerCountry == army.ownerCountryTag)
        {
            Debug.Log("Army dost province'e vardı: " + province.shapeName);
            return;
        }

        CaptureProvince(army, province);
    }

    private void CaptureProvince(ArmyData army, ProvinceData province)
    {
        Debug.Log("Province ele geçiriliyor: " + province.shapeName);

        countryManager.TransferProvince(province, army.ownerCountryTag);

        if (selectionManager != null)
        {
            selectionManager.RefreshCurrentArmySelection();
            selectionManager.RefreshCurrentProvinceSelection();
        }
    }
}