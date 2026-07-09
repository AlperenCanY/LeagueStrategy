using UnityEngine;

[System.Serializable]
public class ProvinceData
{
    [Header("Identity")]
    public int prov_id;
    public string shapeName;
    public string shapeGroup;
    public string shapeID;

    [Header("Ownership")]
    public string ownerCountry;

    [Header("Basic Province Stats")]
    public int population = 100000;
    public int recruitablePopulation;

    public int economyValue = 30;

    [Range(0, 100)]
    public int infrastructure = 50;

    public int supplyLimit = 1000;
    public string terrainType = "Plains";

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

    [Header("Control")]
    [Range(0, 100)]
    public int resistance = 0;

    [Range(0, 100)]
    public int compliance = 0;

    [Header("Legacy / Old System")]
    public int stationedTroops;
}