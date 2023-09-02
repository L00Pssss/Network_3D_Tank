using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderImages;

    [SerializeField] private Color localTeamColor;
    [SerializeField] private Color otherTeamColor;

    private Destructible destructible;

    public void Initialization(Destructible destructible,int destructibleTeamId, int localplayerTeamId)
    {
        this.destructible = destructible;

        destructible.HitPointChanged += OnHitPointChanged;
        slider.maxValue = destructible.MaxHitPoint;
        slider.value = slider.maxValue;

        if (localplayerTeamId == destructibleTeamId)
        {
            SetLocalColor();
        }
        else
        {
            SetOtherColor();
        }
    }

    private void OnDestroy()
    {
        if (destructible != null) return;

        destructible.HitPointChanged -= OnHitPointChanged;
    }

    private void OnHitPointChanged(float hitPoint)
    {
        slider.value = hitPoint;
    }

    private void SetLocalColor()
    {
        sliderImages.color = localTeamColor;
    }

    private void SetOtherColor()
    {
        sliderImages.color = otherTeamColor;
    }
}
