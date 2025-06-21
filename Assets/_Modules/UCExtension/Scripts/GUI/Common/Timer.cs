using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UCExtension.GUI
{
    [RequireComponent(typeof(Text))]
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TimerType type;

        Text countDownText;

        public Text CountDownText => countDownText;

        IEnumerator ieCountDown;

        bool isStop = false;

        int currentSecond;

        public int CurrentSecond { get { return currentSecond; } }

        public void SetType(TimerType type)
        {
            this.type = type;
        }
        public void CountDown(int second, UnityAction finish = null)
        {
            if (!countDownText)
            {
                countDownText = GetComponent<Text>();
            }
            currentSecond = second;
            if (ieCountDown != null) UCEManager.Ins.StopCoroutine(ieCountDown);
            ieCountDown = IEHourCountDown(finish);
            UCEManager.Ins.StartCoroutine(ieCountDown);
        }
        IEnumerator IEHourCountDown(UnityAction finish = null)
        {

            while (currentSecond > 0) // run until second = 0
            {
                countDownText.text = GetCountDownFormat(currentSecond, type);
                currentSecond--;
                yield return new WaitForSecondsRealtime(1);
                yield return new WaitUntil(() => !isStop);
            }
            finish?.Invoke();
        }

        public void AddSecond(int second)
        {
            currentSecond += second;
        }
        public void StopCountDown(bool completely = false)
        {
            if (completely)
            {
                if (ieCountDown != null)
                {
                    StopCoroutine(ieCountDown);
                }
            }
            else
            {
                isStop = true;
            }
        }
        public void ContinueCountDown()
        {
            isStop = false;
        }
        public string GetCountDownFormat(int second, TimerType type = TimerType.second)
        {
            switch (type)
            {
                case TimerType.hour:
                    return DateTimeExtension.GetHourFormat(second);
                case TimerType.minute:
                    return DateTimeExtension.GetMinuteFormat(second);
                default:
                    return second.ToString();
            }
        }
        // 1 minute = 60 second
        // 1 hour = 60 minute = 60*60 second;
    }
    public enum TimerType
    {
        hour,
        minute,
        second
    }
}