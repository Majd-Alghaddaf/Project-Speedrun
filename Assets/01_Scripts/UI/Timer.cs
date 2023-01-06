using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    #region Singleton
    private static Timer _instance;

    public static Timer Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private TextMeshProUGUI timerText;

    private float _time = 0;

    private int _minutes = 0;
    private int _seconds = 0;
    private int _milliseconds = 0;

    private string _minutesString;
    private string _secondsString;
    private string _millisecondsString;

    private bool paused = false;

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (paused)
            return;

        _time += Time.deltaTime;

        _minutes = Mathf.FloorToInt(_time / 60);
        _seconds = Mathf.FloorToInt(_time) % 60;
        _milliseconds = Mathf.RoundToInt((1 - (Mathf.Ceil(_time) - _time)) * 100);

        if (_milliseconds == 100)
            _milliseconds = 0;

        _minutesString = _minutes.ToString("00");
        _secondsString = _seconds.ToString("00");
        _millisecondsString = _milliseconds.ToString("00");

        timerText.text = $"{_minutesString}:{_secondsString}:{_millisecondsString}";
    }

    public void Stop() 
    {
        Pause();
        ResetTime();
    }
    public void ResetTime() => _time = 0;
    public void Continue() => paused = false;
    public void Pause() => paused = true;

    public float GetTimeFloat()
    {
        return _time;
    }    
    public string GetTimeString()
    {
        return timerText.text;
    }

}
