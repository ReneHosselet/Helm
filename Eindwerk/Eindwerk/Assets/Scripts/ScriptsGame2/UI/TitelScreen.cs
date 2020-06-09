using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitelScreen : MonoBehaviour
{
    public GameObject mainmenu;
    public GameObject titel;
    public Animator playerCharacter;
    public Animator camera;
    public float transTime = 2f;
    private bool isMainMenu = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMainMenu)
        {
            if (Input.anyKey)
            {
                StartCoroutine(FadeOut());
            }
        }        
    }

    private IEnumerator FadeOut()
    {
        titel.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(transTime);
        playerCharacter.SetTrigger("Point");
        camera.SetTrigger("MainMenu");
        mainmenu.SetActive(true);
        titel.SetActive(false);
        isMainMenu = true;
    }
}
