using System.Collections.Generic;
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
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        if (NetworkSessionManager.Instance != null && NetworkSessionManager.Events != null)
            NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
        if (turret != null)
            turret.UpdateSelectedAmmunation -= OnTurretUpdateSlectedAmmunation;
        for (int i = 0; i < allAmmunitions.Count; i++)
        {
            allAmmunitions[i].AmmoCountChanged -= OnAmmoCountChanged;
        }
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        turret = vehicle.Turret;

        turret.UpdateSelectedAmmunation += OnTurretUpdateSlectedAmmunation;

        for (int i = 0; i < turret.Ammunitions.Length; i++)
        {
            UIAmmunitionElement ammunitionElement = Instantiate(ammunitionElementPrefab);
            ammunitionElement.transform.SetParent(ammunitionPanel);
            ammunitionElement.transform.localPosition = Vector3.one;
            ammunitionElement.SetAmmunition(turret.Ammunitions[i]);

            turret.Ammunitions[i].AmmoCountChanged += OnAmmoCountChanged;
            
            allAmmunitionElements.Add(ammunitionElement);
            allAmmunitions.Add(turret.Ammunitions[i]);
            
            if (i == 0)
            {
                ammunitionElement.Select();
            }
        }
        
    }

    private void OnAmmoCountChanged(int ammoCount)
    {
        allAmmunitionElements[turret.SelectedAmmunitionIndex].UpdateAmmoCount(ammoCount);
    }

    private void OnTurretUpdateSlectedAmmunation(int index)
    {
        allAmmunitionElements[lastSelectionAmmunationIndex].UnSelect();
        allAmmunitionElements[index].Select();

        lastSelectionAmmunationIndex = index;
    }
}
