using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] protected Button firstSelected;

    private void OnEnable()
    {
        if (firstSelected != null)
            SetFirstSelected(firstSelected);
    }

    private void SetFirstSelected(Button button)
    {
        firstSelected = button;
        firstSelected.Select();
    }
}