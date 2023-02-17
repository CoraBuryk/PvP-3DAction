using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Death : MonoBehaviour
{
    [SerializeField] private Button _reloadButton;//test button

    private void OnEnable()
    {
        _reloadButton.onClick.AddListener(Reload);
    }

    private async void Reload()
    {
        await System.Threading.Tasks.Task.Delay(5000);
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        _reloadButton.onClick.RemoveListener(Reload);
    }

}
