using UnityEngine;
using UnityEngine.UIElements;

public class ArmyInfoUI : MonoBehaviour
{
    public SelectionManager selectionManager;
    public ProvinceManager provinceManager;
    public CountryManager countryManager;

    private VisualElement panel;

    private Label armyIdLabel;
    private Label ownerLabel;
    private Label troopsLabel;
    private Label locationLabel;
    private Label statusLabel;

    private void OnEnable()
    {
        UIDocument document = GetComponent<UIDocument>();

        if (document == null)
        {
            Debug.LogError("ArmyInfoUI için UIDocument component eksik.");
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
        panel.style.backgroundColor = new Color(0.05f, 0.05f, 0.05f, 0.78f);
        panel.style.display = DisplayStyle.None;

        Label title = new Label("Army Info");
        title.style.fontSize = 20;
        title.style.color = Color.white;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.marginBottom = 10;

        armyIdLabel = CreateLabel("Army ID: -");
        ownerLabel = CreateLabel("Owner: -");
        troopsLabel = CreateLabel("Troops: -");
        locationLabel = CreateLabel("Location: -");
        statusLabel = CreateLabel("Status: Idle");

        panel.Add(title);
        panel.Add(armyIdLabel);
        panel.Add(ownerLabel);
        panel.Add(troopsLabel);
        panel.Add(locationLabel);
        panel.Add(statusLabel);

        root.Add(panel);

        if (selectionManager != null)
        {
            selectionManager.OnArmySelected += ShowArmy;
            selectionManager.OnProvinceSelected += HandleProvinceSelected;
        }
    }

    private void OnDisable()
    {
        if (selectionManager != null)
        {
            selectionManager.OnArmySelected -= ShowArmy;
            selectionManager.OnProvinceSelected -= HandleProvinceSelected;
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

    private void HandleProvinceSelected(ProvinceSelection selection)
    {
        Hide();
    }

    private void ShowArmy(ArmyData army)
    {
        if (army == null)
        {
            Hide();
            return;
        }

        ProvinceData location = null;

        if (provinceManager != null)
        {
            location = provinceManager.GetProvinceById(army.currentProvinceId);
        }

        CountryData owner = null;

        if (countryManager != null)
        {
            owner = countryManager.GetCountry(army.ownerCountryTag);
        }

        armyIdLabel.text = "Army ID: " + army.armyId;
        ownerLabel.text = "Owner: " + (owner != null ? owner.countryName : army.ownerCountryTag);
        troopsLabel.text = "Troops: " + army.troopCount;
        locationLabel.text = "Location: " + (location != null ? location.shapeName : "Unknown");
        if (army.isMoving)
{
    ProvinceData target = provinceManager.GetProvinceById(army.targetProvinceId);

    statusLabel.text =
        "Status: Moving to " +
        (target != null ? target.shapeName : "Unknown") +
        " (" + army.movementDaysRemaining + " days)";
}
else
{
    statusLabel.text = "Status: Idle";
}

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