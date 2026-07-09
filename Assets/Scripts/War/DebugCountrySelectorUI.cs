using UnityEngine;
using UnityEngine.UIElements;

public class DebugCountrySelectorUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private PlayerState playerState;

    private void OnEnable()
    {
        VisualElement root = uiDocument.rootVisualElement;

        root.Q<Button>("TurButton").clicked += () =>
            playerState.SetPlayerCountry("TUR");

        root.Q<Button>("GrcButton").clicked += () =>
            playerState.SetPlayerCountry("GRC");

        root.Q<Button>("BgrButton").clicked += () =>
            playerState.SetPlayerCountry("BGR");
    }
}