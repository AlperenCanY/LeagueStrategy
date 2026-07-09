using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ProvinceInfoUI : MonoBehaviour
{
    [Header("References")]
    public SelectionManager selectionManager;

    public event Action OnRecruitButtonClicked;

    private VisualElement panel;

    private Label provinceNameLabel;
    private Label ownerLabel;
    private Label tagLabel;
    private Label idLabel;
    public PlayerState playerState;
public ArmyManager armyManager;
private Label recruitStatusLabel;

    private Label populationLabel;
    private Label recruitablePopulationLabel;
    private Label economyLabel;
    private Label infrastructureLabel;
    private Label supplyLabel;
    private Label terrainLabel;

    private Label resourcesLabel;
    private Label factoriesLabel;
    private Label controlLabel;

    private Label troopsLabel;

    private Label moneyLabel;
    private Label manpowerLabel;
    private Label provinceCountLabel;
    private Label dailyIncomeLabel;
    private Label dailyManpowerLabel;

    private Button recruitButton;

    private void OnEnable()
    {
        BuildUI();
        SubscribeEvents();
        Hide();
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
            Debug.LogError("ProvinceInfoUI: UIDocument component eksik.");
            return;
        }

        VisualElement root = document.rootVisualElement;
        root.Clear();

        panel = CreatePanel();

        Label title = CreateTitle("Province Info");

        provinceNameLabel = CreateLabel("Province: -");
        ownerLabel = CreateLabel("Owner: -");
        tagLabel = CreateLabel("Tag: -");
        idLabel = CreateLabel("ID: -");

        populationLabel = CreateLabel("Population: -");
        recruitablePopulationLabel = CreateLabel("Recruitable Population: -");
        economyLabel = CreateLabel("Economy: -");
        infrastructureLabel = CreateLabel("Infrastructure: -");
        supplyLabel = CreateLabel("Supply: -");
        terrainLabel = CreateLabel("Terrain: -");

        resourcesLabel = CreateLabel("Resources: -");
        factoriesLabel = CreateLabel("Factories: -");
        controlLabel = CreateLabel("Control: -");

        troopsLabel = CreateLabel("Troops: -");
        recruitStatusLabel = CreateLabel("Recruit: -");

        moneyLabel = CreateLabel("Money: -");
        manpowerLabel = CreateLabel("Manpower: -");
        provinceCountLabel = CreateLabel("Owned Provinces: -");
        dailyIncomeLabel = CreateLabel("Daily Income: -");
        dailyManpowerLabel = CreateLabel("Daily Manpower: -");

        recruitButton = CreateRecruitButton();

        panel.Add(title);

        panel.Add(provinceNameLabel);
        panel.Add(ownerLabel);
        panel.Add(tagLabel);
        panel.Add(idLabel);

        panel.Add(CreateSpacer(6));

        panel.Add(populationLabel);
        panel.Add(recruitablePopulationLabel);
        panel.Add(economyLabel);
        panel.Add(infrastructureLabel);
        panel.Add(supplyLabel);
        panel.Add(terrainLabel);

        panel.Add(CreateSpacer(6));

        panel.Add(resourcesLabel);
        panel.Add(factoriesLabel);
        panel.Add(controlLabel);

        panel.Add(CreateSpacer(6));

        panel.Add(troopsLabel);

        panel.Add(CreateSpacer(6));

        panel.Add(moneyLabel);
        panel.Add(manpowerLabel);
        panel.Add(provinceCountLabel);
        panel.Add(dailyIncomeLabel);
        panel.Add(dailyManpowerLabel);

        panel.Add(recruitStatusLabel);
panel.Add(recruitButton);

        root.Add(panel);
    }

    private VisualElement CreatePanel()
    {
        VisualElement element = new VisualElement();

        element.style.position = Position.Absolute;
        element.style.right = 20;
        element.style.top = 20;
        element.style.width = 340;

        element.style.paddingTop = 12;
        element.style.paddingBottom = 12;
        element.style.paddingLeft = 12;
        element.style.paddingRight = 12;

        element.style.backgroundColor = new Color(0f, 0f, 0f, 0.72f);

        return element;
    }

    private Label CreateTitle(string text)
    {
        Label label = new Label(text);

        label.style.fontSize = 20;
        label.style.color = Color.white;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.marginBottom = 10;

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

    private Button CreateRecruitButton()
    {
        Button button = new Button();
        button.text = "Recruit 1000";

        button.style.marginTop = 10;
        button.style.height = 32;
        button.style.fontSize = 15;

        button.clicked += HandleRecruitClicked;

        return button;
    }

    private void SubscribeEvents()
    {
        if (selectionManager != null)
        {
            selectionManager.OnProvinceSelected += ShowSelection;
        }
    }

    private void UnsubscribeEvents()
    {
        if (selectionManager != null)
        {
            selectionManager.OnProvinceSelected -= ShowSelection;
        }

        if (recruitButton != null)
        {
            recruitButton.clicked -= HandleRecruitClicked;
        }
    }

    private void HandleRecruitClicked()
    {
        OnRecruitButtonClicked?.Invoke();
    }

    private void ShowSelection(ProvinceSelection selection)
    {
        if (selection == null || selection.province == null)
        {
            Hide();
            return;
        }

        ShowProvince(selection.province, selection.ownerCountry);
    }

    public void ShowProvince(ProvinceData province, CountryData ownerCountry)
    {
        if (province == null)
        {
            Hide();
            return;
        }

        UpdateProvinceLabels(province);
        UpdateCountryLabels(ownerCountry);

        Show();
    }

    private void UpdateProvinceLabels(ProvinceData province)
    {
        provinceNameLabel.text = $"Province: {province.shapeName}";
        tagLabel.text = $"Tag: {province.ownerCountry}";
        idLabel.text = $"ID: {province.prov_id}";

        populationLabel.text = $"Population: {province.population}";
        recruitablePopulationLabel.text = $"Recruitable Population: {province.recruitablePopulation}";
        economyLabel.text = $"Economy: {province.economyValue}";
        infrastructureLabel.text = $"Infrastructure: {province.infrastructure}";
        supplyLabel.text = $"Supply: {province.supplyLimit}";
        terrainLabel.text = $"Terrain: {province.terrainType}";

        resourcesLabel.text =
            $"Resources: Food {province.food} | Steel {province.steel} | Coal {province.coal} | Oil {province.oil}";

        factoriesLabel.text =
            $"Factories: Civ {province.civilianFactories} | Mil {province.militaryFactories} | Dock {province.dockyards} | Ref {province.refineries}";

        controlLabel.text =
            $"Control: Resistance {province.resistance}% | Compliance {province.compliance}%";

        troopsLabel.text = $"Troops: {province.stationedTroops}";
        UpdateRecruitButton(province);
    }

    private void UpdateCountryLabels(CountryData ownerCountry)
    {
        if (ownerCountry == null)
        {
            ownerLabel.text = "Owner: Unknown";
            moneyLabel.text = "Money: -";
            manpowerLabel.text = "Manpower: -";
            provinceCountLabel.text = "Owned Provinces: -";
            dailyIncomeLabel.text = "Daily Income: -";
            dailyManpowerLabel.text = "Daily Manpower: -";
            return;
        }

        ownerLabel.text = $"Owner: {ownerCountry.countryName}";
        moneyLabel.text = $"Money: {ownerCountry.money}";
        manpowerLabel.text = $"Manpower: {ownerCountry.manpower}";
        provinceCountLabel.text = $"Owned Provinces: {ownerCountry.ProvinceCount}";
        dailyIncomeLabel.text = $"Daily Income: {ownerCountry.dailyIncome}";
        dailyManpowerLabel.text = $"Daily Manpower: {ownerCountry.dailyManpowerGain}";
    }

    private void Show()
    {
        if (panel != null)
        {
            panel.style.display = DisplayStyle.Flex;
        }
    }
private void UpdateRecruitButton(ProvinceData province)
{
    if (recruitButton == null || recruitStatusLabel == null)
        return;

    if (province == null)
    {
        recruitButton.SetEnabled(false);
        recruitStatusLabel.text = "Recruit: -";
        return;
    }

    if (armyManager == null || playerState == null)
    {
        recruitButton.SetEnabled(true);
        recruitStatusLabel.text = "Recruit: System not linked";
        return;
    }

    string playerTag = playerState.PlayerCountryTag;

    bool canRecruit = armyManager.CanRecruitArmy(
        province.prov_id,
        playerTag,
        out string reason
    );

    recruitButton.text =
        "Recruit " + armyManager.recruitAmount +
        " | $" + armyManager.moneyCost +
        " | MP " + armyManager.manpowerCost;

    recruitButton.SetEnabled(canRecruit);

    if (canRecruit)
    {
        recruitStatusLabel.text = "Recruit: Ready";
    }
    else
    {
        recruitStatusLabel.text = "Recruit: " + reason;
    }
}
    private void Hide()
    {
        if (panel != null)
        {
            panel.style.display = DisplayStyle.None;
        }
    }
}