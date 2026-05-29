using UnityEngine;

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
        //wird bereits durch Editor Unput geupdated
        UnityEditor.EditorApplication.delayCall += ApplyCustomization;
    }

    private void ApplyCustomization()
    {
        //bestimmte Meshes am Ghoul können per Skript aktiviert oder deaktiviert werden
        if (this == null) return;
        if (helmetMesh != null) helmetMesh.SetActive(hasHelmet);
        if (clothingMesh != null) clothingMesh.SetActive(hasClothing);
    }
}