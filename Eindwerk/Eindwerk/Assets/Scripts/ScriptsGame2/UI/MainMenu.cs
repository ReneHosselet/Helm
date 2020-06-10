using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator loadLevelAnimator;
    public Animator character;
    public Animator bGMusic;
    public float transitionTime = 3f;
    private AudioSource selected;

    private void Start()
    {
        selected = this.transform.Find("sounds/Selected").GetComponent<AudioSource>();
    }
    public void playGame()
    {
        playSound(selected);
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
        playSound(selected);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Menu()
    {
        playSound(selected);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
    public void QuitGame()
    {
        playSound(selected);
        Application.Quit();
    }
    private IEnumerator StartGame()
    {
        character.SetTrigger("Play");
        loadLevelAnimator.SetTrigger("Start");
        bGMusic.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void playSound(AudioSource aS)
    {
        aS.Stop();
        aS.Play();
    }
    public void Back()
    {
        playSound(selected);
    }
}
