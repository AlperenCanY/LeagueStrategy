using UnityEngine;

public class WarManager : MonoBehaviour
{
    public ArmyManager armyManager;
    public ProvinceManager provinceManager;
    public CountryManager countryManager;
    public SelectionManager selectionManager;

    [Header("Combat")]
    [Range(0f, 1f)]
    public float winnerLossRatioFromLoserPower = 0.5f;

    private void OnEnable()
    {
        if (armyManager != null)
            armyManager.OnArmyArrived += HandleArmyArrived;
    }

    private void OnDisable()
    {
        if (armyManager != null)
            armyManager.OnArmyArrived -= HandleArmyArrived;
    }

    private void HandleArmyArrived(ArmyData attacker)
    {
        if (attacker == null)
            return;

        if (armyManager.GetArmy(attacker.armyId) == null)
            return;

        ProvinceData targetProvince = provinceManager.GetProvinceById(attacker.currentProvinceId);

        if (targetProvince == null)
            return;

        if (targetProvince.ownerCountry == attacker.ownerCountryTag)
        {
            Debug.Log("Army dost province'e vardı: " + targetProvince.shapeName);
            RefreshSelections();
            return;
        }

        ArmyData defender = armyManager.GetFirstEnemyArmyInProvince(
            targetProvince.prov_id,
            attacker.ownerCountryTag
        );

        if (defender == null)
        {
            CaptureProvince(attacker, targetProvince);
            RefreshSelections();
            return;
        }

        ResolveCombat(attacker, defender, targetProvince);
        RefreshSelections();
    }

    private void ResolveCombat(ArmyData attacker, ArmyData defender, ProvinceData targetProvince)
    {
        int attackerPower = attacker.troopCount;
        int defenderPower = defender.troopCount;

        Debug.Log(
            "Savaş başladı. Attacker Army " + attacker.armyId +
            " (" + attackerPower + ")" +
            " vs Defender Army " + defender.armyId +
            " (" + defenderPower + ")"
        );

        if (attackerPower > defenderPower)
        {
            ResolveAttackerVictory(attacker, defender, targetProvince);
        }
        else
        {
            ResolveDefenderVictory(attacker, defender, targetProvince);
        }
    }

    private void ResolveAttackerVictory(ArmyData attacker, ArmyData defender, ProvinceData targetProvince)
    {
        int defenderPower = defender.troopCount;
        int winnerLoss = CalculateWinnerLoss(defenderPower);

        int attackerRemainingTroops = Mathf.Max(1, attacker.troopCount - winnerLoss);

        armyManager.SetArmyTroopCount(attacker.armyId, attackerRemainingTroops);
        armyManager.DestroyArmy(defender.armyId);

        CaptureProvince(attacker, targetProvince);

        Debug.Log(
            "Saldıran kazandı. Army " + attacker.armyId +
            " kalan asker: " + attackerRemainingTroops
        );
    }

    private void ResolveDefenderVictory(ArmyData attacker, ArmyData defender, ProvinceData targetProvince)
    {
        int attackerPower = attacker.troopCount;
        int winnerLoss = CalculateWinnerLoss(attackerPower);

        int defenderRemainingTroops = Mathf.Max(1, defender.troopCount - winnerLoss);

        armyManager.SetArmyTroopCount(defender.armyId, defenderRemainingTroops);
        armyManager.DestroyArmy(attacker.armyId);

        Debug.Log(
            "Savunan kazandı. Province aynı kaldı: " + targetProvince.shapeName +
            " / Defender Army " + defender.armyId +
            " kalan asker: " + defenderRemainingTroops
        );
    }

    private int CalculateWinnerLoss(int loserPower)
    {
        int loss = Mathf.RoundToInt(loserPower * winnerLossRatioFromLoserPower);
        return Mathf.Max(1, loss);
    }

    private void CaptureProvince(ArmyData army, ProvinceData province)
    {
        if (army == null || province == null)
            return;

        if (province.ownerCountry == army.ownerCountryTag)
            return;

        Debug.Log("Province ele geçiriliyor: " + province.shapeName);

        countryManager.TransferProvince(province, army.ownerCountryTag);
    }

    private void RefreshSelections()
    {
        if (selectionManager == null)
            return;

        selectionManager.RefreshCurrentArmySelection();
        selectionManager.RefreshCurrentProvinceSelection();
    }
}