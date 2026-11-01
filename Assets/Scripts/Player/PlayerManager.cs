using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private SteeringAgent _steeringAgent;
    private PlayerPatience _playerPatience;
    private PatienceVisuals _patienceVisuals;
    private RoomItem _roomItem;
    private TMP_Text _warningText;
    private int _floorMask;

    public GameObject queueFloor;
    public Canvas canvas;

    private void Awake() {
        _steeringAgent = GetComponent<SteeringAgent>();
        _playerPatience = GetComponent<PlayerPatience>();
        _patienceVisuals = GetComponent<PatienceVisuals>();
        _warningText = GetComponentInChildren<TMP_Text>();
        _floorMask = LayerMask.GetMask("Floor");

        _steeringAgent.enabled = false; // inert until placed
    }

    private void OnEnable() {
        // Visuals don't depend on the floor, so set them up straight away.
        canvas = FindFirstObjectByType<Canvas>();
        Initialize();

        // RoomItem may be added by DragManager after Awake, so look it up here.
        _roomItem = GetComponent<RoomItem>();
        if (_roomItem == null) return;

        _roomItem.Settled += HandleSettled;
        _roomItem.PickedUp += Unplace;
    }

    private void OnDisable() {
        if (_roomItem == null) return;

        _roomItem.Settled -= HandleSettled;
        _roomItem.PickedUp -= Unplace;
    }
    
    [Tooltip("Tick for players placed in the scene (or spawned by a spawner) that should start walking immediately. Leave off for drag-spawned prefabs.")]
    public bool autoPlaceOnStart;

    private void Start() {
        if (!autoPlaceOnStart) return;

        if (!TryPlace())
            Debug.LogWarning($"{name}: autoPlaceOnStart but no floor found below", this);
    }

    private void HandleSettled() {
        //Debug.Log($"{name}: Settled raised", this);
        if (!TryPlace())
            Debug.LogWarning($"{name}: no floor found under final drop position", this);
    }

    /// Finds the floor below the item's resting position and starts the agent.
    public bool TryPlace() {
        Vector3 origin = transform.position + Vector3.up;
        if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 100f, _floorMask)) {
            return false;
        }

        queueFloor = hit.transform.gameObject;
        _steeringAgent.queueFloor = queueFloor;
        _steeringAgent.enabled = true;
        return true;
    }

    /// Stops the agent while the item is being dragged.
    public void Unplace() {
        _steeringAgent.enabled = false;
    }

    private void Initialize() {
        var rend = GetComponent<Renderer>();

        _playerPatience.notificationCanvas = canvas;
        _patienceVisuals.patience = _playerPatience;
        _patienceVisuals.characterVisual = transform;
        _patienceVisuals.characterRenderer = rend;
        _patienceVisuals.characterMaterial = rend.material;
        _patienceVisuals.warningText = _warningText;
    }
}