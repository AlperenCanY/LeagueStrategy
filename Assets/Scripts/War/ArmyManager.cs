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

    private List<ArmyData> tickBuffer = new List<ArmyData>();
    private List<ArmyData> arrivedBuffer = new List<ArmyData>();

    public event Action<ArmyData> OnArmyCreated;
    public event Action<ArmyData> OnArmyChanged;
    public event Action<ArmyData> OnArmyArrived;
    public event Action<ArmyData> OnArmyDestroyed;

    private void Update()
    {
        TickArmyMovement();
    }

    public ArmyData RecruitArmy(int provinceId, string requesterCountryTag)
    {
        if (provinceManager == null || countryManager == null)
        {
            Debug.LogError("ArmyManager bağlantıları eksik.");
            return null;
        }

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

        int moveDays = Mathf.Max(1, defaultMovementDays);

        army.isMoving = true;
        army.sourceProvinceId = army.currentProvinceId;
        army.targetProvinceId = targetProvinceId;
        army.movementDaysTotal = moveDays;
        army.movementDaysRemaining = moveDays;
        army.movementProgress = 0f;

        Debug.Log("Army " + army.armyId + " hareket ediyor -> " + targetProvince.shapeName);

        OnArmyChanged?.Invoke(army);

        return true;
    }

    private void TickArmyMovement()
    {
        if (timeManager == null)
            return;

        if (timeManager.isPaused)
            return;

        tickBuffer.Clear();
        arrivedBuffer.Clear();

        foreach (ArmyData army in armiesById.Values)
            tickBuffer.Add(army);

        foreach (ArmyData army in tickBuffer)
        {
            if (army == null)
                continue;

            if (!armiesById.ContainsKey(army.armyId))
                continue;

            if (!army.isMoving)
                continue;

            int oldRemainingDays = army.movementDaysRemaining;

            float totalMoveSeconds = Mathf.Max(0.01f, timeManager.secondsPerDay * army.movementDaysTotal);

            army.movementProgress += Time.deltaTime / totalMoveSeconds;
            army.movementProgress = Mathf.Clamp01(army.movementProgress);

            army.movementDaysRemaining = Mathf.Max(
                0,
                Mathf.CeilToInt((1f - army.movementProgress) * army.movementDaysTotal)
            );

            if (army.movementProgress >= 1f)
            {
                FinishMovement(army);
                arrivedBuffer.Add(army);
                continue;
            }

            if (army.movementDaysRemaining != oldRemainingDays)
                OnArmyChanged?.Invoke(army);
        }

        foreach (ArmyData arrivedArmy in arrivedBuffer)
        {
            if (arrivedArmy == null)
                continue;

            if (!armiesById.ContainsKey(arrivedArmy.armyId))
                continue;

            OnArmyArrived?.Invoke(arrivedArmy);
        }
    }

    private void FinishMovement(ArmyData army)
    {
        army.currentProvinceId = army.targetProvinceId;
        army.sourceProvinceId = army.currentProvinceId;

        army.isMoving = false;
        army.movementDaysTotal = 0;
        army.movementDaysRemaining = 0;
        army.movementProgress = 1f;

        ProvinceData province = provinceManager.GetProvinceById(army.currentProvinceId);

        Debug.Log("Army " + army.armyId + " vardı: " + (province != null ? province.shapeName : "Unknown"));

        OnArmyChanged?.Invoke(army);
    }

    public bool DestroyArmy(int armyId)
    {
        ArmyData army = GetArmy(armyId);

        if (army == null)
            return false;

        armiesById.Remove(armyId);

        army.isMoving = false;
        army.movementProgress = 1f;
        army.movementDaysRemaining = 0;
        army.movementDaysTotal = 0;

        Debug.Log("Army yok edildi. ID: " + army.armyId);

        OnArmyDestroyed?.Invoke(army);

        return true;
    }

    public bool SetArmyTroopCount(int armyId, int newTroopCount)
    {
        ArmyData army = GetArmy(armyId);

        if (army == null)
            return false;

        army.troopCount = Mathf.Max(0, newTroopCount);

        if (army.troopCount <= 0)
        {
            DestroyArmy(armyId);
            return true;
        }

        OnArmyChanged?.Invoke(army);
        return true;
    }

    public ArmyData GetFirstEnemyArmyInProvince(int provinceId, string requesterCountryTag)
    {
        foreach (ArmyData army in armiesById.Values)
        {
            if (army == null)
                continue;

            if (army.isMoving)
                continue;

            if (army.currentProvinceId != provinceId)
                continue;

            if (army.ownerCountryTag == requesterCountryTag)
                continue;

            if (army.troopCount <= 0)
                continue;

            return army;
        }

        return null;
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