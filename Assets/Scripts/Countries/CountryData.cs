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
}