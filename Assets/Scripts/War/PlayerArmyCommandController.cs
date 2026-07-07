using UnityEngine;

public class PlayerArmyCommandController : MonoBehaviour
{
    public SelectionManager selectionManager;
    public PlayerState playerState;
    public ArmyManager armyManager;
    public ProvinceMapPicker provinceMapPicker;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryMoveSelectedArmy();
        }
    }

    private void TryMoveSelectedArmy()
    {
        if (selectionManager == null || playerState == null || armyManager == null || provinceMapPicker == null)
        {
            Debug.LogError("PlayerArmyCommandController bağlantıları eksik.");
            return;
        }

        ArmyData selectedArmy = selectionManager.CurrentArmySelection;

        if (selectedArmy == null)
        {
            Debug.Log("Asker göndermek için önce bir army seç.");
            return;
        }

        if (selectedArmy.ownerCountryTag != playerState.playerCountryTag)
        {
            Debug.Log("Bu army sana ait değil.");
            return;
        }

        if (!provinceMapPicker.TryPickProvince(Input.mousePosition, out int targetProvinceId))
        {
            Debug.Log("Geçerli bir hedef province'e tıklanmadı.");
            return;
        }

        bool success = armyManager.MoveArmy(selectedArmy.armyId, targetProvinceId);

        if (success)
        {
            selectionManager.RefreshCurrentArmySelection();
        }
    }
}