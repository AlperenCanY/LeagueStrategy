using System;
using System.Collections.Generic;
using UnityEngine;

public class CountryManager : MonoBehaviour
{
    public ProvinceManager provinceManager;

    public event Action OnCountriesChanged;
    public event Action OnProvinceOwnershipChanged;

    private Dictionary<string, CountryData> countriesByTag = new Dictionary<string, CountryData>();

    private void Start()
    {
        BuildCountriesFromProvinces();
    }

    private void BuildCountriesFromProvinces()
    {
        if (provinceManager == null)
        {
            Debug.LogError("CountryManager: ProvinceManager atanmadı.");
            return;
        }

        countriesByTag.Clear();

        foreach (ProvinceData province in provinceManager.GetAllProvinces())
        {
            string tag = province.ownerCountry;

            if (string.IsNullOrEmpty(tag))
                continue;

            if (!countriesByTag.ContainsKey(tag))
            {
                CountryData country = new CountryData();
                country.tag = tag;
                country.countryName = GetCountryNameFromTag(tag);
                country.money = 1000;
                country.manpower = 10000;
                country.mapColor = GetCountryColorFromTag(tag);

                countriesByTag[tag] = country;
            }

            if (!countriesByTag[tag].ownedProvinceIds.Contains(province.prov_id))
                countriesByTag[tag].ownedProvinceIds.Add(province.prov_id);
        }

        Debug.Log("Ülke yüklendi: " + countriesByTag.Count);

        foreach (CountryData country in countriesByTag.Values)
            Debug.Log(country.tag + " / Province: " + country.ProvinceCount);
    }

    private Color32 GetCountryColorFromTag(string tag)
    {
        switch (tag)
        {
            case "TUR":
                return new Color32(200, 40, 40, 130);

            case "GRC":
                return new Color32(40, 90, 220, 130);

            case "BGR":
                return new Color32(40, 170, 80, 130);

            default:
                return GenerateColorFromTag(tag);
        }
    }

    private Color32 GenerateColorFromTag(string tag)
    {
        int hash = tag.GetHashCode();

        byte r = (byte)(80 + Mathf.Abs(hash % 120));
        byte g = (byte)(80 + Mathf.Abs((hash / 10) % 120));
        byte b = (byte)(80 + Mathf.Abs((hash / 100) % 120));

        return new Color32(r, g, b, 130);
    }

    private string GetCountryNameFromTag(string tag)
    {
        switch (tag)
        {
            case "TUR":
                return "Türkiye";
            case "GRC":
                return "Yunanistan";
            case "BGR":
                return "Bulgaristan";
            default:
                return tag;
        }
    }

    public CountryData GetCountry(string tag)
    {
        countriesByTag.TryGetValue(tag, out CountryData country);
        return country;
    }

    public IEnumerable<CountryData> GetAllCountries()
    {
        return countriesByTag.Values;
    }

    public void NotifyCountriesChanged()
    {
        OnCountriesChanged?.Invoke();
    }

    public void TransferProvince(ProvinceData province, string newOwnerTag)
    {
        if (province == null)
            return;

        if (string.IsNullOrEmpty(newOwnerTag))
            return;

        string oldOwnerTag = province.ownerCountry;

        if (oldOwnerTag == newOwnerTag)
            return;

        CountryData oldOwner = GetCountry(oldOwnerTag);
        CountryData newOwner = GetCountry(newOwnerTag);

        if (oldOwner != null)
            oldOwner.ownedProvinceIds.Remove(province.prov_id);

        if (newOwner != null && !newOwner.ownedProvinceIds.Contains(province.prov_id))
            newOwner.ownedProvinceIds.Add(province.prov_id);

        province.ownerCountry = newOwnerTag;

        Debug.Log(province.shapeName + " province sahibi değişti: " + oldOwnerTag + " -> " + newOwnerTag);

        NotifyCountriesChanged();
        OnProvinceOwnershipChanged?.Invoke();
    }
    private void CalculateCountryStats()
{
    foreach (CountryData country in countries.Values)
    {
        country.totalPopulation = 0;
        country.totalRecruitablePopulation = 0;

        country.totalFood = 0;
        country.totalSteel = 0;
        country.totalCoal = 0;
        country.totalOil = 0;
        country.totalAluminium = 0;
        country.totalChromium = 0;
        country.totalTungsten = 0;
        country.totalRubber = 0;

        country.civilianFactories = 0;
        country.militaryFactories = 0;
        country.dockyards = 0;
        country.refineries = 0;

        foreach (ProvinceData province in country.provinces)
        {
            country.totalPopulation += province.population;
            country.totalRecruitablePopulation += province.recruitablePopulation;

            country.totalFood += province.food;
            country.totalSteel += province.steel;
            country.totalCoal += province.coal;
            country.totalOil += province.oil;
            country.totalAluminium += province.aluminium;
            country.totalChromium += province.chromium;
            country.totalTungsten += province.tungsten;
            country.totalRubber += province.rubber;

            country.civilianFactories += province.civilianFactories;
            country.militaryFactories += province.militaryFactories;
            country.dockyards += province.dockyards;
            country.refineries += province.refineries;
        }

        Debug.Log(
            $"{country.countryTag} | Pop:{country.totalPopulation} | MP:{country.totalRecruitablePopulation} | " +
            $"Food:{country.totalFood} Steel:{country.totalSteel} Oil:{country.totalOil} | " +
            $"Civ:{country.civilianFactories} Mil:{country.militaryFactories}"
        );
    }
}
}