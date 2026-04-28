using Unity.Services.Authentication.Components;
using Unity.Services.Authentication;
using UnityEngine;
using WebSocketSharp;

public class Test : MonoBehaviour
{
    [SerializeField] PlayerAuthentication auth;

    void Awake()
    {
        auth.Events.SignedIn.AddListener(OnSignedIn);
    }

    void OnSignedIn()
    {
        // At this point:
        // AuthenticationService.Instance.PlayerName is READY

        string name = ProfileManager.Instance.Profile.playerName;
        if (!name.IsNullOrEmpty())
        {
            AuthenticationService.Instance.UpdatePlayerNameAsync(name);
        }
    }
}
