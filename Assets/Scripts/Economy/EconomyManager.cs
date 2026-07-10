using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [Header("References")]
    public TimeManager timeManager;
    public CountryManager countryManager;

    [Header("Money Settings")]
    public int economyValueDivisor = 5;
    public int moneyPerCivilianFactory = 4;
    public int moneyPerRefinery = 2;
    public float maxInfrastructureIncomeBonus = 0.30f;

    [Header("Manpower Settings")]
    public int recruitablePopulationDivisor = 120000;
    public int minimumManpowerGain = 1;

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

    private void HandleDayPassed(int day, int month, int year)
    {
        ApplyDailyEconomy();
    }

    private void ApplyDailyEconomy()
    {
        if (countryManager == null)
        {
            Debug.LogError("EconomyManager: CountryManager atanmadı.");
            return;
        }

        if (countryManager.provinceManager == null)
        {
            Debug.LogError("EconomyManager: CountryManager içinde ProvinceManager atanmadı.");
            return;
        }

        foreach (CountryData country in countryManager.GetAllCountries())
        {
            int dailyIncome = CalculateDailyIncome(country);
            int dailyManpowerGain = CalculateDailyManpowerGain(country);

            country.dailyIncome = dailyIncome;
            country.dailyManpowerGain = dailyManpowerGain;

            country.money += dailyIncome;
            country.manpower += dailyManpowerGain;
        }

        countryManager.NotifyCountriesChanged();
    }

    private int CalculateDailyIncome(CountryData country)
    {
        if (country == null)
            return 0;

        float totalIncome = 0f;

        foreach (int provinceId in country.ownedProvinceIds)
        {
            ProvinceData province = countryManager.provinceManager.GetProvinceById(provinceId);

            if (province == null)
                continue;

            totalIncome += CalculateProvinceIncome(province);
        }

        if (country.ownedProvinceIds.Count > 0)
            return Mathf.Max(1, Mathf.RoundToInt(totalIncome));

        return 0;
    }

    private float CalculateProvinceIncome(ProvinceData province)
    {
        if (province == null)
            return 0f;

        float economyIncome = province.economyValue / Mathf.Max(1f, economyValueDivisor);

        float factoryIncome =
            province.civilianFactories * moneyPerCivilianFactory +
            province.refineries * moneyPerRefinery;

        float infrastructureBonus = GetInfrastructureIncomeMultiplier(province.infrastructure);

        return (economyIncome + factoryIncome) * infrastructureBonus;
    }

    private float GetInfrastructureIncomeMultiplier(int infrastructure)
    {
        float normalizedInfrastructure = Mathf.Clamp01(infrastructure / 100f);

        return 1f + normalizedInfrastructure * maxInfrastructureIncomeBonus;
    }

    private int CalculateDailyManpowerGain(CountryData country)
    {
        if (country == null)
            return 0;

        int totalRecruitablePopulation = 0;

        foreach (int provinceId in country.ownedProvinceIds)
        {
            ProvinceData province = countryManager.provinceManager.GetProvinceById(provinceId);

            if (province == null)
                continue;

            totalRecruitablePopulation += province.recruitablePopulation;
        }

        int manpowerGain = totalRecruitablePopulation / Mathf.Max(1, recruitablePopulationDivisor);

        if (country.ownedProvinceIds.Count > 0)
            manpowerGain = Mathf.Max(minimumManpowerGain, manpowerGain);

        return manpowerGain;
    }
}