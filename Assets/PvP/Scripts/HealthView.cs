using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _heartMeshRenderer;
    [SerializeField] private HealthController _health;

    private void OnEnable()
    {
        _health.HealthChange += HeartsView;
    }

    private void OnDisable()
    {
        _health.HealthChange -= HeartsView;
    }

    private void Start()
    {
        HeartsView();
    }

    public void HeartsView()
    {
        for (int i = 0; i < _heartMeshRenderer.Length; i++)
        {
            if (i < _health.NumOfHeart)
            {
                _heartMeshRenderer[i].material.SetColor("_Color", Color.red);
            }
            else
            {
                _heartMeshRenderer[i].material.SetColor("_Color", Color.white);
            }
        }
    }
}
