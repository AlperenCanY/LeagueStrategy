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
            stats.totalRecruitablePopulation += province.recruitablePopulation;

            stats.totalEconomyValue += province.economyValue;
            stats.totalSupplyLimit += province.supplyLimit;
            infrastructureSum += province.infrastructure;

            stats.totalFood += province.food;
            stats.totalSteel += province.steel;
            stats.totalCoal += province.coal;
            stats.totalOil += province.oil;
            stats.totalAluminium += province.aluminium;
            stats.totalChromium += province.chromium;
            stats.totalTungsten += province.tungsten;
            stats.totalRubber += province.rubber;

            stats.civilianFactories += province.civilianFactories;
            stats.militaryFactories += province.militaryFactories;
            stats.dockyards += province.dockyards;
            stats.refineries += province.refineries;
        }

        if (stats.provinceCount > 0)
            stats.averageInfrastructure = infrastructureSum / stats.provinceCount;

        return stats;
    }
}