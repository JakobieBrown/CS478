using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class SessionInitializer : MonoBehaviour
{
    //public static bool IsInitialized { get; private set; }
    //async void Awake()
    //{
    //    await InitializeAsync();
    //}

    //public static async Task InitializeAsync()
    //{
    //    if (IsInitialized) return;

    //    try
    //    {
    //        await UnityServices.InitializeAsync();

    //        if (!AuthenticationService.Instance.IsSignedIn)
    //        {
    //            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //        }

    //        IsInitialized = true;
    //        Debug.Log("Multiplayer Services Initialized!");
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"Initialization failed: {e.Message}");
    //    }
    //}
}
