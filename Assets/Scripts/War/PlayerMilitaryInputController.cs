using UnityEngine;

public class PlayerMilitaryInputController : MonoBehaviour
{
    public SelectionManager selectionManager;
    public PlayerState playerState;
    public ArmyManager armyManager;
    public ProvinceInfoUI provinceInfoUI;

    private void OnEnable()
    {
        if (provinceInfoUI != null)
            provinceInfoUI.OnRecruitButtonClicked += TryRecruitArmy;
    }

    private void OnDisable()
    {
        if (provinceInfoUI != null)
            provinceInfoUI.OnRecruitButtonClicked -= TryRecruitArmy;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            TryRecruitArmy();
    }

    private void TryRecruitArmy()
    {
        if (selectionManager == null || playerState == null || armyManager == null)
        {
            Debug.LogError("PlayerMilitaryInputController bağlantıları eksik.");
            return;
        }

        ProvinceSelection selection = selectionManager.CurrentProvinceSelection;

        if (selection == null || selection.province == null)
        {
            Debug.Log("Asker basmak için önce kendi province'ini seç.");
            return;
        }

        ArmyData army = armyManager.RecruitArmy(
            selection.province.prov_id,
            playerState.playerCountryTag
        );

        if (army != null)
        {
            selectionManager.RefreshCurrentProvinceSelection();
        }
    }
}