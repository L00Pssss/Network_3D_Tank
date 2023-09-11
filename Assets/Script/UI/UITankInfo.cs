using UnityEngine;

public class UITankInfo : MonoBehaviour
{
    [SerializeField] private UIHealthSlider healthSlider;

    [SerializeField] private Vector3 worldOffset;
    public Vector3 WorldOffset => worldOffset;

    private Vehicle tank;

    public Vehicle Tank => tank;

    public void SetTank(Vehicle tank)
    {
        this.tank = tank;

        healthSlider.Initialization(tank, tank.TeamId, Player.Local.TeamId);      
    }
}
