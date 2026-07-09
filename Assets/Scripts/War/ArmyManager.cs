using System;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager : MonoBehaviour
{
    [Header("References")]
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

    private readonly Dictionary<int, ArmyData> armiesById = new Dictionary<int, ArmyData>();

    private readonly List<ArmyData> tickBuffer = new List<ArmyData>();
    private readonly List<ArmyData> arrivedBuffer = new List<ArmyData>();

    public event Action<ArmyData> OnArmyCreated;
    public event Action<ArmyData> OnArmyChanged;
    public event Action<ArmyData> OnArmyArrived;
    public event Action<ArmyData> OnArmyDestroyed;

    private void Update()
    {
        TickArmyMovement();
    }

    // =========================
    // Recruitment
    // =========================

    public bool CanRecruitArmy(int provinceId, string requesterCountryTag, out string reason)
    {
        reason = "";

        if (provinceManager == null || countryManager == null)
        {
            reason = "System missing";
            return false;
        }

        ProvinceData province = provinceManager.GetProvinceById(provinceId);

        if (province == null)
        {
            reason = "Province not found";
            return false;
        }

        if (province.ownerCountry != requesterCountryTag)
        {
            reason = "Not your province";
            return false;
        }

        CountryData country = countryManager.GetCountry(requesterCountryTag);

        if (country == null)
        {
            reason = "Country not found";
            return false;
        }

        if (province.recruitablePopulation < recruitAmount)
        {
            reason = "Low recruitable population";
            return false;
        }

        if (country.manpower < manpowerCost)
        {
            reason = "Low manpower";
            return false;
        }

        if (country.money < moneyCost)
        {
            reason = "Low money";
            return false;
        }

        reason = "Ready";
        return true;
    }

    public ArmyData RecruitArmy(int provinceId, string requesterCountryTag)
    {
        if (!CanRecruitArmy(provinceId, requesterCountryTag, out string reason))
        {
            Debug.Log("Recruit başarısız: " + reason);
            return null;
        }

        ProvinceData province = provinceManager.GetProvinceById(provinceId);
        CountryData country = countryManager.GetCountry(requesterCountryTag);

        if (province == null || country == null)
        {
            Debug.LogError("ArmyManager: Recruit sırasında province veya country null geldi.");
            return null;
        }

        country.manpower -= manpowerCost;
        country.money -= moneyCost;
        province.recruitablePopulation -= recruitAmount;

        ArmyData army = CreateArmy(requesterCountryTag, provinceId, recruitAmount);

        countryManager.NotifyCountriesChanged();

        Debug.Log(
            "Army oluşturuldu. ID: " + army.armyId +
            " / Province: " + province.shapeName +
            " / Troops: " + recruitAmount +
            " / Cost: $" + moneyCost +
            " / Manpower: " + manpowerCost
        );

        OnArmyCreated?.Invoke(army);
        OnArmyChanged?.Invoke(army);

        return army;
    }

    private ArmyData CreateArmy(string ownerCountryTag, int provinceId, int troopCount)
    {
        ArmyData army = new ArmyData(
            nextArmyId,
            ownerCountryTag,
            provinceId,
            troopCount
        );

        nextArmyId++;
        armiesById[army.armyId] = army;

        return army;
    }

    // =========================
    // Movement
    // =========================

    public bool StartMoveArmy(int armyId, int targetProvinceId)
    {
        if (provinceManager == null)
        {
            Debug.LogError("ArmyManager: ProvinceManager atanmadı.");
            return false;
        }

        ArmyData army = GetArmy(armyId);

        if (army == null)
        {
            Debug.LogWarning("ArmyManager: Army bulunamadı. ID: " + armyId);
            return false;
        }

        if (army.isMoving)
        {
            Debug.Log("Army zaten hareket ediyor.");
            return false;
        }

        ProvinceData targetProvince = provinceManager.GetProvinceById(targetProvinceId);

        if (targetProvince == null)
        {
            Debug.LogWarning("ArmyManager: Hedef province bulunamadı. ID: " + targetProvinceId);
            return false;
        }

        if (army.currentProvinceId == targetProvinceId)
        {
            Debug.Log("Army zaten bu province içinde.");
            return false;
        }

        StartMovement(army, targetProvinceId);

        Debug.Log("Army " + army.armyId + " hareket ediyor -> " + targetProvince.shapeName);

        OnArmyChanged?.Invoke(army);

        return true;
    }

    private void StartMovement(ArmyData army, int targetProvinceId)
    {
        int moveDays = Mathf.Max(1, defaultMovementDays);

        army.isMoving = true;
        army.sourceProvinceId = army.currentProvinceId;
        army.targetProvinceId = targetProvinceId;
        army.movementDaysTotal = moveDays;
        army.movementDaysRemaining = moveDays;
        army.movementProgress = 0f;
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
        {
            tickBuffer.Add(army);
        }

        foreach (ArmyData army in tickBuffer)
        {
            if (army == null)
                continue;

            if (!armiesById.ContainsKey(army.armyId))
                continue;

            if (!army.isMoving)
                continue;

            TickSingleArmyMovement(army);
        }

        FireArrivedEvents();
    }

    private void TickSingleArmyMovement(ArmyData army)
    {
        int oldRemainingDays = army.movementDaysRemaining;

        float totalMoveSeconds = Mathf.Max(
            0.01f,
            timeManager.secondsPerDay * army.movementDaysTotal
        );

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
            return;
        }

        if (army.movementDaysRemaining != oldRemainingDays)
        {
            OnArmyChanged?.Invoke(army);
        }
    }

    private void FinishMovement(ArmyData army)
    {
        army.currentProvinceId = army.targetProvinceId;
        army.sourceProvinceId = army.currentProvinceId;
        army.targetProvinceId = army.currentProvinceId;

        army.isMoving = false;
        army.movementDaysTotal = 0;
        army.movementDaysRemaining = 0;
        army.movementProgress = 1f;

        ProvinceData province = provinceManager != null
            ? provinceManager.GetProvinceById(army.currentProvinceId)
            : null;

        Debug.Log(
            "Army " + army.armyId +
            " vardı: " + (province != null ? province.shapeName : "Unknown")
        );

        OnArmyChanged?.Invoke(army);
    }

    private void FireArrivedEvents()
    {
        foreach (ArmyData arrivedArmy in arrivedBuffer)
        {
            if (arrivedArmy == null)
                continue;

            if (!armiesById.ContainsKey(arrivedArmy.armyId))
                continue;

            OnArmyArrived?.Invoke(arrivedArmy);
        }
    }

    // =========================
    // Army State
    // =========================

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

    // =========================
    // Queries
    // =========================

    public ArmyData GetArmy(int armyId)
    {
        armiesById.TryGetValue(armyId, out ArmyData army);
        return army;
    }

    public IEnumerable<ArmyData> GetAllArmies()
    {
        return armiesById.Values;
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
}