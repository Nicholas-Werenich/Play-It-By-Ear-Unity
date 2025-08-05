using UnityEngine;
using UnityEngine.SceneManagement;

//Loading additive menu functions
public class PauseMenu : MonoBehaviour
{
    private bool menuLoaded = false;
    [SerializeField] private string addedScene;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (!menuLoaded)
                LoadMenu();
            else
                UnLoadMenu();
    }


    private void LoadMenu()
    {
        menuLoaded = true;
        Time.timeScale = 0f;
        SceneManager.LoadScene(addedScene, LoadSceneMode.Additive);
    }

    public void UnLoadMenu()
    {
        menuLoaded = false;

        if(!LevelComplete.levelCompleted)
            Time.timeScale = 1.0f;

        SceneManager.UnloadSceneAsync(addedScene);
    }
}