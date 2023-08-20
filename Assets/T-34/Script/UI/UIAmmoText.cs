using TMPro;
using UnityEngine;

public class UIAmmoText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        if (Player.Local == null) return;

        if (Player.Local.ActiveVechicle == null) return;

        text.text = Player.Local.ActiveVechicle.Turret.AmmoCount.ToString();
    }
}
