using UnityEngine;

public class CountryStatsCalculator : MonoBehaviour
{
    public ProvinceManager provinceManager;

    public CountryStatsData Calculate(CountryData country)
    {
        CountryStatsData stats = new CountryStatsData();

        if (country == null || provinceManager == null)
            return stats;

        stats.provinceCount = country.ownedProvinceIds.Count;

        int infrastructureSum = 0;

        foreach (int provinceId in country.ownedProvinceIds)
        {
            ProvinceData province = provinceManager.GetProvinceById(provinceId);

            if (province == null)
                continue;

            stats.totalPopulation += province.population;
            stats.totalEconomyValue += province.economyValue;
            stats.totalSupplyLimit += province.supplyLimit;
            infrastructureSum += province.infrastructure;
        }

        if (stats.provinceCount > 0)
        {
            stats.averageInfrastructure = infrastructureSum / stats.provinceCount;
        }

        return stats;
    }
}