using TMPro;
using UnityEngine;

public class UIHealthText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        if(Player.Local == null) return;

        if(Player.Local.ActiveVechicle == null) return;

        text.text = Player.Local.ActiveVechicle.HitPoint.ToString();
    }
}
