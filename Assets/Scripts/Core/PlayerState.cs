    using System;
    using UnityEngine;

    public class PlayerState : MonoBehaviour
    {
        [SerializeField]
        private string playerCountryTag = "TUR";

        public string PlayerCountryTag => playerCountryTag;

        public CountryData PlayerCountry
        {
            get
            {
                CountryManager countryManager = FindAnyObjectByType<CountryManager>();

                if (countryManager == null)
                    return null;

                return countryManager.GetCountry(playerCountryTag);
            }
        }

        public event Action<string> OnPlayerCountryChanged;

        public void SetPlayerCountry(string newTag)
        {
            if (string.IsNullOrWhiteSpace(newTag))
                return;

            if (playerCountryTag == newTag)
                return;

            playerCountryTag = newTag;

            Debug.Log("Aktif oyuncu ülkesi: " + playerCountryTag);

            OnPlayerCountryChanged?.Invoke(playerCountryTag);
        }
    }