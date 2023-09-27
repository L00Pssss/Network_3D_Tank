using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    [SerializeField] private Transform mainCanvas;
    
    [SerializeField] private SizeMap sizeMap;

    [SerializeField] private UITankMark tankMarkPrefab;

    [SerializeField] protected Image background;

    private UITankMark[] tankMarks;
    private Vehicle[] vehicles;

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
        vehicles = FindObjectsOfType<Vehicle>();

        tankMarks = new UITankMark[vehicles.Length];

        for (int i = 0; i < tankMarks.Length; i++) 
        {
            tankMarks[i] = Instantiate(tankMarkPrefab);

            if (vehicles[i].TeamId == Player.Local.TeamId)
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
        tankMarks = null; // �� �����. 
    }

    private void Update()
    {

        if (tankMarks == null) return;

        for (int i = 0; i < tankMarks.Length; i++)
        {
            if (vehicles[i] == null) continue;

            if (vehicles[i] != Player.Local.ActiveVehicle)
            {
                bool isVisible = Player.Local.ActiveVehicle.vehicleViewer.IsVisible((vehicles[i].netIdentity));
            
                tankMarks[i].gameObject.SetActive((isVisible));
            }
    
            if (tankMarks[i].gameObject.activeSelf == false) continue;

            Vector3 normalPositon = sizeMap.GetNormalPositon(vehicles[i].transform.position);

            Vector3 posInMinimap = new Vector3(normalPositon.x * background.rectTransform.sizeDelta.x * 0.5f,
                normalPositon.z * background.rectTransform.sizeDelta.y * 0.5f, 0);
            
            posInMinimap.x *= mainCanvas.localScale.x;
            posInMinimap.y *= mainCanvas.localScale.y;

            tankMarks[i].transform.position = background.transform.position + posInMinimap;
        }
    }
}
