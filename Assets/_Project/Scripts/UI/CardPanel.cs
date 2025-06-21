using API.LogEvent;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    [SerializeField] private Text totaltext;
    [SerializeField] private List<CashBtn> buttons;
    [SerializeField] private Button btnDelete;
    [SerializeField] private Button btnOK;
    [SerializeField] private Button btnClear;

    private float currentInput = 0f;
    private bool hasDecimal = false;
    private int decimalPlaces = 0;

    public static Action<float> OnConfirmPayment;

    private void Start()
    {
        totaltext.text = "0";
        foreach (var btn in buttons)
        {
            btn.OnClick(() => PressBtn(btn.Value));
        }
        btnClear.onClick.AddListener(Clear);
        btnDelete.onClick.AddListener(Backspace);
        btnOK.onClick.AddListener(Confirm);
    }
    public void PressBtn(int value)
    {
        Vibrator.SoftVibrate();
        if (value >= 0 && value <= 9)
        {
            AddDigit(value);
        }
        else if (value == -1) // .
        {
            AddDecimal();
        }
    }

    private void AddDigit(int digit)
    {
        if (hasDecimal)
        {
            if (decimalPlaces >= 2) return;
            decimalPlaces++;
            currentInput += digit / Mathf.Pow(10, decimalPlaces);
        }
        else
        {
            if ((int)currentInput >= 99999) return; // giới hạn số lớn
            currentInput = currentInput * 10 + digit;
        }

        UpdateDisplay();
    }

    private void AddDecimal()
    {
        if (!hasDecimal)
        {
            hasDecimal = true;
            UpdateDisplay();
        }
    }
    private void UpdateDisplay()
    {
        if (hasDecimal && decimalPlaces == 0)
        {
            totaltext.text = ((int)currentInput).ToString() + ".";
        }
        else
        {
            string format = hasDecimal ? "F" + decimalPlaces : "F0";
            totaltext.text = currentInput.ToString(format);
        }
    }
    private void Backspace()
    {
        Vibrator.SoftVibrate();
        if (hasDecimal && decimalPlaces > 0)
        {
            // Remove last decimal digit
            float power = Mathf.Pow(10, decimalPlaces);
            float lastDigit = (currentInput * power) % 10;
            currentInput -= lastDigit / power;
            decimalPlaces--;
        }
        else if (hasDecimal)
        {
            // Remove decimal point
            hasDecimal = false;
        }
        else
        {
            // Remove last integer digit
            currentInput = Mathf.Floor(currentInput / 10);
        }

        UpdateDisplay();
    }

    private void Clear()
    {
        Vibrator.SoftVibrate();
        currentInput = 0f;
        hasDecimal = false;
        decimalPlaces = 0;
        totaltext.text = "0";
    }
    private void Confirm()
    {
        // Round to 2 decimal places for final amount
        float amount = (float)Math.Round(currentInput, 2);
        OnConfirmPayment?.Invoke(amount);
        Vibrator.SoftVibrate();
        Clear();
    }
}
