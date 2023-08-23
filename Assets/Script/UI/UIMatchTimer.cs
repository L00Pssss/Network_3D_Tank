using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class UIMatchTimer : MonoBehaviour
{
    [SerializeField] private MatchTimer timer;
    [SerializeField] private TextMeshProUGUI text;

   // private float lastUpdateTime;
    [SerializeField] private int UpdateInterval = 2;
    bool stoptimer = true;

    private float timerUI;

    private void Start()
    {
        timerUI = timer.MatchTime;
        UpdateTimeText();
    }

    private void Update()
    {
        timerUI += Time.deltaTime;

        if (timerUI >= UpdateInterval && stoptimer == true)
        {
            UpdateTimeText();
            timerUI = 0f;
        }
    }

    private void UpdateTimeText()
    {
        text.text = FormatTime(timer.TimeLeft);
    }

    private string FormatTime(float time)
    {
        return text.text = $"{(time / 60):00}:{(time % 60):00}";
    }
}