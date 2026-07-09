using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CountryData
{
    public string tag;
    public string countryName;

    public int money;
    public int manpower;

    public int dailyIncome;
    public int dailyManpowerGain;

    public Color32 mapColor;

    public List<int> ownedProvinceIds = new List<int>();

    public int ProvinceCount
    {
        get { return ownedProvinceIds.Count; }
    }
    // TOPLAM DEĞERLER
public int totalPopulation;
public int totalRecruitablePopulation;

public int totalFood;
public int totalSteel;
public int totalCoal;
public int totalOil;
public int totalAluminium;
public int totalChromium;
public int totalTungsten;
public int totalRubber;

public int civilianFactories;
public int militaryFactories;
public int dockyards;
public int refineries;
}