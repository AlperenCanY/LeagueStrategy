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
        Debug.Log("ProvinceManager: LoadCsv başladı.");

        provincesById.Clear();

        if (provinceCsv == null)
        {
            Debug.LogError("ProvinceManager: Province CSV atanmadı.");
            return;
        }

        string[] lines = provinceCsv.text.Split('\n');

        if (lines.Length <= 1)
        {
            Debug.LogError("ProvinceManager: CSV boş veya okunamadı.");
            return;
        }

        char delimiter = DetectDelimiter(lines[0]);
        string[] headers = ParseCsvLine(lines[0], delimiter);

        int idIndex = GetHeaderIndex(headers, "prov_id");
        int nameIndex = GetOptionalHeaderIndex(headers, "shapeName");
        int groupIndex = GetOptionalHeaderIndex(headers, "shapeGroup");
        int shapeIdIndex = GetOptionalHeaderIndex(headers, "shapeID");
        int ownerIndex = GetOptionalHeaderIndex(headers, "ownerCountry");

        int populationIndex = GetOptionalHeaderIndex(headers, "population");
        int recruitablePopulationIndex = GetOptionalHeaderIndex(headers, "recruitablePopulation");
        int economyIndex = GetOptionalHeaderIndex(headers, "economyValue", "economy");
        int infrastructureIndex = GetOptionalHeaderIndex(headers, "infrastructure");
        int supplyIndex = GetOptionalHeaderIndex(headers, "supplyLimit");
        int terrainIndex = GetOptionalHeaderIndex(headers, "terrainType", "terrain");

        int foodIndex = GetOptionalHeaderIndex(headers, "food");
        int steelIndex = GetOptionalHeaderIndex(headers, "steel");
        int coalIndex = GetOptionalHeaderIndex(headers, "coal");
        int oilIndex = GetOptionalHeaderIndex(headers, "oil");
        int aluminiumIndex = GetOptionalHeaderIndex(headers, "aluminium", "aluminum");
        int chromiumIndex = GetOptionalHeaderIndex(headers, "chromium");
        int tungstenIndex = GetOptionalHeaderIndex(headers, "tungsten");
        int rubberIndex = GetOptionalHeaderIndex(headers, "rubber");

        int civilianFactoriesIndex = GetOptionalHeaderIndex(headers, "civilianFactories");
        int militaryFactoriesIndex = GetOptionalHeaderIndex(headers, "militaryFactories");
        int dockyardsIndex = GetOptionalHeaderIndex(headers, "dockyards");
        int refineriesIndex = GetOptionalHeaderIndex(headers, "refineries");

        int resistanceIndex = GetOptionalHeaderIndex(headers, "resistance");
        int complianceIndex = GetOptionalHeaderIndex(headers, "compliance");
        int stationedTroopsIndex = GetOptionalHeaderIndex(headers, "stationedTroops");

        if (idIndex == -1)
        {
            Debug.LogError("ProvinceManager: CSV içinde prov_id kolonu yok.");
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

            province.shapeName = GetOptionalString(values, nameIndex, "Province " + province.prov_id);
            province.shapeGroup = GetOptionalString(values, groupIndex, "NEU");
            province.shapeID = GetOptionalString(values, shapeIdIndex, province.prov_id.ToString());

            province.ownerCountry = GetOptionalString(values, ownerIndex, province.shapeGroup);

            province.population = GetOptionalInt(values, populationIndex, GenerateDefaultPopulation(province));
            province.recruitablePopulation = GetOptionalInt(values, recruitablePopulationIndex, GenerateDefaultRecruitablePopulation(province));

            province.economyValue = GetOptionalInt(values, economyIndex, GenerateDefaultEconomy(province));
            province.infrastructure = GetOptionalInt(values, infrastructureIndex, GenerateDefaultInfrastructure(province));
            province.supplyLimit = GetOptionalInt(values, supplyIndex, GenerateDefaultSupply(province));
            province.terrainType = GetOptionalString(values, terrainIndex, GenerateDefaultTerrain(province));

            province.food = GetOptionalInt(values, foodIndex, GenerateDefaultFood(province));
            province.steel = GetOptionalInt(values, steelIndex, GenerateDefaultSteel(province));
            province.coal = GetOptionalInt(values, coalIndex, GenerateDefaultCoal(province));
            province.oil = GetOptionalInt(values, oilIndex, GenerateDefaultOil(province));
            province.aluminium = GetOptionalInt(values, aluminiumIndex, GenerateDefaultAluminium(province));
            province.chromium = GetOptionalInt(values, chromiumIndex, GenerateDefaultChromium(province));
            province.tungsten = GetOptionalInt(values, tungstenIndex, GenerateDefaultTungsten(province));
            province.rubber = GetOptionalInt(values, rubberIndex, GenerateDefaultRubber(province));

            province.civilianFactories = GetOptionalInt(values, civilianFactoriesIndex, GenerateDefaultCivilianFactories(province));
            province.militaryFactories = GetOptionalInt(values, militaryFactoriesIndex, GenerateDefaultMilitaryFactories(province));
            province.dockyards = GetOptionalInt(values, dockyardsIndex, GenerateDefaultDockyards(province));
            province.refineries = GetOptionalInt(values, refineriesIndex, 0);

            province.resistance = GetOptionalInt(values, resistanceIndex, 0);
            province.compliance = GetOptionalInt(values, complianceIndex, 0);
            province.stationedTroops = GetOptionalInt(values, stationedTroopsIndex, 0);

            provincesById[province.prov_id] = province;
        }

        Debug.Log("ProvinceManager: Province yüklendi: " + provincesById.Count);
    }

    private char DetectDelimiter(string headerLine)
    {
        int commaCount = CountCharacter(headerLine, ',');
        int semicolonCount = CountCharacter(headerLine, ';');

        return semicolonCount > commaCount ? ';' : ',';
    }

    private int CountCharacter(string text, char target)
    {
        int count = 0;

        foreach (char c in text)
        {
            if (c == target)
                count++;
        }

        return count;
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
            string cleanHeader = CleanHeader(headers[i]);

            if (cleanHeader == headerName)
                return i;
        }

        Debug.LogError("ProvinceManager: CSV içinde zorunlu kolon bulunamadı: " + headerName);
        return -1;
    }

    private int GetOptionalHeaderIndex(string[] headers, params string[] possibleNames)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            string cleanHeader = CleanHeader(headers[i]);

            foreach (string possibleName in possibleNames)
            {
                if (cleanHeader == possibleName)
                    return i;
            }
        }

        return -1;
    }

    private string CleanHeader(string header)
    {
        return header.Trim().Replace("\uFEFF", "");
    }

    private string GetValue(string[] values, int index)
    {
        if (index < 0 || index >= values.Length)
            return "";

        return values[index].Trim();
    }

    private int GetOptionalInt(string[] values, int index, int defaultValue)
    {
        if (index < 0 || index >= values.Length)
            return defaultValue;

        string value = values[index].Trim();

        if (string.IsNullOrEmpty(value))
            return defaultValue;

        if (int.TryParse(value, out int result))
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

    private int GenerateDefaultPopulation(ProvinceData province)
    {
        if (IsMajorCity(province.shapeName))
            return 1500000;

        if (province.shapeGroup == "TUR")
            return 500000;

        if (province.shapeGroup == "GRC")
            return 300000;

        if (province.shapeGroup == "BGR")
            return 250000;

        return 200000;
    }

    private int GenerateDefaultRecruitablePopulation(ProvinceData province)
    {
        return Mathf.RoundToInt(province.population * 0.20f);
    }

    private int GenerateDefaultEconomy(ProvinceData province)
    {
        if (IsMajorCity(province.shapeName))
            return 100;

        if (province.shapeName == "Ankara")
            return 80;

        if (province.shapeName == "Izmir")
            return 85;

        return 30;
    }

    private int GenerateDefaultInfrastructure(ProvinceData province)
    {
        if (IsMajorCity(province.shapeName))
            return 80;

        return 50;
    }

    private int GenerateDefaultSupply(ProvinceData province)
    {
        return province.infrastructure * 20;
    }

    private string GenerateDefaultTerrain(ProvinceData province)
    {
        return "Plains";
    }

    private int GenerateDefaultFood(ProvinceData province)
    {
        return 5;
    }

    private int GenerateDefaultSteel(ProvinceData province)
    {
        return 0;
    }

    private int GenerateDefaultCoal(ProvinceData province)
    {
        return 0;
    }

    private int GenerateDefaultOil(ProvinceData province)
    {
        return 0;
    }

    private int GenerateDefaultAluminium(ProvinceData province)
    {
        return 0;
    }

    private int GenerateDefaultChromium(ProvinceData province)
    {
        return 0;
    }

    private int GenerateDefaultTungsten(ProvinceData province)
    {
        return 0;
    }

    private int GenerateDefaultRubber(ProvinceData province)
    {
        return 0;
    }

    private int GenerateDefaultCivilianFactories(ProvinceData province)
    {
        if (IsMajorCity(province.shapeName))
            return 3;

        return 1;
    }

    private int GenerateDefaultMilitaryFactories(ProvinceData province)
    {
        if (IsMajorCity(province.shapeName))
            return 2;

        return 0;
    }

    private int GenerateDefaultDockyards(ProvinceData province)
    {
        if (province.shapeName == "Istanbul" || province.shapeName == "Izmir" || province.shapeName == "Athens")
            return 2;

        return 0;
    }

    private bool IsMajorCity(string provinceName)
    {
        return provinceName == "Istanbul" ||
               provinceName == "Ankara" ||
               provinceName == "Izmir" ||
               provinceName == "Athens" ||
               provinceName == "Sofia";
    }
}