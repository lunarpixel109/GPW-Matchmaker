using System;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{ 
    private SteeringAgent _steeringAgent;
    private PlayerPatience _playerPatience;
    private PatienceVisuals _patienceVisuals;
    
    private TMP_Text _warningText;
    
    public GameObject queueFloor;
    public Canvas canvas;
    
    private void OnEnable() {
        _steeringAgent = GetComponent<SteeringAgent>();
        _playerPatience = GetComponent<PlayerPatience>();
        _patienceVisuals = GetComponent<PatienceVisuals>();
        
        int mask = LayerMask.GetMask("Floor");
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100, mask)) {
            queueFloor = hit.transform.gameObject;
        }

        canvas = FindFirstObjectByType<Canvas>();
        
        _warningText = GetComponentInChildren<TMP_Text>();
        
        Initialize();
    }

    private void OnDisable() {
        
    }

    private void Initialize() {
        //_steeringAgent.queueFloor = queueFloor;

        _playerPatience.notificationCanvas = canvas;

        _patienceVisuals.patience = _playerPatience;
        _patienceVisuals.characterVisual = this.transform;
        _patienceVisuals.characterRenderer = this.GetComponent<Renderer>();
        _patienceVisuals.characterMaterial = this.GetComponent<Renderer>().material;
        _patienceVisuals.warningText = _warningText;
    }
}
