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
}
