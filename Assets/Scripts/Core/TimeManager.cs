using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Date")]
    public int day = 1;
    public int month = 1;
    public int year = 1936;

    [Header("Time State")]
    public bool isPaused = false;
    public int speed = 1;

    [Header("Speed Settings")]
    public float oneXSecondsPerDay = 0.5f;
    public float secondsPerDay = 0.5f;

    private float timer;

    public event Action<int, int, int> OnDayPassed;
    public event Action OnTimeStateChanged;

    private void Awake()
    {
        ApplySpeedToSecondsPerDay();
    }

    private void Update()
    {
        if (isPaused)
            return;

        timer += Time.deltaTime;

        if (timer >= secondsPerDay)
        {
            timer = 0f;
            AdvanceDay();
        }
    }

    private void AdvanceDay()
    {
        day++;

        if (day > 30)
        {
            day = 1;
            month++;
        }

        if (month > 12)
        {
            month = 1;
            year++;
        }

        OnDayPassed?.Invoke(day, month, year);
        OnTimeStateChanged?.Invoke();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        OnTimeStateChanged?.Invoke();
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
        OnTimeStateChanged?.Invoke();
    }

    public void SetSpeed(int newSpeed)
    {
        speed = Mathf.Clamp(newSpeed, 1, 3);
        ApplySpeedToSecondsPerDay();

        OnTimeStateChanged?.Invoke();
    }

    public void SetSpeed(float newSecondsPerDay)
    {
        secondsPerDay = Mathf.Max(0.05f, newSecondsPerDay);

        if (Mathf.Approximately(secondsPerDay, oneXSecondsPerDay))
            speed = 1;
        else if (secondsPerDay < oneXSecondsPerDay && secondsPerDay >= oneXSecondsPerDay / 2f)
            speed = 2;
        else if (secondsPerDay < oneXSecondsPerDay / 2f)
            speed = 3;

        OnTimeStateChanged?.Invoke();
    }

    private void ApplySpeedToSecondsPerDay()
    {
        secondsPerDay = oneXSecondsPerDay / Mathf.Max(1, speed);
    }

    public string GetDateText()
    {
        return day + "." + month + "." + year;
    }

    public string GetSpeedText()
    {
        if (isPaused)
            return "Paused";

        return speed + "x";
    }
}