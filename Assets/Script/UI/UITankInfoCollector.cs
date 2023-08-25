using System.Collections.Generic;
using UnityEngine;

public class UITankInfoCollector : MonoBehaviour
{
    [SerializeField] private Transform tankInfoPanel;

    [SerializeField] private UITankInfo tankInfoPrefab;

    private UITankInfo[] tankInfo;

    private List<Player> playersWithoutLocal;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Match != null)
        {
            NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }
    }

    private void OnMatchStart()
    {
        Player[] players = FindObjectsOfType<Player>();

        playersWithoutLocal = new List<Player>(players.Length - 1);

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == Player.Local) continue;

            playersWithoutLocal.Add(players[i]);
        }

        tankInfo = new UITankInfo[playersWithoutLocal.Count];

        for (int i = 0; i < playersWithoutLocal.Count; i++)
        {
            tankInfo[i] = Instantiate(tankInfoPrefab);

            tankInfo[i].SetTank(playersWithoutLocal[i].ActiveVechicle);
            tankInfo[i].transform.SetParent(tankInfoPanel);
        }
    }

    private void OnMatchEnd()
    {
        for (int i = 0; i < tankInfoPanel.transform.childCount; i++)
        {
            Destroy(tankInfoPanel.transform.GetChild(i).gameObject);
        }

        tankInfo = null;
    }

    private void Update()
    {
        if (tankInfo == null) return;

        for (int i = 0; i < tankInfo.Length; i++)
        {
            if(tankInfo[i] == null) continue;
            Vector3 screenPositon = Camera.main.WorldToScreenPoint(tankInfo[i].Tank.transform.position + tankInfo[i].WorldOffset);

            if (screenPositon.z > 0)
            {
                tankInfo[i].transform.position = screenPositon;
            }
        }
    }

}
