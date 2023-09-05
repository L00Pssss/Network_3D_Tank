using TMPro;
using UnityEngine;

public class UIHitResultPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private TextMeshProUGUI damage;

    public void SetTypeResult(string textResult)
    {
        type.text = textResult;
    }

    public void SetDamgeResult(float dmg)
    {
        if (dmg <= 0) return;

        damage.text = "-" + dmg.ToString("F0");
    }
}
