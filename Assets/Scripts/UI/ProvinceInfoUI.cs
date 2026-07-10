using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ProvinceInfoUI : MonoBehaviour
{
    [Header("References")]
    public SelectionManager selectionManager;
    public PlayerState playerState;
    public ArmyManager armyManager;

    public event Action OnRecruitButtonClicked;

    private VisualElement panel;

    private Label titleLabel;
    private Label idTagLabel;

    private Label ownerLabel;
    private Label populationLabel;
    private Label recruitableLabel;
    private Label economyLabel;
    private Label infrastructureLabel;
    private Label supplyLabel;
    private Label terrainLabel;
    private Label troopsLabel;

    private Label countryMoneyLabel;
    private Label countryManpowerLabel;
    private Label ownedProvincesLabel;
    private Label dailyStatsLabel;

    private Label recruitStatusLabel;
    private Label recruitCostLabel;
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

        titleLabel = CreateTitle("Province");
        idTagLabel = CreateSmallLabel("ID: - | Tag: -");

        ownerLabel = CreateLabel("Owner: -");
        populationLabel = CreateLabel("Population: -");
        recruitableLabel = CreateLabel("Recruitable: -");
        economyLabel = CreateLabel("Economy: -");
        infrastructureLabel = CreateLabel("Infrastructure: -");
        supplyLabel = CreateLabel("Supply: -");
        terrainLabel = CreateLabel("Terrain: -");
        troopsLabel = CreateLabel("Troops: -");

        countryMoneyLabel = CreateLabel("Money: -");
        countryManpowerLabel = CreateLabel("Manpower: -");
        ownedProvincesLabel = CreateLabel("Owned Provinces: -");
        dailyStatsLabel = CreateLabel("Daily: -");

        recruitStatusLabel = CreateLabel("Status: -");
        recruitCostLabel = CreateLabel("Cost: -");
        recruitButton = CreateRecruitButton();

        VisualElement header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;
        header.style.alignItems = Align.Center;

        header.Add(titleLabel);
        header.Add(idTagLabel);

        VisualElement contentRow = new VisualElement();
        contentRow.style.flexDirection = FlexDirection.Row;
        contentRow.style.marginTop = 8;

        VisualElement provinceColumn = CreateColumn();
        VisualElement countryColumn = CreateColumn();
        VisualElement recruitColumn = CreateColumn();

        provinceColumn.Add(CreateSectionTitle("Province"));
        provinceColumn.Add(ownerLabel);
        provinceColumn.Add(populationLabel);
        provinceColumn.Add(recruitableLabel);
        provinceColumn.Add(economyLabel);
        provinceColumn.Add(infrastructureLabel);
        provinceColumn.Add(supplyLabel);
        provinceColumn.Add(terrainLabel);
        provinceColumn.Add(troopsLabel);

        countryColumn.Add(CreateSectionTitle("Owner Country"));
        countryColumn.Add(countryMoneyLabel);
        countryColumn.Add(countryManpowerLabel);
        countryColumn.Add(ownedProvincesLabel);
        countryColumn.Add(dailyStatsLabel);

        recruitColumn.Add(CreateSectionTitle("Recruitment"));
        recruitColumn.Add(recruitStatusLabel);
        recruitColumn.Add(recruitCostLabel);
        recruitColumn.Add(recruitButton);

        contentRow.Add(provinceColumn);
        contentRow.Add(countryColumn);
        contentRow.Add(recruitColumn);

        panel.Add(header);
        panel.Add(CreateDivider());
        panel.Add(contentRow);

        root.Add(panel);
    }

    private VisualElement CreatePanel()
    {
        VisualElement element = new VisualElement();

        element.style.position = Position.Absolute;
element.style.right = 12;
element.style.bottom = 12;

element.style.width = 560;
element.style.height = 185;

element.style.paddingTop = 8;
element.style.paddingBottom = 8;
element.style.paddingLeft = 10;
element.style.paddingRight = 10;

        element.style.backgroundColor = new Color(0f, 0f, 0f, 0.78f);

        return element;
    }

private VisualElement CreateColumn()
{
    VisualElement column = new VisualElement();
    column.style.flexGrow = 1;
    column.style.width = Length.Percent(33);
    column.style.paddingRight = 8;
    return column;
}
private Label CreateTitle(string text)
{
    Label label = new Label(text);
    label.style.fontSize = 15;
    label.style.color = Color.white;
    label.style.unityFontStyleAndWeight = FontStyle.Bold;
    label.style.whiteSpace = WhiteSpace.NoWrap;
    return label;
}

private Label CreateSectionTitle(string text)
{
    Label label = new Label(text);
    label.style.fontSize = 11;
    label.style.color = new Color(1f, 0.78f, 0.28f);
    label.style.unityFontStyleAndWeight = FontStyle.Bold;
    label.style.marginBottom = 3;
    label.style.whiteSpace = WhiteSpace.NoWrap;
    return label;
}

private Label CreateLabel(string text)
{
    Label label = new Label(text);
    label.style.fontSize = 10;
    label.style.color = Color.white;
    label.style.marginBottom = 2;
    label.style.whiteSpace = WhiteSpace.NoWrap;
    return label;
}

private Label CreateSmallLabel(string text)
{
    Label label = new Label(text);
    label.style.fontSize = 9;
    label.style.color = new Color(1f, 1f, 1f, 0.85f);
    label.style.whiteSpace = WhiteSpace.NoWrap;
    return label;
}

    private VisualElement CreateDivider()
    {
        VisualElement divider = new VisualElement();

        divider.style.height = 1;
        divider.style.marginTop = 8;
        divider.style.marginBottom = 4;
        divider.style.backgroundColor = new Color(1f, 1f, 1f, 0.22f);

        return divider;
    }

private Button CreateRecruitButton()
{
    Button button = new Button();
    button.text = "Recruit";
    button.style.height = 24;
    button.style.marginTop = 6;
    button.style.fontSize = 10;
    button.clicked += HandleRecruitClicked;
    return button;
}

    private void SubscribeEvents()
    {
        if (selectionManager != null)
            selectionManager.OnProvinceSelected += ShowSelection;
    }

    private void UnsubscribeEvents()
    {
        if (selectionManager != null)
            selectionManager.OnProvinceSelected -= ShowSelection;

        if (recruitButton != null)
            recruitButton.clicked -= HandleRecruitClicked;
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

    private void HandleRecruitClicked()
    {
        OnRecruitButtonClicked?.Invoke();
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
        UpdateRecruitButton(province);

        Show();
    }

    private void UpdateProvinceLabels(ProvinceData province)
    {
        titleLabel.text = province.shapeName;
        idTagLabel.text = "ID: " + province.prov_id + " | Tag: " + province.ownerCountry;

        ownerLabel.text = "Owner: " + province.ownerCountry;
        populationLabel.text = "Population: " + FormatNumber(province.population);
        recruitableLabel.text = "Recruitable: " + FormatNumber(province.recruitablePopulation);
        economyLabel.text = "Economy: " + province.economyValue;
        infrastructureLabel.text = "Infrastructure: " + province.infrastructure;
        supplyLabel.text = "Supply: " + FormatNumber(province.supplyLimit);
        terrainLabel.text = "Terrain: " + province.terrainType;
        troopsLabel.text = "Troops: " + FormatNumber(province.stationedTroops);
    }

    private void UpdateCountryLabels(CountryData ownerCountry)
    {
        if (ownerCountry == null)
        {
            countryMoneyLabel.text = "Money: -";
            countryManpowerLabel.text = "Manpower: -";
            ownedProvincesLabel.text = "Owned Provinces: -";
            dailyStatsLabel.text = "Daily: -";
            return;
        }

        countryMoneyLabel.text = "Money: " + FormatNumber(ownerCountry.money);
        countryManpowerLabel.text = "Manpower: " + FormatNumber(ownerCountry.manpower);
        ownedProvincesLabel.text = "Owned Provinces: " + ownerCountry.ProvinceCount;

        dailyStatsLabel.text =
            "Daily: +" + ownerCountry.dailyIncome +
            " money | +" + ownerCountry.dailyManpowerGain + " MP";
    }

    private void UpdateRecruitButton(ProvinceData province)
    {
        if (recruitButton == null || recruitStatusLabel == null || recruitCostLabel == null)
            return;

        if (province == null)
        {
            recruitButton.SetEnabled(false);
            recruitStatusLabel.text = "Status: -";
            recruitCostLabel.text = "Cost: -";
            return;
        }

        if (armyManager == null || playerState == null)
        {
            recruitButton.SetEnabled(false);
            recruitStatusLabel.text = "Status: System not linked";
            recruitCostLabel.text = "Cost: -";
            return;
        }

        bool canRecruit = armyManager.CanRecruitArmy(
            province.prov_id,
            playerState.PlayerCountryTag,
            out string reason
        );

        recruitStatusLabel.text = canRecruit ? "Status: Ready" : "Status: " + reason;

        recruitCostLabel.text =
            "Cost: $" + armyManager.moneyCost +
            " | MP " + armyManager.manpowerCost +
            " | +" + armyManager.recruitAmount + " troops";

        recruitButton.text = "Recruit " + armyManager.recruitAmount;
        recruitButton.SetEnabled(canRecruit);
    }

    private void Show()
    {
        if (panel != null)
            panel.style.display = DisplayStyle.Flex;
    }

    private void Hide()
    {
        if (panel != null)
            panel.style.display = DisplayStyle.None;
    }

    private string FormatNumber(int value)
    {
        return value.ToString("N0");
    }
}