using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ObjectMover : MasterLerpComponent
{
    [SerializeField] private bool localSpace = true;
    [SerializeField] private Vector3 targetPosition;

    private Vector3 defaultPosition;
    private Vector3 currentPosition;
    private Vector3 direction;

    private void Awake()
    {
        if (localSpace)
        {
            defaultPosition = currentPosition = this.transform.localPosition;
            targetPosition += defaultPosition;
        }
        else
        {
            defaultPosition = currentPosition = this.transform.position;
        }
    }

    public void MoveOn()
    {
        if (localSpace)
        {
            currentPosition = this.transform.localPosition;
        }
        else
        {
            currentPosition = this.transform.position;
        }
        direction = targetPosition - currentPosition;
        if (showDebug) { Debug.Log("Move On! " + direction); }
        StartLerp();
    }

    public void MoveOff()
    {
        if (localSpace)
        {
            currentPosition = this.transform.localPosition;
        }
        else
        {
            currentPosition = this.transform.position;
        }
        direction = defaultPosition - currentPosition;
        if (showDebug) { Debug.Log("Move Off! " + direction); }
        StartLerp();
    }

    protected override void ApplyLerp(float easeStep)
    {
        Vector3 result = direction * easeStep;
        if (localSpace)
        {
            this.transform.localPosition = currentPosition + result;
        }
        else
        {
            this.transform.position = currentPosition + result;
        }
    }
}
