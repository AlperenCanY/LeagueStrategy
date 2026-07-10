using UnityEngine;
using UnityEngine.UIElements;

public class TopBarUI : MonoBehaviour
{
    [Header("References")]
    public TimeManager timeManager;
    public CountryManager countryManager;
    public PlayerState playerState;

    [Header("Debug")]
    public bool showCountrySwitchButtons = true;

    private VisualElement topBar;

    private Label dateLabel;
    private Label countryLabel;
    private Label moneyLabel;
    private Label manpowerLabel;
    private Label speedLabel;
    private Label statusLabel;

    private Button turButton;
    private Button grcButton;
    private Button bgrButton;

    private void OnEnable()
    {
        BuildUI();
        SubscribeEvents();
        Refresh();
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
            Debug.LogError("TopBarUI: UIDocument component eksik.");
            return;
        }

        VisualElement root = document.rootVisualElement;
        root.Clear();

        topBar = CreateTopBar();

        dateLabel = CreateLabel("Date: -");
        countryLabel = CreateLabel("Country: -");
        moneyLabel = CreateLabel("Money: -");
        manpowerLabel = CreateLabel("Manpower: -");
        speedLabel = CreateLabel("Speed: -");
        statusLabel = CreateLabel("Status: -");

        topBar.Add(dateLabel);
        topBar.Add(countryLabel);
        topBar.Add(moneyLabel);
        topBar.Add(manpowerLabel);
        topBar.Add(speedLabel);
        topBar.Add(statusLabel);

        if (showCountrySwitchButtons)
        {
            topBar.Add(CreateFlexibleSpace());

            turButton = CreateCountryButton("TUR");
            grcButton = CreateCountryButton("GRC");
            bgrButton = CreateCountryButton("BGR");

            topBar.Add(turButton);
            topBar.Add(grcButton);
            topBar.Add(bgrButton);
        }

        root.Add(topBar);
    }

    private VisualElement CreateTopBar()
    {
        VisualElement element = new VisualElement();

        element.style.position = Position.Absolute;
        element.style.left = 0;
        element.style.top = 0;
        element.style.right = 0;
        element.style.height = 42;

        element.style.backgroundColor = new Color(0f, 0f, 0f, 0.72f);

        element.style.flexDirection = FlexDirection.Row;
        element.style.alignItems = Align.Center;

        element.style.paddingLeft = 14;
        element.style.paddingRight = 14;

        return element;
    }

    private Label CreateLabel(string text)
    {
        Label label = new Label(text);

        label.style.color = Color.white;
        label.style.fontSize = 15;
        label.style.marginRight = 24;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.whiteSpace = WhiteSpace.NoWrap;

        return label;
    }

    private Button CreateCountryButton(string countryTag)
    {
        Button button = new Button(() => ChangePlayerCountry(countryTag));

        button.text = countryTag;
        button.style.width = 52;
        button.style.height = 28;
        button.style.marginLeft = 6;
        button.style.fontSize = 13;

        return button;
    }

    private VisualElement CreateFlexibleSpace()
    {
        VisualElement space = new VisualElement();
        space.style.flexGrow = 1;
        return space;
    }

    private void SubscribeEvents()
    {
        if (timeManager != null)
        {
            timeManager.OnDayPassed += HandleDayPassed;
            timeManager.OnTimeStateChanged += Refresh;
        }

        if (countryManager != null)
        {
            countryManager.OnCountriesChanged += Refresh;
            countryManager.OnProvinceOwnershipChanged += Refresh;
        }

        if (playerState != null)
        {
            playerState.OnPlayerCountryChanged += HandlePlayerCountryChanged;
        }
    }

    private void UnsubscribeEvents()
    {
        if (timeManager != null)
        {
            timeManager.OnDayPassed -= HandleDayPassed;
            timeManager.OnTimeStateChanged -= Refresh;
        }

        if (countryManager != null)
        {
            countryManager.OnCountriesChanged -= Refresh;
            countryManager.OnProvinceOwnershipChanged -= Refresh;
        }

        if (playerState != null)
        {
            playerState.OnPlayerCountryChanged -= HandlePlayerCountryChanged;
        }
    }

    private void HandleDayPassed(int day, int month, int year)
    {
        Refresh();
    }

    private void HandlePlayerCountryChanged(string tag)
    {
        Refresh();
    }

    private void ChangePlayerCountry(string countryTag)
    {
        if (playerState == null)
        {
            Debug.LogWarning("TopBarUI: PlayerState atanmadı.");
            return;
        }

        playerState.SetPlayerCountry(countryTag);
        Refresh();
    }

    public void Refresh()
    {
        RefreshTimeLabels();
        RefreshCountryLabels();
        RefreshCountryButtons();
    }

    private void RefreshTimeLabels()
    {
        if (timeManager == null)
        {
            dateLabel.text = "Date: -";
            speedLabel.text = "Speed: -";
            statusLabel.text = "Status: -";
            return;
        }

        dateLabel.text = "Date: " + timeManager.GetDateText();
        speedLabel.text = "Speed: " + timeManager.GetSpeedText();
        statusLabel.text = timeManager.isPaused ? "Status: Paused" : "Status: Playing";
    }

    private void RefreshCountryLabels()
    {
        CountryData playerCountry = null;

        if (playerState != null)
            playerCountry = playerState.PlayerCountry;

        if (playerCountry == null)
        {
            countryLabel.text = "Country: -";
            moneyLabel.text = "Money: -";
            manpowerLabel.text = "Manpower: -";
            return;
        }

        countryLabel.text = "Country: " + playerCountry.countryName + " (" + playerCountry.tag + ")";
        moneyLabel.text = "Money: " + FormatNumber(playerCountry.money);
        manpowerLabel.text = "Manpower: " + FormatNumber(playerCountry.manpower);
    }

    private void RefreshCountryButtons()
    {
        if (!showCountrySwitchButtons || playerState == null)
            return;

        string selectedTag = playerState.PlayerCountryTag;

        SetCountryButtonSelected(turButton, selectedTag == "TUR");
        SetCountryButtonSelected(grcButton, selectedTag == "GRC");
        SetCountryButtonSelected(bgrButton, selectedTag == "BGR");
    }

    private void SetCountryButtonSelected(Button button, bool selected)
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
            button.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f, 1f);
            button.style.color = Color.white;
        }
    }

    private string FormatNumber(int value)
    {
        return value.ToString("N0");
    }
}