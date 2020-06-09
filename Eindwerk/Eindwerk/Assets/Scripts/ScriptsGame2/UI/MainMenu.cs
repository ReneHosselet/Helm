using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator loadLevelAnimator;
    public Animator character;
    public float transitionTime = 3f;

    public void playGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
        StartCoroutine(StartGame());
        if (loadLevelAnimator == null)
        {
            loadLevelAnimator = null;
        }
        if (character == null)
        {
            character = null;
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Menu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private IEnumerator StartGame()
    {
        character.SetTrigger("Play");
        loadLevelAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
