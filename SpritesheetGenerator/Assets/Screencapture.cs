using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Screencapture : MonoBehaviour {

    public GameObject ObjectToCapture;
    public GameObject Rendererererer;
    public AnimationClip ac;
    public int FramesToCapture;
    public int Width;
    public int Height;

    string filePath = Directory.GetCurrentDirectory() + "\\capture\\";
    int cnt = 0;

	// Use this for initialization
	void Start () {
        if (ObjectToCapture == null) return;
        if (FramesToCapture <= 0) return;

        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        StartCoroutine(CaptureScreen());
    }

    IEnumerator CaptureScreen()
    {
        for (int i = 0; i < FramesToCapture; i++)
        {
            yield return new WaitForEndOfFrame();
            ac.SampleAnimation(ObjectToCapture, 1f / FramesToCapture * i);
            SaveScreenshot();
        }
    }

    void SaveScreenshot()
    {
        Texture2D tex = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight), 0, 0);

        //Magenta to transparent
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                Color c = tex.GetPixel(x, y);

                if (c == Color.magenta)
                {
                    tex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
        }

        TextureScale.Point(tex, Width, Height);

        string file = filePath + cnt.ToString("D3") + ".png";
        cnt++;
        File.WriteAllBytes(file, tex.EncodeToPNG());
        Debug.Log(file);

        Destroy(tex);
    }

    Rect GUIRectWithObject()
    {
        Vector3 cen = Rendererererer.GetComponent<Renderer>().bounds.center;
        Vector3 ext = Rendererererer.GetComponent<Renderer>().bounds.extents;

        Vector2[] extentPoints = new Vector2[8]
        {
            HandleUtility.WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z - ext.z)),
            HandleUtility.WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z - ext.z)),
            HandleUtility.WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z + ext.z)),
            HandleUtility.WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z + ext.z)),
            HandleUtility.WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z - ext.z)),
            HandleUtility.WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z - ext.z)),
            HandleUtility.WorldToGUIPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z + ext.z)),
            HandleUtility.WorldToGUIPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z + ext.z)),
        };

        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];

        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }

        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }
}
