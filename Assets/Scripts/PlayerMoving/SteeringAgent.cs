using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayerMoveing;
using UnityEngine;

public class SteeringAgent : MonoBehaviour {
    public float maxCurrentSpeed;

    public float maxSteeringSpeed;

    public Vector3 currentVelocity;

    public Vector3 steeringVelocity;

    private List<SteeringBehaviour> steeringBehvaiours = new List<SteeringBehaviour>();

    public GameObject queueFloor;

    public float idleTimer;
    private float _idleTimerUpdate = 0f;

    public float settleTime = 0.3f;

    public Vector3 targetPosition;

    private bool _IsIdle = false;

    private GameManager gameManager;

    private void OnEnable() {
        if (queueFloor == null) {
            enabled = false; // not placed yet
            return;
        }

        currentVelocity = Vector3.zero;
        steeringVelocity = Vector3.zero;
        _idleTimerUpdate = 0f;
        _IsIdle = false;
        targetPosition = new Vector3(queueFloor.transform.position.x, 0.5f, queueFloor.transform.position.z);
        SetSeekEnabled(true);

        gameManager = GameManager.Instance;
    }

    private void Update() {
        if (_IsIdle) {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, settleTime);

            _idleTimerUpdate += Time.deltaTime;

            if (_idleTimerUpdate >= idleTimer) {
                _idleTimerUpdate = 0f;
                _IsIdle = false;
                targetPosition = SteeringHelper.GetTargetPosition(queueFloor);
                SetSeekEnabled(true);
            }

            return;
        }
        CooperativeArbitration();
        DrawDebug();

        if (!gameManager.pause)
        {
            UpdatePosition();
            UpdateDirection();
        }

        if (SteeringHelper.IsAtTarget(transform.position, targetPosition)) {
            SetSeekEnabled(false);
            steeringVelocity = Vector3.zero;
            _IsIdle = true;
        }
    }

    private void SetSeekEnabled(bool value) {
        GetComponents<SteeringBehaviour>(steeringBehvaiours);
        foreach (var seek in steeringBehvaiours.OfType<Seek>()) {
            seek.enabled = value;
        }
    }

    protected virtual void CooperativeArbitration()
    {
        steeringVelocity = Vector3.zero;

        GetComponents<SteeringBehaviour>(steeringBehvaiours);
        foreach (SteeringBehaviour currentBehaviour in steeringBehvaiours)
        {
            if (currentBehaviour.enabled)
            {
                steeringVelocity += currentBehaviour.UpdateBehaviour(this) * currentBehaviour.Weight;
            }
        }
    }

    protected virtual void UpdatePosition()
    {
        var limitedsteeringVelocity = SteeringHelper.LimitVector(steeringVelocity, maxSteeringSpeed * Time.deltaTime);
        steeringVelocity -= limitedsteeringVelocity;

        currentVelocity += limitedsteeringVelocity;
        currentVelocity = SteeringHelper.LimitVector(currentVelocity, maxCurrentSpeed);

        transform.position += currentVelocity * Time.deltaTime;
    }

    protected virtual void UpdateDirection()
    {
        if (currentVelocity.sqrMagnitude > 0.0f)
        {
            transform.forward = Vector3.Normalize(new Vector3(currentVelocity.x, 0.0f, currentVelocity.z));
        }
    }
    
    [Header("Debug")]
    public bool drawDebug = true;

    private void DrawDebug()
    {
        if (!drawDebug) return;

        Vector3 pos = transform.position;

        // Cyan: line to the current target
        Debug.DrawLine(pos, targetPosition, Color.cyan);

        // Green: current velocity (where the agent is actually moving)
        Debug.DrawRay(pos, currentVelocity, Color.green);

        // Red: total steering velocity this frame (sum of all weighted behaviours)
        Debug.DrawRay(pos, steeringVelocity, Color.red);

        // Per-behaviour desired velocity, each in its own colour, drawn slightly raised
        // so they don't overlap the main lines
        Vector3 lifted = pos + Vector3.up * 0.1f;
        foreach (SteeringBehaviour b in steeringBehvaiours)
        {
            if (b.enabled) b.DrawDebug(this);
        }
    }
}