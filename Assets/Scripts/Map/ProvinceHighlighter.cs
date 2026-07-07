using System.Collections.Generic;
using UnityEngine;

public class ProvinceHighlighter : MonoBehaviour
{
    public Texture2D provinceMapTexture;
    public SpriteRenderer mapRenderer;
    public SelectionManager selectionManager;

    [Header("Highlight")]
    public Color32 highlightColor = new Color32(255, 220, 0, 140);

    private SpriteRenderer highlightRenderer;
    private Color32[] provincePixels;

    private Dictionary<int, Sprite> cachedSprites = new Dictionary<int, Sprite>();
    private Dictionary<int, Vector3> cachedLocalPositions = new Dictionary<int, Vector3>();

    private void Awake()
    {
        CreateHighlightRenderer();
        CacheProvincePixels();
    }

    private void OnEnable()
    {
        if (selectionManager != null)
        {
            selectionManager.OnProvinceSelected += HandleProvinceSelected;
        }
    }

    private void OnDisable()
    {
        if (selectionManager != null)
        {
            selectionManager.OnProvinceSelected -= HandleProvinceSelected;
        }
    }

    private void CreateHighlightRenderer()
    {
        if (mapRenderer == null)
        {
            Debug.LogError("ProvinceHighlighter: Map Renderer atanmadı.");
            return;
        }

        GameObject highlightObject = new GameObject("ProvinceHighlightOptimized");
        highlightObject.transform.SetParent(mapRenderer.transform, false);
        highlightObject.transform.localPosition = Vector3.zero;
        highlightObject.transform.localRotation = Quaternion.identity;
        highlightObject.transform.localScale = Vector3.one;

        highlightRenderer = highlightObject.AddComponent<SpriteRenderer>();
        highlightRenderer.sortingOrder = mapRenderer.sortingOrder + 2;
    }

    private void CacheProvincePixels()
    {
        if (provinceMapTexture == null)
        {
            Debug.LogError("ProvinceHighlighter: Province Map Texture atanmadı.");
            return;
        }

        provincePixels = provinceMapTexture.GetPixels32();
    }

private void HandleProvinceSelected(ProvinceSelection selection)
{
    if (selection == null || selection.province == null)
    {
        ClearHighlight();
        return;
    }

    HighlightProvince(selection.province.prov_id);
}

    public void HighlightProvince(int provinceId)
    {
        if (highlightRenderer == null || provinceMapTexture == null || mapRenderer == null)
            return;

        if (!cachedSprites.ContainsKey(provinceId))
        {
            CreateHighlightSpriteForProvince(provinceId);
        }

        if (!cachedSprites.ContainsKey(provinceId))
            return;

        highlightRenderer.sprite = cachedSprites[provinceId];
        highlightRenderer.transform.localPosition = cachedLocalPositions[provinceId];
        highlightRenderer.enabled = true;
    }

    private void CreateHighlightSpriteForProvince(int provinceId)
    {
        int width = provinceMapTexture.width;
        int height = provinceMapTexture.height;

        int minX = width;
        int minY = height;
        int maxX = -1;
        int maxY = -1;

        for (int y = 0; y < height; y++)
        {
            int rowIndex = y * width;

            for (int x = 0; x < width; x++)
            {
                int pixelIndex = rowIndex + x;
                int currentId = ColorToProvinceId(provincePixels[pixelIndex]);

                if (currentId == provinceId)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (maxX < minX || maxY < minY)
        {
            Debug.LogWarning("Highlight üretilemedi. Province ID: " + provinceId);
            return;
        }

        int padding = 2;

        minX = Mathf.Max(0, minX - padding);
        minY = Mathf.Max(0, minY - padding);
        maxX = Mathf.Min(width - 1, maxX + padding);
        maxY = Mathf.Min(height - 1, maxY + padding);

        int cropWidth = maxX - minX + 1;
        int cropHeight = maxY - minY + 1;

        Color32[] highlightPixels = new Color32[cropWidth * cropHeight];

        for (int y = 0; y < cropHeight; y++)
        {
            for (int x = 0; x < cropWidth; x++)
            {
                int sourceX = minX + x;
                int sourceY = minY + y;

                int sourceIndex = sourceY * width + sourceX;
                int targetIndex = y * cropWidth + x;

                int currentId = ColorToProvinceId(provincePixels[sourceIndex]);

                if (currentId == provinceId)
                {
                    highlightPixels[targetIndex] = highlightColor;
                }
                else
                {
                    highlightPixels[targetIndex] = new Color32(0, 0, 0, 0);
                }
            }
        }

        Texture2D highlightTexture = new Texture2D(
            cropWidth,
            cropHeight,
            TextureFormat.RGBA32,
            false
        );

        highlightTexture.filterMode = FilterMode.Point;
        highlightTexture.wrapMode = TextureWrapMode.Clamp;
        highlightTexture.SetPixels32(highlightPixels);
        highlightTexture.Apply();

        float highlightPixelsPerUnit = GetProvinceMapPixelsPerUnit();

Sprite highlightSprite = Sprite.Create(
    highlightTexture,
    new Rect(0, 0, cropWidth, cropHeight),
    new Vector2(0.5f, 0.5f),
    highlightPixelsPerUnit,
    0,
    SpriteMeshType.FullRect
);

        Vector3 localPosition = GetLocalPositionForProvinceBounds(minX, minY, maxX, maxY);

        cachedSprites[provinceId] = highlightSprite;
        cachedLocalPositions[provinceId] = localPosition;
    }

    private float GetProvinceMapPixelsPerUnit()
    {
        Sprite visualSprite = mapRenderer.sprite;

        float visualLocalWidth = visualSprite.rect.width / visualSprite.pixelsPerUnit;

        return provinceMapTexture.width / visualLocalWidth;
    }

    private Vector3 GetLocalPositionForProvinceBounds(int minX, int minY, int maxX, int maxY)
    {
        Sprite visualSprite = mapRenderer.sprite;

        float centerX = (minX + maxX + 1) * 0.5f;
        float centerY = (minY + maxY + 1) * 0.5f;

        float u = centerX / provinceMapTexture.width;
        float v = centerY / provinceMapTexture.height;

        float visualPixelX = u * visualSprite.rect.width;
        float visualPixelY = v * visualSprite.rect.height;

        float localX = (visualPixelX - visualSprite.pivot.x) / visualSprite.pixelsPerUnit;
        float localY = (visualPixelY - visualSprite.pivot.y) / visualSprite.pixelsPerUnit;

        return new Vector3(localX, localY, 0f);
    }

    private int ColorToProvinceId(Color32 color)
    {
        return color.r * 65536 + color.g * 256 + color.b;
    }

    private void ClearHighlight()
{
    if (highlightRenderer != null)
    {
        highlightRenderer.enabled = false;
    }
}
}