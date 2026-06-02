using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MaterialChanger : MonoBehaviour
{
    public Material[] materials;
    public int selectedMaterial;
    public bool applyToChildren = false;

    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += ApplyMaterialCustomization;
#endif
    }

    private void ApplyMaterialCustomization()
    {
        if (materials == null || materials.Length == 0) return;

        selectedMaterial = Mathf.Clamp(selectedMaterial, 0, materials.Length - 1);

#if UNITY_EDITOR
        EditorApplication.delayCall += () =>
        {
            if (this == null) return;

            Material mat = materials[selectedMaterial];

            if (applyToChildren)
            {
                foreach (Renderer r in GetComponentsInChildren<Renderer>())
                    r.sharedMaterial = mat;
            }
            else
            {
                Renderer r = GetComponent<Renderer>();

                if (r != null)
                    r.sharedMaterial = mat;
            }
        };
#endif
    }
}

