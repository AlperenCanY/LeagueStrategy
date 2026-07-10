using UnityEngine;
using UnityEngine.UIElements;

public class LeftSidebarUI : MonoBehaviour
{
    [Header("Panel Documents")]
    public UIDocument countryInfoDocument;
    public UIDocument provinceInfoDocument;
    public UIDocument armyInfoDocument;
    public UIDocument timeControlDocument;

    private UIDocument sidebarDocument;

    private Button countryButton;
    private Button provinceButton;
    private Button armyButton;
    private Button timeButton;

    private UIDocument currentlyOpenDocument;

    private void OnEnable()
    {
        sidebarDocument = GetComponent<UIDocument>();

        if (sidebarDocument == null)
        {
            Debug.LogError("LeftSidebarUI: UIDocument eksik.");
            return;
        }

        BuildUI();
        CloseAllPanels();
    }

    private void BuildUI()
    {
        VisualElement root = sidebarDocument.rootVisualElement;
        root.Clear();

        VisualElement bar = new VisualElement();

        bar.style.position = Position.Absolute;
        bar.style.left = 10;
        bar.style.top = 54;
        bar.style.width = 58;

        bar.style.paddingTop = 8;
        bar.style.paddingBottom = 8;
        bar.style.paddingLeft = 6;
        bar.style.paddingRight = 6;

        bar.style.backgroundColor = new Color(0f, 0f, 0f, 0.78f);

        countryButton = CreateMenuButton("C");
        provinceButton = CreateMenuButton("P");
        armyButton = CreateMenuButton("A");
        timeButton = CreateMenuButton("T");

        countryButton.clicked += () => TogglePanel(countryInfoDocument);
        provinceButton.clicked += () => TogglePanel(provinceInfoDocument);
        armyButton.clicked += () => TogglePanel(armyInfoDocument);
        timeButton.clicked += () => TogglePanel(timeControlDocument);

        bar.Add(countryButton);
        bar.Add(provinceButton);
        bar.Add(armyButton);
        bar.Add(timeButton);

        root.Add(bar);
    }

    private Button CreateMenuButton(string text)
    {
        Button button = new Button();

        button.text = text;
        button.style.width = 44;
        button.style.height = 40;
        button.style.marginBottom = 6;
        button.style.fontSize = 17;
        button.style.unityFontStyleAndWeight = FontStyle.Bold;

        button.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
        button.style.color = Color.white;

        return button;
    }

    private void TogglePanel(UIDocument targetDocument)
    {
        if (targetDocument == null)
            return;

        if (currentlyOpenDocument == targetDocument && IsPanelVisible(targetDocument))
        {
            HidePanel(targetDocument);
            currentlyOpenDocument = null;
            RefreshButtonStates();
            return;
        }

        CloseAllPanels();

        ShowPanel(targetDocument);
        currentlyOpenDocument = targetDocument;

        RefreshButtonStates();
    }

    private void CloseAllPanels()
    {
        HidePanel(countryInfoDocument);
        HidePanel(provinceInfoDocument);
        HidePanel(armyInfoDocument);
        HidePanel(timeControlDocument);

        currentlyOpenDocument = null;
        RefreshButtonStates();
    }

    private void ShowPanel(UIDocument document)
    {
        if (document == null)
            return;

        document.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void HidePanel(UIDocument document)
    {
        if (document == null)
            return;

        document.rootVisualElement.style.display = DisplayStyle.None;
    }

    private bool IsPanelVisible(UIDocument document)
    {
        if (document == null)
            return false;

        return document.rootVisualElement.style.display != DisplayStyle.None;
    }

    private void RefreshButtonStates()
    {
        SetButtonSelected(countryButton, currentlyOpenDocument == countryInfoDocument);
        SetButtonSelected(provinceButton, currentlyOpenDocument == provinceInfoDocument);
        SetButtonSelected(armyButton, currentlyOpenDocument == armyInfoDocument);
        SetButtonSelected(timeButton, currentlyOpenDocument == timeControlDocument);
    }

    private void SetButtonSelected(Button button, bool selected)
    {
        if (button == null)
            return;

        if (selected)
        {
            button.style.backgroundColor = new Color(1f, 0.75f, 0.25f, 1f);
            button.style.color = Color.black;
        }
        else
        {
            button.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
            button.style.color = Color.white;
        }
    }
}