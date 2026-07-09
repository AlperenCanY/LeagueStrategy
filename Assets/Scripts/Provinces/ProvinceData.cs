using UnityEngine;
[System.Serializable]
public class ProvinceData
{
    public int prov_id;
    public string shapeName;
    public string shapeGroup;
    public string shapeID;

    public string ownerCountry;

    public int stationedTroops;
    [Header("Population")]
public int population = 100000;
public int recruitablePopulation;

[Header("Resources")]
public int food;
public int steel;
public int coal;
public int oil;
public int aluminium;
public int chromium;
public int tungsten;
public int rubber;

[Header("Industry")]
public int civilianFactories;
public int militaryFactories;
public int dockyards;
public int refineries;

[Header("Province Stats")]
[Range(0,100)]
public int infrastructure = 50;

[Range(0,100)]
public int resistance = 0;

[Range(0,100)]
public int compliance = 0;
}