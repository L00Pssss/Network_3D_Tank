using UnityEngine;
using UnityEngine.Serialization;

public class UIHiddenPanel : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjectScreens;
    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += HiddenPanel;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Match != null)
        {
            NetworkSessionManager.Match.MatchStart -= HiddenPanel;
        }
    }
    
    private void HiddenPanel()
    {
        if (gameObjectScreens == null) return;

        foreach (var screen in gameObjectScreens)
        {
            screen.SetActive(false);
        }
    }
}
