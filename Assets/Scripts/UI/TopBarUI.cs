using UnityEngine;
using UnityEngine.UIElements;

public class TopBarUI : MonoBehaviour
{
    public TimeManager timeManager;
    public CountryManager countryManager;
    public PlayerState playerState;

    private Label dateLabel;
    private Label countryLabel;
    private Label moneyLabel;
    private Label manpowerLabel;
    private Label speedLabel;
    private Label pauseLabel;

    private void OnEnable()
    {
        UIDocument document = GetComponent<UIDocument>();

        if (document == null)
        {
            Debug.LogError("TopBarUI için UIDocument component eksik.");
            return;
        }

        VisualElement root = document.rootVisualElement;

        VisualElement topBar = new VisualElement();
        topBar.style.position = Position.Absolute;
        topBar.style.left = 0;
        topBar.style.top = 0;
        topBar.style.right = 0;
        topBar.style.height = 42;
        topBar.style.backgroundColor = new Color(0f, 0f, 0f, 0.70f);
        topBar.style.flexDirection = FlexDirection.Row;
        topBar.style.alignItems = Align.Center;
        topBar.style.paddingLeft = 14;
        topBar.style.paddingRight = 14;

        dateLabel = CreateLabel("Date: -");
        countryLabel = CreateLabel("Country: -");
        moneyLabel = CreateLabel("Money: -");
        manpowerLabel = CreateLabel("Manpower: -");
        speedLabel = CreateLabel("Speed: -");
        pauseLabel = CreateLabel("Status: -");

        topBar.Add(dateLabel);
        topBar.Add(countryLabel);
        topBar.Add(moneyLabel);
        topBar.Add(manpowerLabel);
        topBar.Add(speedLabel);
        topBar.Add(pauseLabel);
Button turButton = new Button(() =>
{
    playerState.SetPlayerCountry("TUR");
    Refresh();
});

turButton.text = "TUR";

Button grcButton = new Button(() =>
{
    playerState.SetPlayerCountry("GRC");
    Refresh();
});

grcButton.text = "GRC";

Button bgrButton = new Button(() =>
{
    playerState.SetPlayerCountry("BGR");
    Refresh();
});

bgrButton.text = "BGR";

topBar.Add(turButton);
topBar.Add(grcButton);
topBar.Add(bgrButton);
        root.Add(topBar);

        if (timeManager != null)
            timeManager.OnDayPassed += HandleDayPassed;
            if (timeManager != null)
    timeManager.OnTimeStateChanged += Refresh;

       if (countryManager != null)
    countryManager.OnCountriesChanged += Refresh;

if (playerState != null)
    playerState.OnPlayerCountryChanged += HandlePlayerCountryChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (timeManager != null)
            timeManager.OnDayPassed -= HandleDayPassed;

        if (countryManager != null)
            countryManager.OnCountriesChanged -= Refresh;
            if (playerState != null)
    playerState.OnPlayerCountryChanged -= HandlePlayerCountryChanged;
            if (timeManager != null)
    timeManager.OnTimeStateChanged -= Refresh;
    }

    private Label CreateLabel(string text)
    {
        Label label = new Label(text);
        label.style.color = Color.white;
        label.style.fontSize = 15;
        label.style.marginRight = 24;
        return label;
    }

    private void HandleDayPassed(int day, int month, int year)
    {
        Refresh();
    }

    public void Refresh()
    {
        if (timeManager != null)
        {
            dateLabel.text = "Date: " + timeManager.GetDateText();
            speedLabel.text = "Speed: " + timeManager.secondsPerDay.ToString("0.00") + "s/day";
            pauseLabel.text = timeManager.isPaused ? "Status: Paused" : "Status: Playing";
        }

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

        countryLabel.text = "Country: " + playerCountry.countryName;
        moneyLabel.text = "Money: " + playerCountry.money;
        manpowerLabel.text = "Manpower: " + playerCountry.manpower;
    }
    private void HandlePlayerCountryChanged(string tag)
{
    Refresh();
}
}