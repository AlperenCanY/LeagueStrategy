[System.Serializable]
public class ArmyData
{
    public int armyId;
    public string ownerCountryTag;
    public float movementProgress;

    public int currentProvinceId;
    public int troopCount;

    public bool isMoving;
    public int sourceProvinceId;
    public int targetProvinceId;
    public int movementDaysTotal;
    public int movementDaysRemaining;

    public ArmyData(int armyId, string ownerCountryTag, int currentProvinceId, int troopCount)
    {
        this.armyId = armyId;
        this.ownerCountryTag = ownerCountryTag;
        this.currentProvinceId = currentProvinceId;
        this.troopCount = troopCount;

        isMoving = false;
        sourceProvinceId = currentProvinceId;
        targetProvinceId = currentProvinceId;
        movementDaysTotal = 0;
        movementDaysRemaining = 0;
        movementProgress = 1f;
    }

public float MovementProgress
{
    get
    {
        if (!isMoving)
            return 1f;

        return movementProgress;
    }
}
}