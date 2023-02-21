using System.Collections.Generic;
using UnityEngine;

public class HealthView : MonoBehaviour
{
    [SerializeField] private HealthController healthController;
    [SerializeField] private List<GameObject> _heartGameObject;

    private void OnEnable()
    {
        healthController.HealthChange += HandleHealthChange;
    }

    private void HandleHealthChange()
    {
        for (int i = 0; i < _heartGameObject.Count; i++)
        {
            if (i >= healthController.NumOfHeart)
            {
                _heartGameObject[i].SetActive(false);
            }
            else if(i < healthController.NumOfHeart)
            {
                _heartGameObject[i].SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        healthController.HealthChange -= HandleHealthChange;
    }
}
