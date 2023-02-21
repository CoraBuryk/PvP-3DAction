using Mirror;
using System.Collections;
using UnityEngine;

public class DetectCollision : NetworkBehaviour
{
    [Header("Invulnerability time")]
    [SerializeField] private float _timeInvulnerability = 3f;
    
    private PlayerController _playerController;
    private SkinnedMeshRenderer _othePlayerRender;
    private DeathController _onDeathController;

    #region Sync Variables

    [SyncVar] private DetectCollision _otherPlayerCollisionDetect;
    [SyncVar] private HealthController _otherPlayerHealth;

    [SyncVar] private bool _canBeHitted = true;
    [SyncVar] private bool _isInvulnerable; 

    [SyncVar(hook = nameof(OnChangeColor))] private Color materilaColor;

    #endregion

    private void Awake()
    {
        _playerController = gameObject.GetComponent<PlayerController>();      
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (_playerController.IsDashing == true)
        {
            if (hit.gameObject.tag == "Player" && hit.gameObject.GetComponent<DetectCollision>()._canBeHitted == true)
            {
               if(isClient) CmdOnCollisionDetected(hit.gameObject);     
               if(isServer) RpcOnCollisionDetected(hit.gameObject);
               if(isLocalPlayer) LocalOnCollisionDetected(hit.gameObject);

                _otherPlayerHealth = hit.gameObject.GetComponent<HealthController>();

                if (_otherPlayerHealth.NumOfHeart > 0)
                {
                    CmdHealthChanged(_otherPlayerHealth);

                }
                if (_otherPlayerHealth.NumOfHeart <= 0)
                {
                    CmdOnDeath();
                }

            }
        }       
    }

    #region ToServer

    [Command]
    private void CmdOnCollisionDetected(GameObject otherPlayer)
    {
        LocalOnCollisionDetected(otherPlayer);
    }   

    [Command]
    private void CmdHealthChanged(HealthController health)
    {
        RpcHealthChange(health);
    }

    [Command]
    private void CmdOnDeath()
    {
        RpcOnDeath();
    }

    #endregion


    #region ToClient

    [ClientRpc]
    private void RpcOnCollisionDetected(GameObject player)
    {
        LocalOnCollisionDetected(player);
    }

    [ClientRpc]
    private void RpcHealthChange(HealthController health)
    {
        health.ChangingHealthValue(health.maxHealth - 1);
    }

    [ClientRpc]
    private void RpcOnDeath()
    {
        _onDeathController = FindObjectOfType<DeathController>();
        _onDeathController.DeathOneOfThePlayer();
    }

    #endregion

    private void LocalOnCollisionDetected(GameObject otherPlayer)
    {
        _othePlayerRender = otherPlayer.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        _otherPlayerCollisionDetect = otherPlayer.gameObject.GetComponent<DetectCollision>();
        var otherPlayerCharacterController = otherPlayer.gameObject.GetComponent<CharacterController>();

        StartCoroutine(Invisibility(_othePlayerRender, _otherPlayerCollisionDetect));

        while (_isInvulnerable == true)
        {
            otherPlayerCharacterController.detectCollisions = false;
        }
    }

    private void OnChangeColor(Color old, Color newCol)
    {
        materilaColor = newCol;
    }

    private IEnumerator Invisibility(SkinnedMeshRenderer render, DetectCollision collisonDetect)
    {
        ApplyColor(render, Color.red);

        collisonDetect._isInvulnerable = true;
        collisonDetect._canBeHitted = false;

        yield return new WaitForSeconds(_timeInvulnerability);

        ApplyColor(render, Color.white);

        collisonDetect._canBeHitted = true;
        collisonDetect._isInvulnerable = false;
    }
    
    private void ApplyColor(SkinnedMeshRenderer render, Color color)
    {
        for (int i = 0; i < render.materials.Length; i++)
        {
            OnChangeColor(render.materials[i].color, color);
            render.materials[i].SetColor("_Color", color);
        }
    }
}
