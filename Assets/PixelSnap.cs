using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSnap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //fix rotation
        Vector3 eulers = new Vector3(0, roundToCardenal(transform.eulerAngles.y), 0);
        transform.rotation = Quaternion.Euler(eulers);

        //fix position
        transform.position = new Vector3(roundToPixel(transform.position.x), roundToPixel(transform.position.y), roundToPixel(transform.position.z)) + Vector3.up * 0.001f;
    }

    private float roundToCardenal(float f)
    {
        float angle = 90f;
        return Mathf.Round(f * angle) / angle;
    }

    private float roundToPixel(float f)
    {
        float tileSize = 0.85f;
        float pixelsPerTile = 48f;
        float pixelResolution = tileSize / pixelsPerTile;
        return Mathf.Round(f / pixelResolution) * pixelResolution;
    }
}
