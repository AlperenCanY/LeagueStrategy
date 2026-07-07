using System;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager : MonoBehaviour
{
    public CountryManager countryManager;
    public ProvinceManager provinceManager;

    [Header("Recruitment")]
    public int recruitAmount = 1000;
    public int manpowerCost = 1000;
    public int moneyCost = 50;

    private int nextArmyId = 1;

    private Dictionary<int, ArmyData> armiesById = new Dictionary<int, ArmyData>();

    public event Action<ArmyData> OnArmyCreated;
    public event Action<ArmyData> OnArmyMoved;

    public ArmyData RecruitArmy(int provinceId, string requesterCountryTag)
    {
        ProvinceData province = provinceManager.GetProvinceById(provinceId);

        if (province == null)
        {
            Debug.LogWarning("ArmyManager: Province bulunamadı. ID: " + provinceId);
            return null;
        }

        if (province.ownerCountry != requesterCountryTag)
        {
            Debug.Log("Bu province sana ait değil.");
            return null;
        }

        CountryData country = countryManager.GetCountry(requesterCountryTag);

        if (country == null)
        {
            Debug.LogWarning("ArmyManager: Ülke bulunamadı. Tag: " + requesterCountryTag);
            return null;
        }

        if (country.manpower < manpowerCost)
        {
            Debug.Log("Yetersiz manpower.");
            return null;
        }

        if (country.money < moneyCost)
        {
            Debug.Log("Yetersiz para.");
            return null;
        }

        country.manpower -= manpowerCost;
        country.money -= moneyCost;

        ArmyData army = new ArmyData(
            nextArmyId,
            requesterCountryTag,
            provinceId,
            recruitAmount
        );

        nextArmyId++;
        armiesById[army.armyId] = army;

        countryManager.NotifyCountriesChanged();

        Debug.Log("Army oluşturuldu. ID: " + army.armyId + " / Province: " + province.shapeName);

        OnArmyCreated?.Invoke(army);

        return army;
    }

    public bool MoveArmy(int armyId, int targetProvinceId)
    {
        ArmyData army = GetArmy(armyId);

        if (army == null)
        {
            Debug.LogWarning("ArmyManager: Army bulunamadı. ID: " + armyId);
            return false;
        }

        ProvinceData targetProvince = provinceManager.GetProvinceById(targetProvinceId);

        if (targetProvince == null)
        {
            Debug.LogWarning("ArmyManager: Hedef province bulunamadı.");
            return false;
        }

        if (army.currentProvinceId == targetProvinceId)
        {
            Debug.Log("Army zaten bu province içinde.");
            return false;
        }

        army.currentProvinceId = targetProvinceId;

        Debug.Log("Army " + army.armyId + " hedefe gitti: " + targetProvince.shapeName);

        OnArmyMoved?.Invoke(army);

        return true;
    }

    public ArmyData GetArmy(int armyId)
    {
        armiesById.TryGetValue(armyId, out ArmyData army);
        return army;
    }

    public IEnumerable<ArmyData> GetAllArmies()
    {
        return armiesById.Values;
    }
}