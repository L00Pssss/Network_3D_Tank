using UnityEngine;
using UnityEngine.UI;

public class UICaptureBase : MonoBehaviour
{
    [SerializeField] private ConditonCaptureBase conditonCaptureBase;

    [SerializeField] private Slider localTeamSlider;
    [SerializeField] private Slider otherTeamSlider;

    private void Update()
    {
        if (Player.Local == null) return;

        if (Player.Local.TeamId == TeamSide.teamRed)
        {
            UpdateSlider(localTeamSlider, conditonCaptureBase.RedBaseCaptureLevel);
            UpdateSlider(otherTeamSlider, conditonCaptureBase.BlueBaseCaptureLevel);
        }

        if (Player.Local.TeamId == TeamSide.teamBlue)
        {
            UpdateSlider(localTeamSlider, conditonCaptureBase.BlueBaseCaptureLevel);
            UpdateSlider(otherTeamSlider, conditonCaptureBase.RedBaseCaptureLevel);
        }


    }

    private void UpdateSlider(Slider slider, float value)
    {
        if (value == 0)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            if (slider.gameObject.activeSelf == false)
                slider.gameObject.SetActive(true);

            slider.value = value;
        }
    }
}
