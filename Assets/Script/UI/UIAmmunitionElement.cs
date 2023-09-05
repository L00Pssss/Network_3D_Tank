using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAmmunitionElement : MonoBehaviour
{
    [SerializeField] private  TextMeshProUGUI ammCountText;
    [SerializeField] private Image projectileIcon;
    [SerializeField] private GameObject selectionBorder;

    public void SetAmmunition(Ammunition ammunition)
    {
        projectileIcon.sprite = ammunition.ProjectileProperties.Icon;
        
        UpdateAmmoCount(ammunition.AmmCount);
    }
    
    public void UpdateAmmoCount(int count)
    {
        ammCountText.text = count.ToString();
    }

    public void Select()
    {
        selectionBorder.SetActive(true);
    }

    public void UnSelect()
    {
        selectionBorder.SetActive(false);
    }
}
