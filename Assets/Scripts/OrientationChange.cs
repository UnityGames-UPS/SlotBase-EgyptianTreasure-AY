using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class OrientationChange : MonoBehaviour
{
    [SerializeField] private RectTransform UIWrapper;
    [SerializeField] private Image[] targetImages;
    [SerializeField] private float rotationDuration = 0.5f;

    private bool isLandscape;

    void SwitchDisplay(string dimensions)
    {
        StartCoroutine(RotateAndResize(dimensions));
    }

    IEnumerator RotateAndResize(string dimensions)
    {
        string[] parts = dimensions.Split(',');
        if (parts.Length != 2) yield break;

        int width = int.Parse(parts[0]);
        int height = int.Parse(parts[1]);
        isLandscape = width > height;

        // Rotate the UI
        Quaternion targetRot = isLandscape ? Quaternion.identity : Quaternion.Euler(0, 0, -90);
        UIWrapper.DORotate(targetRot.eulerAngles, rotationDuration);

        yield return new WaitForSeconds(rotationDuration);

        // Directly set image sizes
        foreach (Image img in targetImages)
        {
            if (img == null) continue;

            RectTransform rt = img.rectTransform;

            // Remove all anchors and center
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            // Set direct pixel dimensions
            rt.sizeDelta = isLandscape ?
                new Vector2(width, height) :
                new Vector2(height, width); // Swap for portrait
        }

        Debug.Log($"Set all images to: {(isLandscape ? $"{width}x{height}" : $"{height}x{width}")}");
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SwitchDisplay($"{Screen.width},{Screen.height}");
        }
    }
#endif
}