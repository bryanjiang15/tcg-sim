using UnityEngine;
using UnityEngine.UIElements;
    
//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
public class TabbedMenu : MonoBehaviour
{
    private void OnEnable()
    {
        UIDocument menu = GetComponent<UIDocument>();
        VisualElement root = menu.rootVisualElement;
    }
}
