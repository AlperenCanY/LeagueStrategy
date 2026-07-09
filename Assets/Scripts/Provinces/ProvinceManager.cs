using System.Collections.Generic;
using UnityEngine;

public class ProvinceManager : MonoBehaviour
{
    public TextAsset provinceCsv;

    private Dictionary<int, ProvinceData> provincesById = new Dictionary<int, ProvinceData>();

    private void Awake()
    {
        LoadCsv();
    }

private void LoadCsv()
{
    Debug.Log("LoadCsv başladı");

    if (provinceCsv == null)
        {
            Debug.LogError("Province CSV atanmadı.");
            return;
        }

        string[] lines = provinceCsv.text.Split('\n');

        if (lines.Length <= 1)
        {
            Debug.LogError("CSV boş veya okunamadı.");
            return;
        }

        char delimiter = DetectDelimiter(lines[0]);
        string[] headers = ParseCsvLine(lines[0], delimiter);

        int idIndex = GetHeaderIndex(headers, "prov_id");
        int nameIndex = GetHeaderIndex(headers, "shapeName");
        int groupIndex = GetHeaderIndex(headers, "shapeGroup");
        int shapeIdIndex = GetHeaderIndex(headers, "shapeID");
        int populationIndex = GetOptionalHeaderIndex(headers, "population");
int economyIndex = GetOptionalHeaderIndex(headers, "economyValue");
int infrastructureIndex = GetOptionalHeaderIndex(headers, "infrastructure");
int supplyIndex = GetOptionalHeaderIndex(headers, "supplyLimit");
int terrainIndex = GetOptionalHeaderIndex(headers, "terrainType");

        if (idIndex == -1)
        {
            Debug.LogError("CSV içinde prov_id kolonu yok.");
            return;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = ParseCsvLine(line, delimiter);

            ProvinceData province = new ProvinceData();

            if (!int.TryParse(GetValue(values, idIndex), out province.prov_id))
                continue;

            province.shapeName = GetValue(values, nameIndex);
            province.shapeGroup = GetValue(values, groupIndex);
            province.shapeID = GetValue(values, shapeIdIndex);
province.ownerCountry = province.shapeGroup;
province.population = GetOptionalInt(values, populationIndex, GenerateDefaultPopulation(province));
province.economyValue = GetOptionalInt(values, economyIndex, GenerateDefaultEconomy(province));
province.infrastructure = GetOptionalInt(values, infrastructureIndex, 50);
province.supplyLimit = GetOptionalInt(values, supplyIndex, GenerateDefaultSupply(province));
province.terrainType = GetOptionalString(values, terrainIndex, "Plains");

// TEST VERİLERİ
province.population = Random.Range(50000, 500000);

province.recruitablePopulation =
    Mathf.RoundToInt(province.population * 0.20f);

province.food = Random.Range(2, 15);
province.steel = Random.Range(0, 8);
province.coal = Random.Range(0, 8);
province.oil = Random.Range(0, 5);
province.aluminium = Random.Range(0, 4);
province.chromium = Random.Range(0, 3);
province.tungsten = Random.Range(0, 3);
province.rubber = Random.Range(0, 2);

province.civilianFactories = Random.Range(0, 3);
province.militaryFactories = Random.Range(0, 2);
province.dockyards = 0;
province.refineries = 0;

// Province'yi listeye ekle
provincesById[province.prov_id] = province;

Debug.Log("Province yüklendi: " + province.shapeName);
        }

        Debug.Log("Province yüklendi: " + provincesById.Count);
    }

    private char DetectDelimiter(string headerLine)
    {
        int commaCount = headerLine.Split(',').Length;
        int semicolonCount = headerLine.Split(';').Length;

        return semicolonCount > commaCount ? ';' : ',';
    }

    private string[] ParseCsvLine(string line, char delimiter)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == delimiter && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);
        return result.ToArray();
    }

    private int GetHeaderIndex(string[] headers, string headerName)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            string cleanHeader = headers[i].Trim().Replace("\uFEFF", "");

            if (cleanHeader == headerName)
                return i;
        }

        Debug.LogError("CSV içinde kolon bulunamadı: " + headerName);
        return -1;
    }

    private string GetValue(string[] values, int index)
    {
        if (index < 0 || index >= values.Length)
            return "";

        return values[index].Trim();
    }

    public ProvinceData GetProvinceById(int id)
    {
        provincesById.TryGetValue(id, out ProvinceData province);
        return province;
    }

    public bool HasProvince(int id)
    {
        return provincesById.ContainsKey(id);
    }
    public IEnumerable<ProvinceData> GetAllProvinces()
{
    return provincesById.Values;
}

private int GetOptionalHeaderIndex(string[] headers, string headerName)
{
    for (int i = 0; i < headers.Length; i++)
    {
        string cleanHeader = headers[i].Trim().Replace("\uFEFF", "");

        if (cleanHeader == headerName)
            return i;
    }

    return -1;
}

private int GetOptionalInt(string[] values, int index, int defaultValue)
{
    if (index < 0 || index >= values.Length)
        return defaultValue;

    if (int.TryParse(values[index].Trim(), out int result))
        return result;

    return defaultValue;
}

private string GetOptionalString(string[] values, int index, string defaultValue)
{
    if (index < 0 || index >= values.Length)
        return defaultValue;

    string value = values[index].Trim();

    if (string.IsNullOrEmpty(value))
        return defaultValue;

    return value;
}

private int GenerateDefaultPopulation(ProvinceData province)
{
    if (province.shapeGroup == "TUR")
        return 500000;

    if (province.shapeGroup == "GRC")
        return 300000;

    if (province.shapeGroup == "BGR")
        return 250000;

    return 200000;
}

private int GenerateDefaultEconomy(ProvinceData province)
{
    if (province.shapeName == "Istanbul")
        return 100;

    if (province.shapeName == "Ankara")
        return 80;

    if (province.shapeName == "Izmir")
        return 85;

    return 30;
}

private int GenerateDefaultSupply(ProvinceData province)
{
    return province.infrastructure * 20;
}
}