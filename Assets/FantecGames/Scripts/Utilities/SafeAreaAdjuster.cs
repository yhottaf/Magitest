using UnityEngine;

public class SafeAreaAdjuster : MonoBehaviour
{
    private void Awake()
    {
        var panel=GetComponent<RectTransform>();
        var area = Screen.safeArea;

        var anchorMin = area.position;
        var anchorMax= area.position+area.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        panel.anchorMin= anchorMin;
        panel.anchorMax= anchorMax;
    }
}
