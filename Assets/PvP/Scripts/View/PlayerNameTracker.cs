using Mirror;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerNameTracker : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetNameOnScreen))] public string playerName;
    [SerializeField] private TextMeshPro _displayNameOnScreen;
    [SerializeField] private NetManager _netManager;

    private void Awake()
    {
        _netManager = FindObjectOfType<NetManager>();
    }

    [Server]
    private void SetName(string newDisplayName)
    {
        playerName = _netManager.PlayerName;
        playerName = newDisplayName;
    }

    [Command]
    private void CmdSetName(string newDisplayName)
    {
        RPCSetName(newDisplayName);
    }

    [ClientRpc]
    private void RPCSetName(string newDisplayName)
    {
        SetName(newDisplayName);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdSetName(_netManager.PlayerName);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        CmdSetName(_netManager.PlayerName);
        SetNameOnScreen(playerName, _netManager.PlayerName);
    }

    private async void SetNameOnScreen(string oldName, string newName)
    {
        await Task.Delay(15);
        if (playerName != null)
        {
            _displayNameOnScreen.text = newName;
        }
    }
}
