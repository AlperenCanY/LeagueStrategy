using UnityEngine;
using UnityEngine.UIElements;

public class CountryInfoUI : MonoBehaviour
{
    public PlayerState playerState;
    public CountryManager countryManager;
    public CountryStatsCalculator statsCalculator;

    private VisualElement panel;

    private Label countryLabel;
    private Label moneyLabel;
    private Label manpowerLabel;
    private Label provinceCountLabel;
    private Label populationLabel;
    private Label economyLabel;
    private Label supplyLabel;
    private Label infrastructureLabel;

    private bool isVisible = true;

    private void OnEnable()
    {
        UIDocument document = GetComponent<UIDocument>();

        if (document == null)
        {
            Debug.LogError("CountryInfoUI için UIDocument eksik.");
            return;
        }

        VisualElement root = document.rootVisualElement;
        root.Clear();

        panel = new VisualElement();
        panel.style.position = Position.Absolute;
        panel.style.left = 20;
        panel.style.top = 70;
        panel.style.width = 320;
        panel.style.paddingTop = 12;
        panel.style.paddingBottom = 12;
        panel.style.paddingLeft = 12;
        panel.style.paddingRight = 12;
        panel.style.backgroundColor = new Color(0f, 0f, 0f, 0.72f);

        Label title = new Label("Country Info");
        title.style.fontSize = 20;
        title.style.color = Color.white;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.marginBottom = 10;

        countryLabel = CreateLabel("Country: -");
        moneyLabel = CreateLabel("Money: -");
        manpowerLabel = CreateLabel("Manpower: -");
        provinceCountLabel = CreateLabel("Provinces: -");
        populationLabel = CreateLabel("Population: -");
        economyLabel = CreateLabel("Economy: -");
        supplyLabel = CreateLabel("Supply: -");
        infrastructureLabel = CreateLabel("Avg Infrastructure: -");

        panel.Add(title);
        panel.Add(countryLabel);
        panel.Add(moneyLabel);
        panel.Add(manpowerLabel);
        panel.Add(provinceCountLabel);
        panel.Add(populationLabel);
        panel.Add(economyLabel);
        panel.Add(supplyLabel);
        panel.Add(infrastructureLabel);

        root.Add(panel);

        if (countryManager != null)
            countryManager.OnCountriesChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (countryManager != null)
            countryManager.OnCountriesChanged -= Refresh;
    }

    private Label CreateLabel(string text)
    {
        Label label = new Label(text);
        label.style.fontSize = 16;
        label.style.color = Color.white;
        label.style.marginBottom = 5;
        return label;
    }

    public void Toggle()
    {
        isVisible = !isVisible;
        panel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void Refresh()
    {
        if (playerState == null || statsCalculator == null)
            return;

        CountryData country = playerState.PlayerCountry;

        if (country == null)
            return;

        CountryStatsData stats = statsCalculator.Calculate(country);

        countryLabel.text = "Country: " + country.countryName + " (" + country.tag + ")";
        moneyLabel.text = "Money: " + country.money;
        manpowerLabel.text = "Manpower: " + country.manpower;
        provinceCountLabel.text = "Provinces: " + stats.provinceCount;
        populationLabel.text = "Population: " + stats.totalPopulation;
        economyLabel.text = "Economy: " + stats.totalEconomyValue;
        supplyLabel.text = "Supply: " + stats.totalSupplyLimit;
        infrastructureLabel.text = "Avg Infrastructure: " + stats.averageInfrastructure;
    }
}