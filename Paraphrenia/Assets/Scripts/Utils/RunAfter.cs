using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunAfter : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private bool startOnAwake = false;

    private bool _isActive = false;
    private float _targetTime = 0;
    private bool _didComplete = false;

    public UnityEvent onTimeElapse = new();
    
    private void Awake()
    {
        if (startOnAwake)
        {
            _targetTime = Time.time + delay;
            _isActive = true;
        }
    }

    public void Reset()
    {
        _didComplete = false;
        _isActive = false;
    }

    public void StartTimer()
    {
        _targetTime = Time.time + delay;
        _isActive = true;
    }
    
    private void Update()
    {
        if (_isActive && Time.time > _targetTime && !_didComplete)
        {
            onTimeElapse?.Invoke();
            _didComplete = true;
        }
    }
}
