using System.Collections.Generic;
using UnityEngine;

public class ArmyMapVisualManager : MonoBehaviour
{
    public ArmyManager armyManager;
    public SelectionManager selectionManager;
    public ProvinceMapPicker provinceMapPicker;

    private Dictionary<int, ArmyMarkerView> markersByArmyId = new Dictionary<int, ArmyMarkerView>();

    private Sprite markerSprite;

    private void Awake()
    {
        markerSprite = CreateMarkerSprite();
    }

    private void OnEnable()
    {
        if (armyManager != null)
        {
            armyManager.OnArmyCreated += CreateMarkerForArmy;
            armyManager.OnArmyChanged += UpdateMarkerPosition;
        }

        if (selectionManager != null)
        {
            selectionManager.OnArmySelected += HandleArmySelected;
        }
    }

    private void OnDisable()
    {
        if (armyManager != null)
        {
            armyManager.OnArmyCreated -= CreateMarkerForArmy;
            armyManager.OnArmyChanged -= UpdateMarkerPosition;
        }

        if (selectionManager != null)
        {
            selectionManager.OnArmySelected -= HandleArmySelected;
        }
    }

    private void CreateMarkerForArmy(ArmyData army)
    {
        GameObject markerObject = new GameObject("Army_" + army.armyId);

        markerObject.transform.position = GetArmyWorldPosition(army);
        markerObject.transform.localScale = Vector3.one * 0.35f;

        SpriteRenderer renderer = markerObject.AddComponent<SpriteRenderer>();
        renderer.sprite = markerSprite;
        renderer.sortingOrder = 30;

        BoxCollider2D collider = markerObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1.4f, 1.4f);

        ArmyMarkerView markerView = markerObject.AddComponent<ArmyMarkerView>();
        markerView.Initialize(army, selectionManager);

        markersByArmyId[army.armyId] = markerView;
    }

    private void UpdateMarkerPosition(ArmyData army)
    {
        if (!markersByArmyId.TryGetValue(army.armyId, out ArmyMarkerView marker))
            return;

        marker.army = army;
        marker.transform.position = GetArmyWorldPosition(army);
    }

    private Vector3 GetArmyWorldPosition(ArmyData army)
    {
        if (!army.isMoving)
        {
            return provinceMapPicker.GetProvinceCenterWorldPosition(army.currentProvinceId);
        }

        Vector3 sourcePos = provinceMapPicker.GetProvinceCenterWorldPosition(army.sourceProvinceId);
        Vector3 targetPos = provinceMapPicker.GetProvinceCenterWorldPosition(army.targetProvinceId);

        return Vector3.Lerp(sourcePos, targetPos, army.MovementProgress);
    }

    private void HandleArmySelected(ArmyData selectedArmy)
    {
        foreach (ArmyMarkerView marker in markersByArmyId.Values)
        {
            if (selectedArmy == null)
            {
                marker.SetSelected(false);
                continue;
            }

            bool isSelected = marker.army.armyId == selectedArmy.armyId;
            marker.SetSelected(isSelected);
        }
    }

    private Sprite CreateMarkerSprite()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color32[] pixels = new Color32[size * size];

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size * 0.42f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                int index = y * size + x;

                if (distance <= radius)
                    pixels[index] = new Color32(255, 255, 255, 255);
                else
                    pixels[index] = new Color32(0, 0, 0, 0);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            32,
            0,
            SpriteMeshType.FullRect
        );
    }
private void Update()
{
    foreach (ArmyMarkerView marker in markersByArmyId.Values)
    {
        if (marker == null || marker.army == null)
            continue;

        if (!marker.army.isMoving)
            continue;

        marker.transform.position = GetArmyWorldPosition(marker.army);
    }
}
}