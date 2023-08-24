using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIMatchResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private string draw = "Ничья";
    [SerializeField] private string win = "Победа";
    [SerializeField] private string lose = "Поражение";

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Match)
        {
            NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }
    }

    private void OnMatchStart()
    {
        resultPanel.SetActive(false);
    }

    private void OnMatchEnd()
    {
        resultPanel.SetActive(true);

        int winTeamId = NetworkSessionManager.Match.WinTeamId;

        if (winTeamId == -1)
        {
            resultText.text = draw;
            return;
        }

        if (winTeamId == Player.Local.TeamId)
        {
            resultText.text = win;
        }          
        else
        {
            resultText.text = lose;
        }
    }
}
