using UnityEngine;
using UnityEngine.UIElements;

public class TimeControlUI : MonoBehaviour
{
    [Header("References")]
    public TimeManager timeManager;

    private VisualElement panel;

    private Label dateLabel;
    private Label speedLabel;

    private Button pauseButton;
    private Button speed1Button;
    private Button speed2Button;
    private Button speed3Button;

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
            Debug.LogError("TimeControlUI: UIDocument eksik.");
            return;
        }

        VisualElement root = document.rootVisualElement;
        root.Clear();

        panel = CreatePanel();

        dateLabel = CreateLabel("Date: -");
        speedLabel = CreateLabel("Speed: -");

        pauseButton = CreateButton("Pause");
        speed1Button = CreateButton("1x");
        speed2Button = CreateButton("2x");
        speed3Button = CreateButton("3x");

        pauseButton.clicked += HandlePauseClicked;
        speed1Button.clicked += () => SetSpeed(1);
        speed2Button.clicked += () => SetSpeed(2);
        speed3Button.clicked += () => SetSpeed(3);

        panel.Add(dateLabel);
        panel.Add(CreateSpacer(12));
        panel.Add(speedLabel);
        panel.Add(CreateSpacer(12));
        panel.Add(pauseButton);
        panel.Add(speed1Button);
        panel.Add(speed2Button);
        panel.Add(speed3Button);

        root.Add(panel);
    }

    private VisualElement CreatePanel()
    {
        VisualElement element = new VisualElement();

        element.style.position = Position.Absolute;
element.style.left = 78;
element.style.top = 54;
element.style.width = 360;

        element.style.flexDirection = FlexDirection.Row;
        element.style.alignItems = Align.Center;

        element.style.paddingTop = 8;
        element.style.paddingBottom = 8;
        element.style.paddingLeft = 10;
        element.style.paddingRight = 10;

        element.style.backgroundColor = new Color(0f, 0f, 0f, 0.75f);

        return element;
    }

    private Label CreateLabel(string text)
    {
        Label label = new Label(text);

        label.style.fontSize = 15;
        label.style.color = Color.white;
        label.style.marginRight = 6;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.whiteSpace = WhiteSpace.NoWrap;

        return label;
    }

    private Button CreateButton(string text)
    {
        Button button = new Button();

        button.text = text;
        button.style.width = 58;
        button.style.height = 30;
        button.style.marginLeft = 4;
        button.style.fontSize = 14;

        return button;
    }

    private VisualElement CreateSpacer(int width)
    {
        VisualElement spacer = new VisualElement();
        spacer.style.width = width;
        return spacer;
    }

    private void SubscribeEvents()
    {
        if (timeManager == null)
            return;

        timeManager.OnDayPassed += HandleDayPassed;
        timeManager.OnTimeStateChanged += Refresh;
    }

    private void UnsubscribeEvents()
    {
        if (timeManager == null)
            return;

        timeManager.OnDayPassed -= HandleDayPassed;
        timeManager.OnTimeStateChanged -= Refresh;
    }

    private void HandleDayPassed(int day, int month, int year)
    {
        Refresh();
    }

    private void HandlePauseClicked()
    {
        if (timeManager == null)
            return;

        timeManager.TogglePause();
        Refresh();
    }

    private void SetSpeed(int speed)
    {
        if (timeManager == null)
            return;

        timeManager.SetSpeed(speed);
        timeManager.SetPaused(false);

        Refresh();
    }

    private void Refresh()
    {
        if (dateLabel == null || speedLabel == null || pauseButton == null)
            return;

        if (timeManager == null)
        {
            dateLabel.text = "Date: -";
            speedLabel.text = "Speed: -";
            pauseButton.text = "Pause";
            return;
        }

        dateLabel.text = "Date: " + timeManager.GetDateText();
        speedLabel.text = "Speed: " + timeManager.GetSpeedText();

        pauseButton.text = timeManager.isPaused ? "Play" : "Pause";

        UpdateSpeedButtonStates();
    }

    private void UpdateSpeedButtonStates()
    {
        if (timeManager == null)
            return;

        SetButtonSelected(speed1Button, !timeManager.isPaused && timeManager.speed == 1);
        SetButtonSelected(speed2Button, !timeManager.isPaused && timeManager.speed == 2);
        SetButtonSelected(speed3Button, !timeManager.isPaused && timeManager.speed == 3);
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
            button.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f, 1f);
            button.style.color = Color.white;
        }
    }
}