using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer mixer;
    
    public void SetLevel(float sliderVal)
    {
        //slidervalue to dB
        mixer.SetFloat("MasterVol",Mathf.Log10(sliderVal)* 20);
    }
}
  