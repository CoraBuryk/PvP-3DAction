using UnityEngine;
using Mirror;

public class NetManager : NetworkManager
{ 
    [SerializeField] private GameObject[] _spawnPoints;

    private bool _playerSpawned;
    private bool _playerConnected;

    public void OnCreateCharacter(NetworkConnectionToClient conn, PositionMessage message)
    {
        GameObject go = Instantiate(playerPrefab, message.vector3, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, go);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<PositionMessage>(OnCreateCharacter);
    }

    public void ActivePlayerSpawn()
    {
        int num = Random.Range(0, _spawnPoints.Length);

        Vector3 pos = _spawnPoints[num].transform.position;

        PositionMessage m = new PositionMessage() { vector3 = pos};
        NetworkClient.Send(m);
        _playerSpawned = true;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        _playerConnected = true;
        ActivePlayerSpawn();
    }
}

public struct PositionMessage : NetworkMessage
{
    public Vector3 vector3;
}