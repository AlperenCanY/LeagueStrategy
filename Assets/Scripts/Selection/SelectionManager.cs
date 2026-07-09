using System;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public ProvinceManager provinceManager;
    public CountryManager countryManager;
    public ArmyManager armyManager;

    public ProvinceSelection CurrentProvinceSelection { get; private set; }
    public ArmyData CurrentArmySelection { get; private set; }

    public event Action<ProvinceSelection> OnProvinceSelected;
    public event Action<ArmyData> OnArmySelected;

    private void OnEnable()
    {
        if (armyManager != null)
        {
            armyManager.OnArmyChanged += HandleArmyChanged;
            armyManager.OnArmyDestroyed += HandleArmyDestroyed;
        }

        if (countryManager != null)
            countryManager.OnCountriesChanged += RefreshCurrentProvinceSelection;
    }

    private void OnDisable()
    {
        if (armyManager != null)
        {
            armyManager.OnArmyChanged -= HandleArmyChanged;
            armyManager.OnArmyDestroyed -= HandleArmyDestroyed;
        }

        if (countryManager != null)
            countryManager.OnCountriesChanged -= RefreshCurrentProvinceSelection;
    }

    public void SelectProvince(int provinceId)
    {
        ProvinceData province = provinceManager.GetProvinceById(provinceId);

        if (province == null)
        {
            Debug.LogWarning("SelectionManager: Province bulunamadı. ID: " + provinceId);
            return;
        }

        CountryData ownerCountry = null;

        if (countryManager != null)
            ownerCountry = countryManager.GetCountry(province.ownerCountry);

        CurrentArmySelection = null;
        OnArmySelected?.Invoke(null);

        CurrentProvinceSelection = new ProvinceSelection(province, ownerCountry);
        OnProvinceSelected?.Invoke(CurrentProvinceSelection);
    }

    public void SelectArmy(int armyId)
    {
        if (armyManager == null)
        {
            Debug.LogError("SelectionManager: ArmyManager atanmadı.");
            return;
        }

        ArmyData army = armyManager.GetArmy(armyId);

        if (army == null)
        {
            Debug.LogWarning("SelectionManager: Army bulunamadı. ID: " + armyId);
            ClearArmySelection();
            return;
        }

        CurrentProvinceSelection = null;
        OnProvinceSelected?.Invoke(null);

        CurrentArmySelection = army;

        Debug.Log("Army seçildi. ID: " + army.armyId + " / Troops: " + army.troopCount);

        OnArmySelected?.Invoke(army);
    }

    public void RefreshCurrentProvinceSelection()
    {
        if (CurrentProvinceSelection == null || CurrentProvinceSelection.province == null)
            return;

        ProvinceData province = CurrentProvinceSelection.province;

        CountryData ownerCountry = null;

        if (countryManager != null)
            ownerCountry = countryManager.GetCountry(province.ownerCountry);

        CurrentProvinceSelection = new ProvinceSelection(province, ownerCountry);

        OnProvinceSelected?.Invoke(CurrentProvinceSelection);
    }

    public void RefreshCurrentArmySelection()
    {
        if (CurrentArmySelection == null)
            return;

        if (armyManager == null)
        {
            ClearArmySelection();
            return;
        }

        ArmyData army = armyManager.GetArmy(CurrentArmySelection.armyId);

        if (army == null)
        {
            ClearArmySelection();
            return;
        }

        CurrentArmySelection = army;
        OnArmySelected?.Invoke(CurrentArmySelection);
    }

    private void HandleArmyChanged(ArmyData army)
    {
        if (CurrentArmySelection == null || army == null)
            return;

        if (CurrentArmySelection.armyId != army.armyId)
            return;

        CurrentArmySelection = army;
        OnArmySelected?.Invoke(CurrentArmySelection);
    }

    private void HandleArmyDestroyed(ArmyData army)
    {
        if (CurrentArmySelection == null || army == null)
            return;

        if (CurrentArmySelection.armyId != army.armyId)
            return;

        ClearArmySelection();
    }

    private void ClearArmySelection()
    {
        CurrentArmySelection = null;
        OnArmySelected?.Invoke(null);
    }
}