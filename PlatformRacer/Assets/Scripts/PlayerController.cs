using Cinemachine;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Utilities;
using System;
using StateMachine.Player;

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
    private AudioSource audioSource;

    ClientNetworkTransform clientNetworkTransform;

    private PlayerStateMachine stateMachine;

    [SerializeField] CinemachineVirtualCamera playerCamera;
    [SerializeField] AudioListener playerAudioListener;

    // net code general 
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
    //[SerializeField] float extrapolationLimit = 0.5f;
    //[SerializeField] float extrapolationMultiplier = 1.2f;

    //StatePayload extrapolationState;
    //CountdownTimer extrapolationCooldown;

    CountdownTimer reconciliationCooldown;

    [SerializeField] public float dragCoefficient = 0.5f;
    [SerializeField] private float raycastOffsetX;

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
        //extrapolationCooldown = new CountdownTimer(extrapolationLimit);

        //reconciliationCooldown.OnTimerStart += () => {
        //    extrapolationCooldown.Stop();
        //};


        //extrapolationCooldown.OnTimerStart += () =>
        //{
        //    reconciliationCooldown.Stop();
        //    SwitchAuthorityMode(Authority.Server);
        //};

        //extrapolationCooldown.OnTimerStop += () =>
        //{
        //    extrapolationState = default;
        //    SwitchAuthorityMode(Authority.Client);
        //};
    }

    //void SwitchAuthorityMode(Authority authority)
    //{
    //    clientNetworkTransform.authorityMode = authority;
    //    bool shouldSync = authority == Authority.Client;
    //    clientNetworkTransform.SyncPositionX = shouldSync;
    //    clientNetworkTransform.SyncPositionY = shouldSync;
    //}
    public override void OnNetworkSpawn()
    {
        Debug.Log("IsOwner: " + IsOwner + " | Object: " + gameObject.name);
        if (!IsOwner)
        {
            playerAudioListener.enabled = false;
            playerCamera.Priority = 0;
            return;
        }
        playerAudioListener.enabled = true;
        playerCamera.Priority = 100;
        audioSource = GetComponentInChildren<AudioSource>();
        input.OnJumpPerformed += PlayJumpSound;
    }

    private void Update()
    {
        networkTimer.Update(Time.deltaTime);
        Debug.Log(stateMachine.current.ToString());
        CheckGround();
        stateMachine.OnUpdate();
        reconciliationCooldown.Tick(Time.deltaTime);
        //extrapolationCooldown.Tick(Time.deltaTime);
        //Extrapolate();

    }

    private void FixedUpdate()
    {
        while (networkTimer.ShouldTick())
        {
            HandleClientTick();
            HandleServerTick();
        }
        //Extrapolate();
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
        //HandleExtrapolation(serverStateBuffer.Get(bufferIndex), CalculateLatencyInMillis(inputPayload));
    }

    //void Extrapolate()
    //{
    //    if (IsServer && extrapolationCooldown.IsRunning)
    //    {
    //        transform.position += (Vector3)extrapolationState.position;
    //    }
    //}
    //void HandleExtrapolation(StatePayload latest, float latency)
    //{
    //    if (ShouldExtrapolate(latency))
    //    {
    //        if (extrapolationState.position != default)
    //        {
    //            latest = extrapolationState;
    //        }
    //        var posAdjustment = latest.velocity * (1 + latency * extrapolationMultiplier);
    //        extrapolationState.position = posAdjustment;
    //        extrapolationState.velocity = latest.velocity;
    //        extrapolationCooldown.Start();
    //    }
    //    else
    //    {
    //        extrapolationCooldown.Stop();
    //    }
    //}

    //private bool ShouldExtrapolate(float latency)
    //{
    //    return latency < extrapolationLimit && latency > Time.fixedDeltaTime;
    //}

    //static float CalculateLatencyInMillis(InputPayload inputPayload)
    //{
    //    return (DateTime.Now - inputPayload.timestamp).Milliseconds/1000f;
    //}

    [ClientRpc]
    void SendToClientRPC(StatePayload statePayload)
    {
        if (!IsOwner) return;
        lastServerState = statePayload;
    }

    void HandleClientTick()
    {
        if (!IsClient || !IsOwner) return;

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

        HandleServerReconciliation();
    }

    bool ShouldReconcile()
    {
        bool isServerState = !lastServerState.Equals(default);
        bool isLastStateUndefinedOrDifferent = LastProcessedState.Equals(default)
                                                || !LastProcessedState.Equals(lastServerState);
        return isServerState && isLastStateUndefinedOrDifferent && !reconciliationCooldown.IsRunning;// && !extrapolationCooldown.IsRunning;
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

        return new StatePayload() { 
            tick = input.tick,
            position = transform.position,
            velocity = rb.linearVelocity,
            networkObjectId = input.networkingObjectId,
        };
    }
    public void Move(Vector2 inputVector)
    {
        stateMachine.Move(inputVector);
        //int horizontalInput = inputVector.x > 0 ? 1 : inputVector.x < 0 ? -1 : 0; // 1 if x > 0, -1 if x < 0, else 0
        //rb.AddForce(new Vector2(horizontalInput * rb.mass * acceleration, 0));
        //Vector2 dragForce = -0.5f * dragCoefficient * rb.linearVelocity.magnitude * rb.linearVelocity;
        //rb.AddForce(dragForce);
    }
    private void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null && onGround) // So it doesn't spam the sound effect
            audioSource.PlayOneShot(jumpSound);
    }
    private void OnDestroy() // to unsubscribe from the event cleanly when the object is destroyed
    {                        // AI said this is needed - Asked Jakobi if it actually is needed
        input.OnJumpPerformed -= PlayJumpSound;
    }
    public void Jump(InputPayload input)
    {
        stateMachine.Jump(input);
        //if (input.JumpTriggered)
        //{
        //    rb.AddForce(new Vector2(0, 1) * JumpForce);
        //}
    }

    void CheckGround()
    {
        // Three raycast points: center, left, and right
        Vector2 centerPos = transform.position;
        Vector2 leftPos = new Vector2(transform.position.x - raycastOffsetX, transform.position.y);
        Vector2 rightPos = new Vector2(transform.position.x + raycastOffsetX, transform.position.y);

        // Cast rays downward from all three points
        RaycastHit2D centerHit = Physics2D.Raycast(centerPos, Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(leftPos, Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightPos, Vector2.down, raycastDistance, groundLayer);

        // onGround is true if any of the three rays hit something
        onGround = centerHit.collider != null || leftHit.collider != null || rightHit.collider != null;
        
    }

    private void OnDrawGizmos()
    {
        Vector2 centerPos = transform.position;
        Vector2 leftPos = new Vector2(transform.position.x - raycastOffsetX, transform.position.y);
        Vector2 rightPos = new Vector2(transform.position.x + raycastOffsetX, transform.position.y);
        Debug.DrawRay(centerPos, Vector2.down * raycastDistance, Color.red);
        Debug.DrawRay(leftPos, Vector2.down * raycastDistance, Color.red);
        Debug.DrawRay(rightPos, Vector2.down * raycastDistance, Color.red);
    }
}

