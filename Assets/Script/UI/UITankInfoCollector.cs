using System.Collections.Generic;
using UnityEngine;

public class UITankInfoCollector : MonoBehaviour
{
    [SerializeField] private Transform tankInfoPanel;

    [SerializeField] private UITankInfo tankInfoPrefab;

    private UITankInfo[] tankInfo;

    private List<Vehicle> VehiclesWithoutLocal;

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
        Vehicle[] Vehicles = FindObjectsOfType<Vehicle>();

        VehiclesWithoutLocal = new List<Vehicle>(Vehicles.Length - 1);

        for (int i = 0; i < Vehicles.Length; i++)
        {
            if (Vehicles[i] == Player.Local.ActiveVechicle) continue;

            VehiclesWithoutLocal.Add(Vehicles[i]);
        }

        tankInfo = new UITankInfo[VehiclesWithoutLocal.Count];

        for (int i = 0; i < VehiclesWithoutLocal.Count; i++)
        {
            tankInfo[i] = Instantiate(tankInfoPrefab);

            tankInfo[i].SetTank(VehiclesWithoutLocal[i]);
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

            bool isVisible = Player.Local.ActiveVechicle.vehicleViewer.IsVisible((tankInfo[i].Tank.netIdentity));
            
            tankInfo[i].gameObject.SetActive((isVisible));

            if (tankInfo[i].gameObject.activeSelf == false) continue;

            Vector3 screenPositon = Camera.main.WorldToScreenPoint(tankInfo[i].Tank.transform.position + tankInfo[i].WorldOffset);

            if (screenPositon.z > 0)
            {
                tankInfo[i].transform.position = screenPositon;
            }
        }
    }

}
