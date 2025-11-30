using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolerEngineSound : MonoBehaviour
{
    static public float dieselEngineVolume = 0;
    static public float electorEngineVolume = 0;

    public AudioSource dieselEngineAudioSource;
    public AudioSource electorEngineAudioSource;

    void Start()
    {
        dieselEngineVolume = 0;
        electorEngineVolume = 0;
    }

   
    void Update()
    {
        dieselEngineVolume = Mathf.Clamp01(dieselEngineVolume);
        electorEngineVolume = Mathf.Clamp01(electorEngineVolume);

        dieselEngineAudioSource.volume = dieselEngineVolume;
        electorEngineAudioSource.volume = electorEngineVolume;

        dieselEngineVolume -= 0.05f;
        electorEngineVolume -= 0.05f;
    }
}
