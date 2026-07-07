using System;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager : MonoBehaviour
{
    public CountryManager countryManager;
    public ProvinceManager provinceManager;
    public TimeManager timeManager;

    [Header("Recruitment")]
    public int recruitAmount = 1000;
    public int manpowerCost = 1000;
    public int moneyCost = 50;

    [Header("Movement")]
    public int defaultMovementDays = 5;

    private int nextArmyId = 1;

    private Dictionary<int, ArmyData> armiesById = new Dictionary<int, ArmyData>();

    public event Action<ArmyData> OnArmyCreated;
    public event Action<ArmyData> OnArmyChanged;
    public event Action<ArmyData> OnArmyArrived;


    private void OnEnable()
    {
        if (timeManager != null)
        {
            timeManager.OnDayPassed += HandleDayPassed;
        }
    }

    private void OnDisable()
    {
        if (timeManager != null)
        {
            timeManager.OnDayPassed -= HandleDayPassed;
        }
    }

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
        OnArmyChanged?.Invoke(army);

        return army;
    }

    public bool StartMoveArmy(int armyId, int targetProvinceId)
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

        if (army.isMoving)
        {
            Debug.Log("Army zaten hareket ediyor. Yeni hedef verildi.");
        }

        army.isMoving = true;
        army.sourceProvinceId = army.currentProvinceId;
        army.targetProvinceId = targetProvinceId;
        army.movementDaysTotal = defaultMovementDays;
        army.movementDaysRemaining = defaultMovementDays;
        army.movementProgress = 0f;

        Debug.Log("Army " + army.armyId + " hareket ediyor -> " + targetProvince.shapeName);

        OnArmyChanged?.Invoke(army);

        return true;
    }

    private void HandleDayPassed(int day, int month, int year)
    {
        foreach (ArmyData army in armiesById.Values)
        {
            if (!army.isMoving)
                continue;

            army.movementDaysRemaining--;

            if (army.movementDaysRemaining <= 0)
            {
                FinishMovement(army);
            }
            else
            {
                OnArmyChanged?.Invoke(army);
            }
        }
    }

    private void FinishMovement(ArmyData army)
    {
        army.currentProvinceId = army.targetProvinceId;
        army.sourceProvinceId = army.currentProvinceId;

        army.isMoving = false;
        army.movementDaysTotal = 0;
        army.movementDaysRemaining = 0;

        ProvinceData province = provinceManager.GetProvinceById(army.currentProvinceId);

        Debug.Log("Army " + army.armyId + " vardı: " + (province != null ? province.shapeName : "Unknown"));

        OnArmyChanged?.Invoke(army);
        OnArmyArrived?.Invoke(army);
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

    private void Update()
{
    TickArmyMovement();
}

private void TickArmyMovement()
{
    if (timeManager == null)
        return;

    if (timeManager.isPaused)
        return;

    foreach (ArmyData army in armiesById.Values)
    {
        if (!army.isMoving)
            continue;

        int oldRemainingDays = army.movementDaysRemaining;

        float totalMoveSeconds = timeManager.secondsPerDay * army.movementDaysTotal;

        if (totalMoveSeconds <= 0f)
            totalMoveSeconds = 1f;

        army.movementProgress += Time.deltaTime / totalMoveSeconds;
        army.movementProgress = Mathf.Clamp01(army.movementProgress);

        army.movementDaysRemaining = Mathf.CeilToInt(
            (1f - army.movementProgress) * army.movementDaysTotal
        );

        if (army.movementProgress >= 1f)
        {
            FinishMovement(army);
            continue;
        }

        if (army.movementDaysRemaining != oldRemainingDays)
        {
            OnArmyChanged?.Invoke(army);
        }
    }
}
}