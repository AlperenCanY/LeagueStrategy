using System.Collections;
using UnityEngine;

public class CountryMapOverlay : MonoBehaviour
{
    public Texture2D provinceMapTexture;
    public SpriteRenderer mapRenderer;
    public ProvinceManager provinceManager;
    public CountryManager countryManager;

    [Header("Visual")]
    public byte overlayAlpha = 130;

    private SpriteRenderer overlayRenderer;
    private Texture2D overlayTexture;
    private Color32[] provincePixels;

    private void Start()
    {
        CreateOverlayRenderer();
        CacheProvincePixels();

        // CountryManager Start içinde ülke oluşturduğu için 1 frame bekliyoruz.
        StartCoroutine(BuildOverlayNextFrame());
    }

    private IEnumerator BuildOverlayNextFrame()
    {
        yield return null;
        RebuildOverlay();
    }

    private void CreateOverlayRenderer()
    {
        GameObject overlayObject = new GameObject("CountryMapOverlay");
        overlayObject.transform.SetParent(mapRenderer.transform, false);
        overlayObject.transform.localPosition = Vector3.zero;
        overlayObject.transform.localRotation = Quaternion.identity;
        overlayObject.transform.localScale = Vector3.one;

        overlayRenderer = overlayObject.AddComponent<SpriteRenderer>();
        overlayRenderer.sortingOrder = mapRenderer.sortingOrder + 1;
    }

    private void CacheProvincePixels()
    {
        provincePixels = provinceMapTexture.GetPixels32();
    }

    public void RebuildOverlay()
    {
        if (provinceMapTexture == null || provinceManager == null || countryManager == null)
        {
            Debug.LogError("CountryMapOverlay bağlantıları eksik.");
            return;
        }

        int width = provinceMapTexture.width;
        int height = provinceMapTexture.height;

        Color32[] overlayPixels = new Color32[width * height];

        for (int i = 0; i < provincePixels.Length; i++)
        {
            int provinceId = ColorToProvinceId(provincePixels[i]);

            if (provinceId == 0)
            {
                overlayPixels[i] = new Color32(0, 0, 0, 0);
                continue;
            }

            ProvinceData province = provinceManager.GetProvinceById(provinceId);

            if (province == null)
            {
                overlayPixels[i] = new Color32(0, 0, 0, 0);
                continue;
            }

            CountryData ownerCountry = countryManager.GetCountry(province.ownerCountry);

            if (ownerCountry == null)
            {
                overlayPixels[i] = new Color32(0, 0, 0, 0);
                continue;
            }

            Color32 color = ownerCountry.mapColor;
            color.a = overlayAlpha;

            overlayPixels[i] = color;
        }

        overlayTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        overlayTexture.filterMode = FilterMode.Point;
        overlayTexture.wrapMode = TextureWrapMode.Clamp;
        overlayTexture.SetPixels32(overlayPixels);
        overlayTexture.Apply();

        float overlayPixelsPerUnit = GetProvinceMapPixelsPerUnit();

Sprite overlaySprite = Sprite.Create(
    overlayTexture,
    new Rect(0, 0, width, height),
    new Vector2(0.5f, 0.5f),
    overlayPixelsPerUnit,
    0,
    SpriteMeshType.FullRect
);
        overlayRenderer.sprite = overlaySprite;

        Debug.Log("Country map overlay oluşturuldu.");
    }

    private float GetProvinceMapPixelsPerUnit()
    {
        Sprite visualSprite = mapRenderer.sprite;
        float visualLocalWidth = visualSprite.rect.width / visualSprite.pixelsPerUnit;

        return provinceMapTexture.width / visualLocalWidth;
    }

    private int ColorToProvinceId(Color32 color)
    {
        return color.r * 65536 + color.g * 256 + color.b;
    }
}