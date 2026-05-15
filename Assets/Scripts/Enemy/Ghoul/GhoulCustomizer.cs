using UnityEngine;

[ExecuteAlways]
public class GhoulCustomizer : MonoBehaviour
{
    [Header("Outfit")]
    public bool hasHelmet;
    public bool hasClothing;

    [Header("References")]
    public GameObject helmetMesh;
    public GameObject clothingMesh;

    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += ApplyCustomization;
    }

    private void ApplyCustomization()
    {
        if (this == null) return;
        if (helmetMesh != null) helmetMesh.SetActive(hasHelmet);
        if (clothingMesh != null) clothingMesh.SetActive(hasClothing);
    }
}