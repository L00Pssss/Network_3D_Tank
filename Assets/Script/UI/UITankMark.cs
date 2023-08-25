using UnityEngine;
using UnityEngine.UI;

public class UITankMark : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private Color localTeamColor;
    [SerializeField] private Color otherTeamColor;

    public void SetLocalColor()
    {
        image.color = localTeamColor;
    }

    public void SetOtherColor()
    {
        image.color = otherTeamColor;
    }
}
