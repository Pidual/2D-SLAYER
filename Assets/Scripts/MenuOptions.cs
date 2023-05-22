using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class MenuOptions : MonoBehaviour
{

[SerializeField] private AudioMixer audio;
   public void FullScreen(bool option){
    Screen.fullScreen = option;
   }

   public void ChangeVol(float volume){
    audio.SetFloat("CaveMasterVol", volume);
   }

   public void setQuality(int index){
    QualitySettings.SetQualityLevel(index);
   }
}
