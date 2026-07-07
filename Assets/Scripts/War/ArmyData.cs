[System.Serializable]
public class ArmyData
{
    public int armyId;
    public string ownerCountryTag;
    public int currentProvinceId;
    public int troopCount;

    public ArmyData(int armyId, string ownerCountryTag, int currentProvinceId, int troopCount)
    {
        this.armyId = armyId;
        this.ownerCountryTag = ownerCountryTag;
        this.currentProvinceId = currentProvinceId;
        this.troopCount = troopCount;
    }
}