using Cinemachine;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Utilities;
using System;
using StateMachine.Player;
using UnityEditor.Build.Content;

public struct InputPayload : INetworkSerializable
{
    public int tick;
    public Vector2 inputVector;
    public Vector2 position;
    public bool JumpTriggered;
    public bool JumpHeld;
    public DateTime timestamp;
    public ulong networkingObjectId;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref inputVector);
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref JumpTriggered);
        serializer.SerializeValue(ref JumpHeld);
        serializer.SerializeValue(ref timestamp);
        serializer.SerializeValue(ref networkingObjectId);
    }
}

public struct StatePayload : INetworkSerializable
{
    public int tick;
    public Vector2 position;
    public Vector3 velocity;
    public ulong networkObjectId;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref velocity);
        serializer.SerializeValue(ref networkObjectId);
    }
}

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    public InputReader input;
    [HideInInspector] public Rigidbody2D rb;
    public StatBlock StatBlock;
    public bool onGround;
    public float raycastDistance = 0.1f;
    public LayerMask groundLayer;
    public AudioClip jumpSound;
    public AudioClip finishSound;
    private AudioSource audioSource;
    public RaceTimer raceTimer;
    public ResultsManager resultsManager;

    private Action jumpSoundAction;

    ClientNetworkTransform clientNetworkTransform;
    private PlayerStateMachine stateMachine;

    [SerializeField] CinemachineVirtualCamera playerCamera;
    [SerializeField] AudioListener playerAudioListener;

    // netcode general
    NetworkTimer networkTimer;
    const float k_serverTickRate = 60f;
    const int k_bufferSize = 1024;

    // client
    CircularBuffer<StatePayload> clientStateBuffer;
    CircularBuffer<InputPayload> clientInputBuffer;
    StatePayload lastServerState;
    StatePayload LastProcessedState;

    // server
    CircularBuffer<StatePayload> serverStateBuffer;
    Queue<InputPayload> serverInputQueue;

    [SerializeField] float reconciliationThreshold = 10f;
    [SerializeField] float reconciliationCooldownTime = 1f;

    CountdownTimer reconciliationCooldown;

    [SerializeField] public float dragCoefficient = 0.5f;
    [SerializeField] private float raycastOffsetX;

    public Vector2 DragForce() => new Vector2(rb.linearVelocityX, 0) * -dragCoefficient;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        clientNetworkTransform = GetComponent<ClientNetworkTransform>();
        StatBlock = GetComponent<StatBlock>();
        input.Enable();

        stateMachine = new PlayerStateMachine(new PlayerStateFactory(this), this);

        networkTimer = new NetworkTimer(k_serverTickRate);
        clientStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
        clientInputBuffer = new CircularBuffer<InputPayload>(k_bufferSize);
        serverStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
        serverInputQueue = new Queue<InputPayload>();

        reconciliationCooldown = new CountdownTimer(reconciliationCooldownTime);
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn called, IsOwner: " + IsOwner);
        if (!IsOwner)
        {
            playerAudioListener.enabled = false;
            playerCamera.Priority = 0;
            return;
        }
        Debug.Log("Step 2: Past IsOwner check");
        playerAudioListener.enabled = true;
        Debug.Log("Step 3: Past playerAudioListener");
        playerCamera.Priority = 100;
        Debug.Log("Step 4: Past playerCameraPriority check");
        audioSource = GetComponentInChildren<AudioSource>();
        Debug.Log("Step 5: Past audioSource check");

        jumpSoundAction = () => { if (RaceCountdown.RaceStarted) PlayJumpSound(); };
        input.OnJumpPerformed += jumpSoundAction;
        Debug.Log("Step 6: Past playjumpsound");

        raceTimer = FindFirstObjectByType<RaceTimer>();
    }

    private void Update()
    {
        networkTimer.Update(Time.deltaTime);
        CheckGround();
        reconciliationCooldown.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        while (networkTimer.ShouldTick())
        {
            HandleClientTick();
            HandleServerTick();
        }
    }

    void HandleServerTick()
    {
        if (!IsServer) return;
        var bufferIndex = -1;
        InputPayload inputPayload = default;
        while (serverInputQueue.Count > 0)
        {
            inputPayload = serverInputQueue.Dequeue();
            bufferIndex = inputPayload.tick % k_bufferSize;

            StatePayload statePayload = ProcessMovement(inputPayload);
            serverStateBuffer.Add(statePayload, bufferIndex);
        }
        if (bufferIndex == -1) return;
        SendToClientRPC(serverStateBuffer.Get(bufferIndex));
    }

    [ClientRpc]
    void SendToClientRPC(StatePayload statePayload)
    {
        if (!IsOwner) return;
        lastServerState = statePayload;
    }

    private bool hasFinished = false;

    private void PlayFinishSound()
    {
        if (audioSource != null && finishSound != null)
        {
            audioSource.PlayOneShot(finishSound);
        }
    }

    void HandleClientTick()
    {
        if (!IsClient || !IsOwner) return;
        if (!RaceCountdown.RaceStarted) return;

        var currentTick = networkTimer.CurrentTick;
        var bufferIndex = currentTick % k_bufferSize;

        InputPayload inputPayload = new InputPayload()
        {
            tick = currentTick,
            inputVector = input.Move,
            JumpTriggered = input.jumpTriggered,
            JumpHeld = input.jumpHeld,
            position = transform.position,
            timestamp = DateTime.Now,
            networkingObjectId = NetworkObjectId
        };

        clientInputBuffer.Add(inputPayload, bufferIndex);
        SendToServerRPC(inputPayload);

        StatePayload statePayload = ProcessMovement(inputPayload);
        clientStateBuffer.Add(statePayload, bufferIndex);

        stateMachine.OnUpdate();
        stateMachine.OnFixedUpdate();

        if (transform.position.x >= PlayerPrefs.GetInt("SelectedDistance") && !hasFinished)
        {
            hasFinished = true;
            stateMachine.ChangeStates("PlayerStop");
            PlayFinishSound();
            raceTimer.StopTimer();

            resultsManager = FindFirstObjectByType<ResultsManager>();
            float finalTime = raceTimer.GetTime();
            resultsManager.ShowResults(finalTime);
        }

        HandleServerReconciliation();
    }

    bool ShouldReconcile()
    {
        bool isServerState = !lastServerState.Equals(default);
        bool isLastStateUndefinedOrDifferent = LastProcessedState.Equals(default)
                                                || !LastProcessedState.Equals(lastServerState);
        return isServerState && isLastStateUndefinedOrDifferent && !reconciliationCooldown.IsRunning;
    }

    void HandleServerReconciliation()
    {
        if (!ShouldReconcile()) return;

        float positionError;
        int bufferIndex;
        StatePayload rewindState = default;

        bufferIndex = lastServerState.tick % k_bufferSize;
        if (bufferIndex - 1 < 0) return;

        rewindState = IsHost ? serverStateBuffer.Get(bufferIndex - 1) : lastServerState;
        positionError = Vector2.Distance(rewindState.position, clientStateBuffer.Get(bufferIndex).position);

        if (positionError > reconciliationThreshold)
        {
            ReconcileState(rewindState);
            reconciliationCooldown.Start();
        }
        LastProcessedState = lastServerState;
    }

    void ReconcileState(StatePayload rewindState)
    {
        transform.position = rewindState.position;
        rb.linearVelocity = rewindState.velocity;

        if (!rewindState.Equals(lastServerState)) return;

        clientStateBuffer.Add(rewindState, rewindState.tick);

        int tickToReplay = lastServerState.tick;
        while (tickToReplay < networkTimer.CurrentTick)
        {
            int bufferIndex = tickToReplay % k_bufferSize;
            StatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
            clientStateBuffer.Add(statePayload, bufferIndex);
            tickToReplay++;
        }
    }

    [ServerRpc]
    void SendToServerRPC(InputPayload input)
    {
        serverInputQueue.Enqueue(input);
    }

    StatePayload ProcessMovement(InputPayload input)
    {
        Move(input.inputVector);
        Jump(input);

        return new StatePayload()
        {
            tick = input.tick,
            position = transform.position,
            velocity = rb.linearVelocity,
            networkObjectId = input.networkingObjectId,
        };
    }

    public void Move(Vector2 inputVector)
    {
        stateMachine.Move(inputVector);
    }

    private void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null && onGround)
        {
            Debug.Log("PlayJumpSound called");
            audioSource.PlayOneShot(jumpSound);
        }
    }

    public override void OnDestroy()
    {
        input.OnJumpPerformed -= jumpSoundAction;
        base.OnDestroy();
    }

    public void Jump(InputPayload input)
    {
        stateMachine.Jump(input);
    }

    void CheckGround()
    {
        Vector2 centerPos = transform.position;
        Vector2 leftPos = new Vector2(transform.position.x - raycastOffsetX, transform.position.y);
        Vector2 rightPos = new Vector2(transform.position.x + raycastOffsetX, transform.position.y);

        RaycastHit2D centerHit = Physics2D.Raycast(centerPos, Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(leftPos, Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightPos, Vector2.down, raycastDistance, groundLayer);

        onGround = centerHit.collider != null || leftHit.collider != null || rightHit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Vector2 centerPos = transform.position;
        Vector2 leftPos = new Vector2(transform.position.x - raycastOffsetX, transform.position.y);
        Vector2 rightPos = new Vector2(transform.position.x + raycastOffsetX, transform.position.y);
    }
}