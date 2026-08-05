using UnityEngine;
using UnityEngine.EventSystems;

public class ClearSelectionOnClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}