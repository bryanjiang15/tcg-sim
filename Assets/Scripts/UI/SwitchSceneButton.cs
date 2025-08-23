using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Switches to the specified scene by name
    /// </summary>
    /// <param name="sceneName">The name of the scene to switch to</param>
    public void SwitchToScene(string sceneName)
    {
        // Load the scene with the given name
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Switches to the scene by build index
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to switch to</param>
    public void SwitchToScene(int sceneIndex)
    {
        // Load the scene with the given build index
        SceneManager.LoadScene(sceneIndex);
    }
}
