using System;
using TMPro;
using UnityEngine;

public class UIMatchTimer : MonoBehaviour
{
    [SerializeField] private MatchTimer Timer;
    [SerializeField] private TextMeshProUGUI text;

    private float lastUpdateTime;
    [SerializeField] private int UpdateInterval = 2; // интервал обновления в секундах

    private void Update()
    {
        if (Timer != null && Timer.IsTriggered == false)
        {
            if (Time.time - lastUpdateTime >= UpdateInterval)
            {
                UpdateTimeText();
                lastUpdateTime += UpdateInterval;
            }
        }
    }

    private void UpdateTimeText()
    {
        Debug.Log(Timer.TimeLeft);
        text.text = FormatTime(Timer.TimeLeft);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }



    //    private string FormatTime(float time)
    //    {
    //        int minutes = Mathf.FloorToInt(time / 60);
    //        int seconds;

    //        if (time >= Timer.TimeLeft)
    //        {
    //            float remainder = time % 60;
    //            int roundedSeconds = Mathf.RoundToInt(remainder);

    //            if (roundedSeconds >= UpdateInterval)
    //            {
    //                seconds = roundedSeconds;
    //            }
    //            else
    //            {
    //                seconds = Mathf.CeilToInt(Timer.TimeLeft % 60); // Округляем начальные секунды вверх до ближайшего целого
    //            }
    //        }
    //        else
    //        {
    //            seconds = Mathf.FloorToInt(Timer.TimeLeft % 60);

    //            if (seconds == Mathf.FloorToInt(time % 60))
    //            {
    //                seconds = Mathf.CeilToInt(time % 60);
    //            }
    //        }

    //        return $"{minutes:00}:{seconds:00}";
    //    }



    //////
    //private string FormatTime(float time)
    //{
    //    TimeSpan timeSpan = TimeSpan.FromSeconds(time);
    //    return timeSpan.ToString(@"mm\:ss");
    //}

}
