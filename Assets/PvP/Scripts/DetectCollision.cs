using System.Collections;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    [Header("Invulnerability time")]
    [SerializeField] private float _timeInvulnerability = 3f;

    private PlayerController _playerController;
    private HealthController _healthController;
    private bool _canBeHitted = true;  

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Player")
        {
            var render = hit.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            var controller = hit.gameObject.GetComponent<DetectCollision>();
            var health = hit.gameObject.GetComponent<HealthController>();

            StartCoroutine(Invisibility(render, controller));
        }       
    }

    IEnumerator Invisibility(SkinnedMeshRenderer render, DetectCollision controller)
    {
        for(int i = 0; i < render.materials.Length; i++)
        {
            render.materials[i].SetColor("_Color", Color.red);
        }
       
        _canBeHitted = false;
        controller._canBeHitted = false;
        yield return new WaitForSeconds(_timeInvulnerability);

        for (int i = 0; i < render.materials.Length; i++)
        {
            render.materials[i].SetColor("_Color", Color.white);
        }
        _canBeHitted = true;
        controller._canBeHitted = true;
    }
}
