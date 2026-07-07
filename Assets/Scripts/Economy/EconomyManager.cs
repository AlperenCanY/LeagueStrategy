using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public TimeManager timeManager;
    public CountryManager countryManager;

    [Header("Economy Settings")]
    public int moneyPerProvincePerDay = 2;
    public int manpowerPerProvincePerDay = 5;

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

        foreach (CountryData country in countryManager.GetAllCountries())
        {
            int income = country.ProvinceCount * moneyPerProvincePerDay;
            int manpowerGain = country.ProvinceCount * manpowerPerProvincePerDay;

            country.dailyIncome = income;
            country.dailyManpowerGain = manpowerGain;

            country.money += income;
            country.manpower += manpowerGain;
        }

        countryManager.NotifyCountriesChanged();
    }
}