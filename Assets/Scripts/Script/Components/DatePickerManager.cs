using System;
using System.Collections.Generic;
using UnityEngine;

public class DatePickerManager : MonoBehaviour
{
    [Header("Wheel References")]
    public DatePickerWheel monthWheel;
    public DatePickerWheel dayWheel;
    public DatePickerWheel yearWheel;
    
    [Header("Current Date")]
    public int currentMonth = System.DateTime.Now.Month;
    public int currentDay = System.DateTime.Now.Day;
    public int currentYear = System.DateTime.Now.Year;
    
    private readonly string[] monthNames = {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };
    
    public System.Action<DateTime> OnDateChanged;
    
    private void Start()
    {
        SetupWheels();
        SetCurrentDate(DateTime.Now);
    }

    public void Skip()
    {
        AnalyticsManager.Instance.LogEvent("date_picker_skip", 
            new Dictionary<string, string> { { "month", currentMonth.ToString() }, { "day", currentDay.ToString() }, { "year", currentYear.ToString() } });
    }

    public void Continue()
    {
        AnalyticsManager.Instance.LogEvent("date_picker_continue", 
            new Dictionary<string, string> { { "month", currentMonth.ToString() }, { "day", currentDay.ToString() }, { "year", currentYear.ToString() } });
    }
    
    private void SetupWheels()
    {
        // Setup month wheel
        List<string> months = new List<string>(monthNames);
        monthWheel.SetData(months);
        monthWheel.OnValueChanged += OnMonthChanged;
        
        // Setup year wheel
        List<string> years = new List<string>();
        for (int year = 1990; year <= System.DateTime.Now.Year; year++)
        {
            years.Add(year.ToString());
        }
        yearWheel.SetData(years);
        yearWheel.OnValueChanged += OnYearChanged;
        
        // Setup day wheel
        UpdateDayWheel();
        dayWheel.OnValueChanged += OnDayChanged;
    }
    
    private void OnMonthChanged(int index, string value)
    {
        currentMonth = index + 1;
        UpdateDayWheel();
        NotifyDateChanged();
    }
    
    private void OnDayChanged(int index, string value)
    {
        currentDay = index + 1;
        NotifyDateChanged();
    }
    
    private void OnYearChanged(int index, string value)
    {
        currentYear = 1990 + index;
        UpdateDayWheel();
        NotifyDateChanged();
    }
    
    private void UpdateDayWheel()
    {
        int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        List<string> days = new List<string>();
        
        for (int day = 1; day <= daysInMonth; day++)
        {
            days.Add(day.ToString());
        }
        
        dayWheel.SetData(days);
        
        // Adjust current day if it's now invalid
        if (currentDay > daysInMonth)
        {
            currentDay = daysInMonth;
            dayWheel.SetSelectedIndex(currentDay - 1);
        }
    }
    
    public void SetCurrentDate(DateTime date)
    {
        currentYear = date.Year;
        currentMonth = date.Month;
        currentDay = date.Day;
        
        monthWheel.SetSelectedIndex(currentMonth - 1);
        yearWheel.SetSelectedIndex(currentYear - 1990);
        UpdateDayWheel();
        dayWheel.SetSelectedIndex(currentDay - 1);
    }
    
    private void NotifyDateChanged()
    {
        try
        {
            DateTime selectedDate = new DateTime(currentYear, currentMonth, currentDay);
            OnDateChanged?.Invoke(selectedDate);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Invalid date: " + e.Message);
        }
    }
    
    public DateTime GetSelectedDate()
    {
        return new DateTime(currentYear, currentMonth, currentDay);
    }
}