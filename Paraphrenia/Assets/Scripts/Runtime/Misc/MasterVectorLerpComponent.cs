using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterVectorLerpComponent : MonoBehaviour
{
    enum StartInput { Start, Update, ButtonPress, None };
    [SerializeField] private StartInput _startInput;
    [SerializeField] private string _inputButton = "space";
    [SerializeField] private float _frameRate = 30f;
    [SerializeField] private float _duration = 1f;
    private float _deltaTime;
    private float _percent;

    [SerializeField] protected Vector3 _targetVector;
    [SerializeField] protected Vector3 _startVector;
    protected Vector3 _direction;

    [SerializeField] private bool _showDebug = false;
    [SerializeField] private bool _useObjectLocation = true;
    [SerializeField] private bool _forceFinalVectorEqualTargetVector = true;

    void Start()
    {
        _deltaTime = 1 / _frameRate;
        if (_useObjectLocation)
        {
            _startVector += this.gameObject.transform.position;
            _targetVector += this.gameObject.transform.position;
        }
        _direction = _targetVector - _startVector;
        if (_startInput == StartInput.Start)
        {
            StartLerp();
        }
    }

    
    void Update()
    {
        if (_startInput == StartInput.ButtonPress && Input.GetKeyDown(_inputButton))
        {
            StartLerp();
        }
        if (_startInput == StartInput.Update)
        {
            StartLerp();
        }
    }
    public void ForceStartLerp()
    {
        if(_startInput == StartInput.None){StartLerp();}
    }

    private void StartLerp()
    {
        if (_showDebug){Debug.Log("Lerp Started");}
        _percent = 0f;
        StartCoroutine(Lerp(_deltaTime));
    }

    protected virtual float CalculateEaseStep(float currentPercent){return currentPercent;}
    protected virtual void ApplyLerp(Vector3 result){}
    protected virtual void OnLerpStart(){}
    protected virtual void OnLerpUpdate(){}
    protected virtual void OnLerpEnd(){}

    IEnumerator Lerp(float dt)
    {
        OnLerpStart();
        while (_percent < 1)
        {
            OnLerpUpdate();
            if (_showDebug){Debug.Log("Lerp Updating");}
            _percent += dt / _duration;
            if (_showDebug){Debug.Log(_percent);}
            float easeStep = CalculateEaseStep(_percent);
            Vector3 result = _direction * easeStep;
            ApplyLerp(result);
            yield return new WaitForSeconds(dt);
        }
        if (_forceFinalVectorEqualTargetVector) { ApplyLerp(_targetVector); }
        OnLerpEnd();
    }
}
