using TMPro;
using UnityEngine;

public class UIMatchTimer : MonoBehaviour
{
    [SerializeField] private MatchTimer timer;
    [SerializeField] private TextMeshProUGUI text;


    [SerializeField] private int UpdateInterval = 2;

    private float timerUI;

    private void Start()
    {
        timerUI = timer.MatchTime;
        UpdateTimeText();
    }

    private void Update()
    {
        timerUI += Time.deltaTime;

        if (timerUI >= UpdateInterval)
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


    //private void Update()
    //{
    //    text.text = timer.TimeLeft.ToString("F0");
    //}
}