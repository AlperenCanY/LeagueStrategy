using UnityEngine;

public class ProvinceMapPicker : MonoBehaviour
{
    public Camera mainCamera;
    public SpriteRenderer mapRenderer;
    public Texture2D provinceMapTexture;
    public ProvinceManager provinceManager;

    [Header("Debug")]
    public bool logClickedColor = false;

    public bool TryPickProvince(Vector3 screenPosition, out int provinceId)
    {
        provinceId = 0;

        if (mainCamera == null || mapRenderer == null || provinceMapTexture == null || provinceManager == null)
        {
            Debug.LogError("ProvinceMapPicker bağlantıları eksik.");
            return false;
        }

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
        Vector3 localPos = mapRenderer.transform.InverseTransformPoint(worldPos);

        Sprite sprite = mapRenderer.sprite;
        Rect visualRect = sprite.rect;
        Vector2 pivot = sprite.pivot;
        float ppu = sprite.pixelsPerUnit;

        float visualPixelX = localPos.x * ppu + pivot.x;
        float visualPixelY = localPos.y * ppu + pivot.y;

        float u = visualPixelX / visualRect.width;
        float v = visualPixelY / visualRect.height;

        if (u < 0f || u > 1f || v < 0f || v > 1f)
            return false;

        int x = Mathf.FloorToInt(u * provinceMapTexture.width);
        int y = Mathf.FloorToInt(v * provinceMapTexture.height);

        x = Mathf.Clamp(x, 0, provinceMapTexture.width - 1);
        y = Mathf.Clamp(y, 0, provinceMapTexture.height - 1);

        provinceId = GetProvinceIdFromPixel(x, y);

        return provinceId != 0 && provinceManager.HasProvince(provinceId);
    }

    private int GetProvinceIdFromPixel(int x, int y)
    {
        Color32 color = provinceMapTexture.GetPixel(x, y);
        int id = ColorToProvinceId(color);

        if (logClickedColor)
        {
            Debug.Log("Clicked Color: R=" + color.r + " G=" + color.g + " B=" + color.b + " ID=" + id);
        }

        if (provinceManager.HasProvince(id))
            return id;

        int searchRadius = 3;

        for (int offsetY = -searchRadius; offsetY <= searchRadius; offsetY++)
        {
            for (int offsetX = -searchRadius; offsetX <= searchRadius; offsetX++)
            {
                int px = x + offsetX;
                int py = y + offsetY;

                if (px < 0 || py < 0 || px >= provinceMapTexture.width || py >= provinceMapTexture.height)
                    continue;

                Color32 nearbyColor = provinceMapTexture.GetPixel(px, py);
                int nearbyId = ColorToProvinceId(nearbyColor);

                if (provinceManager.HasProvince(nearbyId))
                    return nearbyId;
            }
        }

        return 0;
    }

    private int ColorToProvinceId(Color32 color)
    {
        return color.r * 65536 + color.g * 256 + color.b;
    }
    public Vector3 GetProvinceCenterWorldPosition(int provinceId)
{
    int width = provinceMapTexture.width;
    int height = provinceMapTexture.height;

    long sumX = 0;
    long sumY = 0;
    int count = 0;

    Color32[] pixels = provinceMapTexture.GetPixels32();

    for (int y = 0; y < height; y++)
    {
        int row = y * width;

        for (int x = 0; x < width; x++)
        {
            int index = row + x;
            int currentId = ColorToProvinceId(pixels[index]);

            if (currentId == provinceId)
            {
                sumX += x;
                sumY += y;
                count++;
            }
        }
    }

    if (count == 0)
        return mapRenderer.transform.position;

    float centerX = (float)sumX / count;
    float centerY = (float)sumY / count;

    float u = centerX / width;
    float v = centerY / height;

    Sprite sprite = mapRenderer.sprite;

    float visualPixelX = u * sprite.rect.width;
    float visualPixelY = v * sprite.rect.height;

    float localX = (visualPixelX - sprite.pivot.x) / sprite.pixelsPerUnit;
    float localY = (visualPixelY - sprite.pivot.y) / sprite.pixelsPerUnit;

    Vector3 localPosition = new Vector3(localX, localY, 0f);

    return mapRenderer.transform.TransformPoint(localPosition);
}
}