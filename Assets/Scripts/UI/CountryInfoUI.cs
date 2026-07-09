using UnityEngine;
using UnityEngine.UIElements;

public class CountryInfoUI : MonoBehaviour
{
    [Header("References")]
    public PlayerState playerState;
    public CountryManager countryManager;
    public CountryStatsCalculator statsCalculator;

    private VisualElement panel;

    private Label countryLabel;

    private Label moneyLabel;
    private Label manpowerLabel;

    private Label provinceCountLabel;
    private Label populationLabel;
    private Label recruitablePopulationLabel;

    private Label economyLabel;
    private Label supplyLabel;
    private Label infrastructureLabel;

    private Label resourcesLabel;
    private Label rareResourcesLabel;
    private Label industryLabel;

    private bool isVisible = true;

    private void OnEnable()
    {
        BuildUI();
        SubscribeEvents();

        Invoke(nameof(Refresh), 0.1f);
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void BuildUI()
    {
        UIDocument document = GetComponent<UIDocument>();

        if (document == null)
        {
            Debug.LogError("CountryInfoUI: UIDocument eksik.");
            return;
        }

        VisualElement root = document.rootVisualElement;
        root.Clear();

        panel = CreatePanel();

        Label title = CreateTitle("Country Info");

        countryLabel = CreateLabel("Country: -");

        moneyLabel = CreateLabel("Money: -");
        manpowerLabel = CreateLabel("Manpower: -");

        provinceCountLabel = CreateLabel("Provinces: -");
        populationLabel = CreateLabel("Population: -");
        recruitablePopulationLabel = CreateLabel("Recruitable Population: -");

        economyLabel = CreateLabel("Economy: -");
        supplyLabel = CreateLabel("Supply: -");
        infrastructureLabel = CreateLabel("Avg Infrastructure: -");

        resourcesLabel = CreateLabel("Resources: -");
        rareResourcesLabel = CreateLabel("Rare Resources: -");
        industryLabel = CreateLabel("Industry: -");

        panel.Add(title);
        panel.Add(countryLabel);

        panel.Add(CreateSpacer(6));
        panel.Add(CreateSectionTitle("Treasury"));
        panel.Add(moneyLabel);
        panel.Add(manpowerLabel);

        panel.Add(CreateSpacer(6));
        panel.Add(CreateSectionTitle("Population"));
        panel.Add(provinceCountLabel);
        panel.Add(populationLabel);
        panel.Add(recruitablePopulationLabel);

        panel.Add(CreateSpacer(6));
        panel.Add(CreateSectionTitle("Province Power"));
        panel.Add(economyLabel);
        panel.Add(supplyLabel);
        panel.Add(infrastructureLabel);

        panel.Add(CreateSpacer(6));
        panel.Add(CreateSectionTitle("Resources"));
        panel.Add(resourcesLabel);
        panel.Add(rareResourcesLabel);

        panel.Add(CreateSpacer(6));
        panel.Add(CreateSectionTitle("Industry"));
        panel.Add(industryLabel);

        root.Add(panel);
    }

    private VisualElement CreatePanel()
    {
        VisualElement element = new VisualElement();

        element.style.position = Position.Absolute;
        element.style.left = 20;
        element.style.top = 70;
        element.style.width = 360;

        element.style.paddingTop = 14;
        element.style.paddingBottom = 14;
        element.style.paddingLeft = 14;
        element.style.paddingRight = 14;

        element.style.backgroundColor = new Color(0f, 0f, 0f, 0.76f);

        return element;
    }

    private Label CreateTitle(string text)
    {
        Label label = new Label(text);

        label.style.fontSize = 21;
        label.style.color = Color.white;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.marginBottom = 8;

        return label;
    }

    private Label CreateSectionTitle(string text)
    {
        Label label = new Label(text);

        label.style.fontSize = 15;
        label.style.color = new Color(1f, 0.82f, 0.35f);
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.marginTop = 2;
        label.style.marginBottom = 4;

        return label;
    }

    private Label CreateLabel(string text)
    {
        Label label = new Label(text);

        label.style.fontSize = 15;
        label.style.color = Color.white;
        label.style.marginBottom = 4;
        label.style.whiteSpace = WhiteSpace.Normal;

        return label;
    }

    private VisualElement CreateSpacer(int height)
    {
        VisualElement spacer = new VisualElement();
        spacer.style.height = height;
        return spacer;
    }

    private void SubscribeEvents()
    {
        if (countryManager != null)
        {
            countryManager.OnCountriesChanged += Refresh;
            countryManager.OnProvinceOwnershipChanged += Refresh;
        }
    }

    private void UnsubscribeEvents()
    {
        if (countryManager != null)
        {
            countryManager.OnCountriesChanged -= Refresh;
            countryManager.OnProvinceOwnershipChanged -= Refresh;
        }
    }

    public void Toggle()
    {
        isVisible = !isVisible;

        if (panel != null)
            panel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void Refresh()
    {
        if (panel == null)
            return;

        if (playerState == null)
        {
            Debug.LogWarning("CountryInfoUI: PlayerState atanmadı.");
            return;
        }

        if (statsCalculator == null)
        {
            Debug.LogWarning("CountryInfoUI: CountryStatsCalculator atanmadı.");
            return;
        }

        CountryData country = playerState.PlayerCountry;

        if (country == null)
            return;

        CountryStatsData stats = statsCalculator.Calculate(country);

        countryLabel.text = $"{country.countryName} ({country.tag})";

        moneyLabel.text = $"Money: {FormatNumber(country.money)}";
        manpowerLabel.text = $"Manpower: {FormatNumber(country.manpower)}";

        provinceCountLabel.text = $"Provinces: {stats.provinceCount}";
        populationLabel.text = $"Population: {FormatNumber(stats.totalPopulation)}";
        recruitablePopulationLabel.text = $"Recruitable: {FormatNumber(stats.totalRecruitablePopulation)}";

        economyLabel.text = $"Economy Value: {FormatNumber(stats.totalEconomyValue)}";
        supplyLabel.text = $"Total Supply: {FormatNumber(stats.totalSupplyLimit)}";
        infrastructureLabel.text = $"Avg Infrastructure: {stats.averageInfrastructure}";

        resourcesLabel.text =
            $"Food {stats.totalFood} | Steel {stats.totalSteel} | Coal {stats.totalCoal} | Oil {stats.totalOil}";

        rareResourcesLabel.text =
            $"Aluminium {stats.totalAluminium} | Chromium {stats.totalChromium} | Tungsten {stats.totalTungsten} | Rubber {stats.totalRubber}";

        industryLabel.text =
            $"Civ {stats.civilianFactories} | Mil {stats.militaryFactories} | Dock {stats.dockyards} | Ref {stats.refineries}";
    }

    private string FormatNumber(int value)
    {
        return value.ToString("N0");
    }
}