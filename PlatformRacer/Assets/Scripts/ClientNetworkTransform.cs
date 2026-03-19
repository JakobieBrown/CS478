using UnityEngine;
using Unity.Netcode.Components;


public enum Authority
{
    Server,
    Client
}
[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    public Authority authorityMode = Authority.Client;
    
    protected override bool OnIsServerAuthoritative() => authorityMode == Authority.Server;
}
