using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Transform cam;
    [Range(0f, 2f)] public float parallaxFactor = 0.5f; // 0 = locked to camera, 1 = same speed as camera
    public bool infiniteTiling = true;
    public bool xOnly = true;            // lock Y so layers don’t drift vertically
    public bool pixelSnap = true;
    public float pixelsPerUnit = 100f;   // match your sprite PPU

    private Transform[] tiles;
    private float tileWidthWorld;
    private float originX;               // ensures no jump on Play
    private float startY;                // preserves your Scene Y placement

    void Start()
    {
        if (cam == null) cam = Camera.main ? Camera.main.transform : null;
        if (cam == null) { enabled = false; return; }

        // cache children as tiles
        int n = transform.childCount;
        tiles = new Transform[n];
        for (int i = 0; i < n; i++) tiles[i] = transform.GetChild(i);

        // get sprite width from first tile
        var sr = n > 0 ? tiles[0].GetComponent<SpriteRenderer>() : null;
        tileWidthWorld = sr ? sr.bounds.size.x : 0f;

        // record current placement as the baseline
        startY = transform.position.y;
        // Origin so that: x = originX + cam.x * factor  equals current x at Start
        originX = transform.position.x - cam.position.x * parallaxFactor;
    }

    void LateUpdate()
    {
        // parallax X (preserve your Scene Y)
        float x = originX + cam.position.x * parallaxFactor;
        float y = xOnly ? startY : (startY + (cam.position.y * parallaxFactor));

        if (pixelSnap && pixelsPerUnit > 0f) {
            x = Mathf.Round(x * pixelsPerUnit) / pixelsPerUnit;
            if (!xOnly) y = Mathf.Round(y * pixelsPerUnit) / pixelsPerUnit;
        }

        transform.position = new Vector3(x, y, transform.position.z);

        if (!infiniteTiling || tileWidthWorld <= 0f || tiles == null || tiles.Length == 0) return;

        // wrap tiles horizontally
        foreach (var tile in tiles)
        {
            float diff = cam.position.x - tile.position.x;
            if (diff > tileWidthWorld)
                tile.position += new Vector3(tileWidthWorld * tiles.Length, 0f, 0f);
            else if (diff < -tileWidthWorld)
                tile.position -= new Vector3(tileWidthWorld * tiles.Length, 0f, 0f);
        }
    }
}
