using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BarGroupController : MonoBehaviour {

    public AudioSource Source;
    public List<BarGroupVisualizer> Visualizers = new List<BarGroupVisualizer>();

    [Header("Settings")]
    public bool PlayOnStart = false;
    
    public void Start() {
        if(PlayOnStart) {
            Source.Play();
        }
    }
}
