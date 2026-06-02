using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GhoulCustomizer : MonoBehaviour
{
    #region Settings
    [Header("References")]
    public GameObject helmetMesh;
    public GameObject clothingMesh;

    [Header("Outfit")]
    public bool hasHelmet;
    public bool hasClothing;
    #endregion

    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += ApplyCustomization;
#endif
    }

    private void ApplyCustomization()
    {
        if (this == null) return;

        if (helmetMesh != null)
            helmetMesh.SetActive(hasHelmet);

        if (clothingMesh != null)
            clothingMesh.SetActive(hasClothing);
    }
}