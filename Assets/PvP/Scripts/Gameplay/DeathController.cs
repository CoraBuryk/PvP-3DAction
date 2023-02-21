using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathController : MonoBehaviour
{
    [SerializeField] private NetManager _netManager;
    [SerializeField] private List<GameObject> _spawnPoints;
    [SerializeField] private TextMeshProUGUI _playerWinText;
    [SerializeField] private GameObject[] _players;
    [SerializeField] private int newHealth = 3;
   
    private List<GameObject> _removedSpawnPoints = new List<GameObject>();
    private string _playerName;

    public void DeathOneOfThePlayer()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].GetComponent<HealthController>().NumOfHeart > 0)
            {
                _playerName = _players[i].GetComponent<PlayerNameTracker>().playerName;
                _playerWinText.text = "Winner: " + _playerName;

                StartCoroutine(Respawn());
            }
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3.1f);

        HideGameObjectsUponDeath();

        yield return new WaitForSeconds(5f);

        RespawnAtRandomPoint();

        yield return new WaitForSeconds(0.1f);

        ShowGameObjectsUponRespawn();
    }

    private void HideGameObjectsUponDeath()
    {
        foreach (var player in _players)
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.GetComponent<HealthController>().ChangingHealthValue(newHealth);
            player.SetActive(false);
        }
    }

    private void ShowGameObjectsUponRespawn()
    {
        foreach (var player in _players)
        {
            player.GetComponent<CharacterController>().enabled = true;
            player.SetActive(true);
        }

        foreach(var removed in _removedSpawnPoints)
        {
            _spawnPoints.Add(removed);
        }
    }

    private void RespawnAtRandomPoint()
    {
        for(int i = 0; i < _players.Length; i++) 
        {
            int random = Random.Range(0, _spawnPoints.Count - 1);
            var spawnPoint = _spawnPoints[random];
            _removedSpawnPoints.Add(spawnPoint);
            _spawnPoints.RemoveAt(random);
           
            _players[i].transform.position = spawnPoint.transform.position;
            _players[i].transform.rotation = Quaternion.identity;
        }   
    }
}
