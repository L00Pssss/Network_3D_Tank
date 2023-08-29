using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class UIPlayerLable : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fragText;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private Image backgroundImgae;
    [SerializeField] private Color selfColor;

    private int netId;
    public int NetId => netId;

    public void Initialized(int netId, string nickname)
    {
        this.netId = netId;
        nicknameText.text = nickname;

        if (netId == Player.Local.netId)
        {
            backgroundImgae.color = selfColor;
        }
    }

    public void UpdateFrag(int frag)
    {
        fragText.text = frag.ToString();
    }

}
