using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
public class MaterialChanger : MonoBehaviour
{
    public Material[] materials;
    public int selectedMaterial;
    public bool applyToChildren = false;

    private void OnValidate()
    {
        if (materials == null || materials.Length == 0) return;
        selectedMaterial = Mathf.Clamp(selectedMaterial, 0, materials.Length - 1);

        UnityEditor.EditorApplication.delayCall += () =>
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
                if (r != null) r.sharedMaterial = mat;
            }
        };
    }
}
#endif
