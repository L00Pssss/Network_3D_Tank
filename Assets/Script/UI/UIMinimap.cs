using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    [SerializeField] private SizeMap sizeMap;

    [SerializeField] private UITankMark tankMarkPrefab;

    [SerializeField] protected Image background;

    private UITankMark[] tankMarks;
    private Player[] players;

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
        players = FindObjectsOfType<Player>();

        tankMarks = new UITankMark[players.Length];

        Debug.Log(tankMarks.Length);

        for (int i = 0; i < tankMarks.Length; i++) 
        {
            tankMarks[i] = Instantiate(tankMarkPrefab);

            if (players[i].TeamId == Player.Local.TeamId)
                tankMarks[i].SetLocalColor();
            else
                tankMarks[i].SetOtherColor();

            tankMarks[i].transform.SetParent(background.transform);
        }
    }

    private void OnMatchEnd()
    {
        for (int i = 0; i < background.transform.childCount; i++)
        {
            Destroy(background.transform.GetChild(i).gameObject);
        }
        tankMarks = null; // на выбор. 
    }

    private void Update()
    {

        if (tankMarks == null) return;

        for (int i = 0; i < tankMarks.Length; i++)
        {
            if (players[i] == null) continue;

            Vector3 normalPositon = sizeMap.GetNormalPositon(players[i].ActiveVechicle.transform.position);

            Vector3 posInMinimap = new Vector3(normalPositon.x * background.rectTransform.sizeDelta.x * 0.5f,
                normalPositon.z * background.rectTransform.sizeDelta.y * 0.5f, 0);

            tankMarks[i].transform.position = background.transform.position + posInMinimap;
        }
    }
}
