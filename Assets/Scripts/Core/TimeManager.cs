using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public int day = 1;
    public int month = 1;
    public int year = 1936;

    public bool isPaused = false;

    [Header("Speed")]
    public float secondsPerDay = 0.5f;

    private float timer;

    public event Action<int, int, int> OnDayPassed;

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
    }

    public string GetDateText()
    {
        return day.ToString("00") + "." + month.ToString("00") + "." + year;
    }

public void TogglePause()
{
    isPaused = !isPaused;
    OnTimeStateChanged?.Invoke();
}

public void SetSpeed(float newSecondsPerDay)
{
    secondsPerDay = newSecondsPerDay;
    OnTimeStateChanged?.Invoke();
}
    public event Action OnTimeStateChanged;
}