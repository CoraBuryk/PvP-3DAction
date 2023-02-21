using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCameraText : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook;

    private void Awake()
    {
        cinemachineFreeLook = CinemachineFreeLook.FindObjectOfType<CinemachineFreeLook>();
    }

    private void Update()
    {
        transform.LookAt(cinemachineFreeLook.transform);
    }
}
