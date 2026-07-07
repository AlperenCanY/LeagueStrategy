using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ProvinceInfoUI : MonoBehaviour
{
    private Label provinceNameLabel;
    private Label ownerLabel;
    private Label tagLabel;
    private Label idLabel;
    private Label moneyLabel;
    private Label manpowerLabel;
    public event Action OnRecruitButtonClicked;

private Button recruitButton;
    private Label provinceCountLabel;
    public SelectionManager selectionManager;
    private Label dailyIncomeLabel;
    private Label troopsLabel;
private Label dailyManpowerLabel;

    private VisualElement panel;

    private void OnEnable()
    {
        UIDocument document = GetComponent<UIDocument>();

        if (document == null)
        {
            Debug.LogError("ProvinceInfoUI için UIDocument component eksik.");
            return;
        }

        VisualElement root = document.rootVisualElement;
        root.Clear();

        panel = new VisualElement();
        panel.style.position = Position.Absolute;
        panel.style.right = 20;
        panel.style.top = 20;
        panel.style.width = 320;
        panel.style.paddingTop = 12;
        panel.style.paddingBottom = 12;
        panel.style.paddingLeft = 12;
        panel.style.paddingRight = 12;
        panel.style.backgroundColor = new Color(0f, 0f, 0f, 0.70f);

        Label title = new Label("Province Info");
        title.style.fontSize = 20;
        title.style.color = Color.white;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.marginBottom = 10;
        recruitButton = new Button();
recruitButton.text = "Recruit 1000";
recruitButton.style.marginTop = 10;
recruitButton.style.height = 32;
recruitButton.style.fontSize = 15;
recruitButton.clicked += HandleRecruitClicked;


        provinceNameLabel = CreateLabel("Province: -");
        ownerLabel = CreateLabel("Owner: -");
        tagLabel = CreateLabel("Tag: -");
        idLabel = CreateLabel("ID: -");
        moneyLabel = CreateLabel("Money: -");
        manpowerLabel = CreateLabel("Manpower: -");
        provinceCountLabel = CreateLabel("Owned Provinces: -");
        dailyIncomeLabel = CreateLabel("Daily Income: -");
dailyManpowerLabel = CreateLabel("Daily Manpower: -");
troopsLabel = CreateLabel("Troops: -");

        panel.Add(title);
        panel.Add(provinceNameLabel);
        panel.Add(ownerLabel);
        panel.Add(tagLabel);
        panel.Add(idLabel);
        panel.Add(moneyLabel);
        panel.Add(manpowerLabel);
        panel.Add(provinceCountLabel);
panel.Add(dailyIncomeLabel);
panel.Add(dailyManpowerLabel);
panel.Add(troopsLabel);
panel.Add(recruitButton);
        root.Add(panel);


        if (selectionManager != null)
{
    selectionManager.OnProvinceSelected += ShowSelection;
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
private void OnDisable()
{
    if (selectionManager != null)
    {
        selectionManager.OnProvinceSelected -= ShowSelection;
    }
}

    private Label CreateLabel(string text)
    {
        Label label = new Label(text);
        label.style.fontSize = 16;
        label.style.color = Color.white;
        label.style.marginBottom = 5;
        return label;
    }

    public void ShowProvince(ProvinceData province, CountryData ownerCountry)
    {
        if (province == null)
            return;

        provinceNameLabel.text = "Province: " + province.shapeName;
        idLabel.text = "ID: " + province.prov_id;
        tagLabel.text = "Tag: " + province.ownerCountry;
        troopsLabel.text = "Troops: " + province.stationedTroops;

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

        ownerLabel.text = "Owner: " + ownerCountry.countryName;
        moneyLabel.text = "Money: " + ownerCountry.money;
        manpowerLabel.text = "Manpower: " + ownerCountry.manpower;
        provinceCountLabel.text = "Owned Provinces: " + ownerCountry.ProvinceCount;
        dailyIncomeLabel.text = "Daily Income: " + ownerCountry.dailyIncome;
dailyManpowerLabel.text = "Daily Manpower: " + ownerCountry.dailyManpowerGain;;
panel.style.display = DisplayStyle.Flex;


    }

    private void Hide()
{
    if (panel != null)
    {
        panel.style.display = DisplayStyle.None;
    }
}


}