using UnityEngine;

public class CountryInfoInputController : MonoBehaviour
{
    public CountryInfoUI countryInfoUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (countryInfoUI != null)
                countryInfoUI.Toggle();
        }
    }
}