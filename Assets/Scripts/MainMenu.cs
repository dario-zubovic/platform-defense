using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private bool loading;
    
    public void Update() {
        if(Input.GetButtonDown("Jump")) {
            LoadLevel();
        }
    }

    public void LoadLevel() {
        if(this.loading) {
            return;
        }

        SceneManager.LoadSceneAsync("Level", LoadSceneMode.Single);
        this.loading = true;
    }
}