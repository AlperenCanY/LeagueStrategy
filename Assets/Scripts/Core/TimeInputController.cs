using UnityEngine;

public class TimeInputController : MonoBehaviour
{
    public TimeManager timeManager;

    private void Update()
    {
        if (timeManager == null)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeManager.TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            timeManager.SetSpeed(1.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            timeManager.SetSpeed(0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            timeManager.SetSpeed(0.2f);
        }
    }
}