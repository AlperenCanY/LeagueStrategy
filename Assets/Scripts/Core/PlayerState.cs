using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public CountryManager countryManager;

    [Header("Player")]
    public string playerCountryTag = "TUR";

    public CountryData PlayerCountry
    {
        get
        {
            if (countryManager == null)
                return null;

            return countryManager.GetCountry(playerCountryTag);
        }
    }

    public bool IsPlayerProvince(ProvinceData province)
    {
        if (province == null)
            return false;

        return province.ownerCountry == playerCountryTag;
    }

    public void SetPlayerCountry(string tag)
    {
        playerCountryTag = tag;
        Debug.Log("Oyuncu ülkesi seçildi: " + tag);
    }
}