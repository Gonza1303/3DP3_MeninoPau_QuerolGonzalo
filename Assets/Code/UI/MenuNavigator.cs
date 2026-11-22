using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Keeps a UI element focused while this panel is active so it can be driven with
/// keyboard / gamepad and not only the mouse. Put one on each menu panel that is
/// toggled on/off and assign the element that should be focused when it opens.
/// </summary>
public class MenuNavigator : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private void OnEnable()
    {
        // Wait one frame so the panel's children are active and the EventSystem has
        // finished its own start-up selection before we override it.
        StartCoroutine(SelectAtEndOfFrame());
    }

    private IEnumerator SelectAtEndOfFrame()
    {
        yield return null;
        Select();
    }

    private void Update()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null) return;

        // Recover focus if it was lost (e.g. a mouse click on empty space),
        // otherwise the arrows / stick would stop responding.
        GameObject current = eventSystem.currentSelectedGameObject;
        if (current == null || !current.activeInHierarchy)
        {
            Select();
        }
    }

    private void Select()
    {
        if (firstSelected == null || !firstSelected.activeInHierarchy) return;

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null) return;

        eventSystem.SetSelectedGameObject(firstSelected);
    }
}