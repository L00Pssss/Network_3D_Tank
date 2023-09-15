using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UIAmmunitionPanel : MonoBehaviour
{
    [SerializeField] private Transform ammunitionPanel;
    [SerializeField] private UIAmmunitionElement ammunitionElementPrefab;

    private List<UIAmmunitionElement> allAmmunitionElements = new List<UIAmmunitionElement>();
    private List<Ammunition> allAmmunitions = new List<Ammunition>();

    private Turret turret;
    private int lastSelectionAmmunationIndex;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStarted;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Match != null)
        {
            NetworkSessionManager.Match.MatchStart -= OnMatchStarted;
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }

    }

    private void OnMatchStarted()
    {
        turret = Player.Local.ActiveVechicle.Turret;

        turret.UpdateSelectedAmmunation += OnTurretUpdateSlectedAmmunation;
        
        Debug.Log((turret.Ammunitions.Length));
        Debug.Log((turret.name));

        for (int i = 0; i < ammunitionPanel.childCount; i++)
        {
            Destroy(ammunitionPanel.GetChild(i).gameObject);
        }
        
        allAmmunitionElements.Clear();;
        allAmmunitions.Clear();
        
        for (int i = 0; i < turret.Ammunitions.Length; i++)
        {
            
            UIAmmunitionElement ammunitionElement = Instantiate(ammunitionElementPrefab);
            ammunitionElement.transform.SetParent(ammunitionPanel);
            ammunitionElement.transform.localPosition = Vector3.one;
            ammunitionElement.SetAmmunition(turret.Ammunitions[i]);

            turret.Ammunitions[i].AmmoCountChanged += RPCAmmoCountChanged;
            
            allAmmunitionElements.Add(ammunitionElement);
            allAmmunitions.Add(turret.Ammunitions[i]);
            
            if (i == 0)
            {
                ammunitionElement.Select();
            }
        }
    }

    private void OnMatchEnd()
    {
        if (turret != null)
            turret.UpdateSelectedAmmunation -= OnTurretUpdateSlectedAmmunation;
        for (int i = 0; i < allAmmunitions.Count; i++)
        {
            allAmmunitions[i].AmmoCountChanged -= RPCAmmoCountChanged;
        }
    }
    
    private void RPCAmmoCountChanged(int ammoCount)
    {
        Debug.Log(ammoCount + " RPCAmmoCountChanged"  );
        allAmmunitionElements[turret.SelectedAmmunitionIndex].UpdateAmmoCount(ammoCount);
    }

    private void OnTurretUpdateSlectedAmmunation(int index)
    {
        allAmmunitionElements[lastSelectionAmmunationIndex].UnSelect();
        allAmmunitionElements[index].Select();

        lastSelectionAmmunationIndex = index;
    }
}
