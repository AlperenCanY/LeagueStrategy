using UnityEngine;

public class ArmyMarkerView : MonoBehaviour
{
    public ArmyData army;
    public SelectionManager selectionManager;

    private SpriteRenderer spriteRenderer;

    public void Initialize(ArmyData armyData, SelectionManager selectionManagerReference)
    {
        army = armyData;
        selectionManager = selectionManagerReference;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSelected(bool isSelected)
    {
        transform.localScale = isSelected ? Vector3.one * 0.5f : Vector3.one * 0.35f;

        if (spriteRenderer != null)
            spriteRenderer.color = isSelected ? Color.yellow : Color.white;
    }
}