using UnityEngine;

[ExecuteAlways]
public class GhostCustomizer : MonoBehaviour
{
    #region Settings
    public enum HostType { Host_001, Host_002, Host_003 }

    [Header("Host Settings")]
    public HostType hostType;

    [Header("Host Meshes")]
    public GameObject ghoulMesh_001;
    public GameObject ghoulMesh_002;
    public GameObject ghoulMesh_003;
    #endregion

    private void OnValidate()
    {
        //wird bereits durch Editor Unput geupdated
        UnityEditor.EditorApplication.delayCall += ApplyCustomization;
    }

    private void ApplyCustomization()
    {
        //gewünschte Meshes können per Skript aktiviert oder deaktiviert werden
        if (this == null) return;

        if (ghoulMesh_001 != null) ghoulMesh_001.SetActive(hostType == HostType.Host_001);
        if (ghoulMesh_002 != null) ghoulMesh_002.SetActive(hostType == HostType.Host_002);
        if (ghoulMesh_003 != null) ghoulMesh_003.SetActive(hostType == HostType.Host_003);
    }
}


