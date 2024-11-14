using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioStoneGate : MonoBehaviour
{
    [SerializeField] OpenStoneGate gate;

    public void StartSoundEffect()
    {
        gate.activationGateSound.Play();

    }
    public void StopSoundEffect()
    {
        gate.activationGateSound.Stop();

    }
}
