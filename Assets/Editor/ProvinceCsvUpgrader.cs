using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ProvinceCsvUpgrader
{
    private static readonly string[] RequiredColumns =
    {
        "ownerCountry",
        "population",
        "recruitablePopulation",
        "economyValue",
        "infrastructure",
        "supplyLimit",
        "terrainType",
        "food",
        "steel",
        "coal",
        "oil",
        "aluminium",
        "chromium",
        "tungsten",
        "rubber",
        "civilianFactories",
        "militaryFactories",
        "dockyards",
        "refineries",
        "resistance",
        "compliance",
        "stationedTroops"
    };

    [MenuItem("Tools/LeagueStrategy/Upgrade Selected Province CSV")]
    public static void UpgradeSelectedCsv()
    {
        Object selectedObject = Selection.activeObject;

        if (selectedObject == null)
        {
            Debug.LogError("Önce Project panelinden province_data.csv dosyasını seç.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(selectedObject);

        if (string.IsNullOrEmpty(path) || !path.EndsWith(".csv"))
        {
            Debug.LogError("Seçilen dosya CSV değil.");
            return;
        }

        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        if (lines.Length <= 1)
        {
            Debug.LogError("CSV boş.");
            return;
        }

        char delimiter = DetectDelimiter(lines[0]);

        List<string> headers = new List<string>(lines[0].Split(delimiter));

        for (int i = 0; i < headers.Count; i++)
        {
            headers[i] = Clean(headers[i]);
        }

        List<string> missingColumns = new List<string>();

        foreach (string requiredColumn in RequiredColumns)
        {
            if (!headers.Contains(requiredColumn))
            {
                headers.Add(requiredColumn);
                missingColumns.Add(requiredColumn);
            }
        }

        List<string> outputLines = new List<string>();
        outputLines.Add(string.Join(delimiter.ToString(), headers));

        int shapeNameIndex = headers.IndexOf("shapeName");
        int shapeGroupIndex = headers.IndexOf("shapeGroup");

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrEmpty(line))
                continue;

            List<string> values = new List<string>(line.Split(delimiter));

            while (values.Count < headers.Count)
            {
                values.Add("");
            }

            string provinceName = GetValue(values, shapeNameIndex);
            string countryTag = GetValue(values, shapeGroupIndex);

            FillIfEmpty(values, headers, "ownerCountry", countryTag);
            FillIfEmpty(values, headers, "population", GetDefaultPopulation(provinceName, countryTag).ToString());
            FillIfEmpty(values, headers, "recruitablePopulation", GetDefaultRecruitablePopulation(provinceName, countryTag).ToString());
            FillIfEmpty(values, headers, "economyValue", GetDefaultEconomy(provinceName).ToString());
            FillIfEmpty(values, headers, "infrastructure", GetDefaultInfrastructure(provinceName).ToString());
            FillIfEmpty(values, headers, "supplyLimit", GetDefaultSupply(provinceName).ToString());
            FillIfEmpty(values, headers, "terrainType", GetDefaultTerrain(provinceName));

            FillIfEmpty(values, headers, "food", "5");
            FillIfEmpty(values, headers, "steel", "0");
            FillIfEmpty(values, headers, "coal", "0");
            FillIfEmpty(values, headers, "oil", "0");
            FillIfEmpty(values, headers, "aluminium", "0");
            FillIfEmpty(values, headers, "chromium", "0");
            FillIfEmpty(values, headers, "tungsten", "0");
            FillIfEmpty(values, headers, "rubber", "0");

            FillIfEmpty(values, headers, "civilianFactories", GetDefaultCivilianFactories(provinceName).ToString());
            FillIfEmpty(values, headers, "militaryFactories", GetDefaultMilitaryFactories(provinceName).ToString());
            FillIfEmpty(values, headers, "dockyards", GetDefaultDockyards(provinceName).ToString());
            FillIfEmpty(values, headers, "refineries", "0");

            FillIfEmpty(values, headers, "resistance", "0");
            FillIfEmpty(values, headers, "compliance", "0");
            FillIfEmpty(values, headers, "stationedTroops", "0");

            outputLines.Add(string.Join(delimiter.ToString(), values));
        }

        string backupPath = path.Replace(".csv", "_backup.csv");
        File.Copy(path, backupPath, true);

        File.WriteAllLines(path, outputLines, Encoding.UTF8);

        AssetDatabase.Refresh();

        Debug.Log("CSV güncellendi. Backup oluşturuldu: " + backupPath);
    }

    private static char DetectDelimiter(string headerLine)
    {
        int commaCount = headerLine.Split(',').Length;
        int semicolonCount = headerLine.Split(';').Length;

        return semicolonCount > commaCount ? ';' : ',';
    }

    private static string Clean(string value)
    {
        return value.Trim().Replace("\uFEFF", "");
    }

    private static string GetValue(List<string> values, int index)
    {
        if (index < 0 || index >= values.Count)
            return "";

        return values[index].Trim();
    }

    private static void FillIfEmpty(List<string> values, List<string> headers, string columnName, string defaultValue)
    {
        int index = headers.IndexOf(columnName);

        if (index < 0)
            return;

        while (values.Count <= index)
        {
            values.Add("");
        }

        if (string.IsNullOrEmpty(values[index]))
        {
            values[index] = defaultValue;
        }
    }

    private static int GetDefaultPopulation(string provinceName, string countryTag)
    {
        if (IsMajorCity(provinceName))
            return 1500000;

        if (countryTag == "TUR")
            return 500000;

        if (countryTag == "GRC")
            return 300000;

        if (countryTag == "BGR")
            return 250000;

        return 200000;
    }

    private static int GetDefaultRecruitablePopulation(string provinceName, string countryTag)
    {
        return Mathf.RoundToInt(GetDefaultPopulation(provinceName, countryTag) * 0.20f);
    }

    private static int GetDefaultEconomy(string provinceName)
    {
        if (IsMajorCity(provinceName))
            return 100;

        return 30;
    }

    private static int GetDefaultInfrastructure(string provinceName)
    {
        if (IsMajorCity(provinceName))
            return 80;

        return 50;
    }

    private static int GetDefaultSupply(string provinceName)
    {
        return GetDefaultInfrastructure(provinceName) * 20;
    }

    private static string GetDefaultTerrain(string provinceName)
    {
        return "Plains";
    }

    private static int GetDefaultCivilianFactories(string provinceName)
    {
        if (IsMajorCity(provinceName))
            return 3;

        return 1;
    }

    private static int GetDefaultMilitaryFactories(string provinceName)
    {
        if (IsMajorCity(provinceName))
            return 2;

        return 0;
    }

    private static int GetDefaultDockyards(string provinceName)
    {
        if (provinceName == "Istanbul" || provinceName == "Izmir" || provinceName == "Athens")
            return 2;

        return 0;
    }

    private static bool IsMajorCity(string provinceName)
    {
        return provinceName == "Istanbul" ||
               provinceName == "Ankara" ||
               provinceName == "Izmir" ||
               provinceName == "Athens" ||
               provinceName == "Sofia";
    }
}