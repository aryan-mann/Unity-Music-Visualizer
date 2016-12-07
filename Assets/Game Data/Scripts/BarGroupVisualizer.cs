using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BarGroupVisualizer : MonoBehaviour {

    private List<GameObject> Bars = new List<GameObject>();
    public AudioSource Source;
    [Header("Waveform Settings")]
    [Tooltip("[64,8192] - Power of Two")]
    public int SampleCount = 512;
    [Tooltip("Amplitude")]
    public int Scale = 1000;
    [Tooltip("Rate of Decrease of Amplitude")]
    public float Differential = 1f;

    [Header("Generation Settings")]
    [Tooltip("The representation of bars")]
    public GameObject ReferenceObject;
    [Tooltip("Ideally, should be divisible by 2")]
    public int ItemCount;
    [Tooltip("Minimum and maximum amplitude")]
    public Vector2 HeightClamp = new Vector2(1, 1000);
    [Tooltip("Distance between each bar")]
    public float BlockSpace = 1f;
    [Tooltip("Grows Downwards")]
    public bool Inverted = false;

    [Header("Time Triggers")]
    public List<DifferentialTrigger> DifferentialTriggers = new List<DifferentialTrigger>();

    private Vector3 RestPosition = new Vector3();

    
    /// <summary>
    /// Used to validate 'SampleCount' the user has entered
    /// </summary>
    private readonly List<int> AvailableSamples = new List<int>() {
        128, 256, 512, 1024, 2048, 4096, 8192
    };
    
    int SamplesPerItem;
    private float[] MaxHeights;

    void Start () {
        Source = (Source == null) ? GetComponent<AudioSource>() : Source;

        RestPosition = transform.position;

        if (!AvailableSamples.Contains(SampleCount)) {
            SampleCount = 512;
        }

        ReferenceObject.transform.parent = transform;
        ItemCount = (ItemCount == 0) ? 1 : ItemCount;
        for (int i = 0; i < ItemCount; i++) {
            GameObject obj = (GameObject) Instantiate(ReferenceObject, transform.position , Quaternion.identity, transform);
            obj.transform.position = transform.position + new Vector3(i * BlockSpace, 0, 0);
            obj.SetActive(true);
            Bars.Add(obj);
        }

        ItemCount = Bars.Count;
        SamplesPerItem = SampleCount / ItemCount;
        MaxHeights = new float[ItemCount];
        for(int i=0; i < MaxHeights.Length; i++){ MaxHeights[i] = 0; }
    }
	
	void Update () {
        ReactToAudio();
        CheckForDifferentialTriggers();
	}

    /*  The rate at which the bars go down is pretty important for visualizing a song
     *  If the rate is too low for a high energy song, it looks super awkward.
     *  Some bands like Radiohead and Led Zepplin have varying amounts of energy even in
     *  a single song, we have to use certain 'triggers' in time to apply a different rate
     *  of decrease to a single section within a song.
     */ 
    void CheckForDifferentialTriggers() {
        if(DifferentialTriggers.Count == 0) { return; }

        for(int i=(DifferentialTriggers.Count-1); i >=0; i--) {
            if(((int)Source.time) >= DifferentialTriggers[i].Seconds && Differential != DifferentialTriggers[i].Differential) {
                Differential = DifferentialTriggers[i].Differential;
                return;
            }
        }
    }

    /// <summary>
    /// Changes the bar's height according to the audio waveform.
    /// </summary>
    void ReactToAudio() {        
        
        for (int i = 0; i < ItemCount; i++) {
            float[] BufferLeftChannel = new float[SampleCount];
            float[] BufferRightChannel = new float[SampleCount];

            Source.GetOutputData(BufferLeftChannel, 0);
            Source.GetOutputData(BufferRightChannel, 1);

            float Average = 0;
            for (int j = 0; j < SamplesPerItem; j++) {
                Average += (BufferLeftChannel[i * SamplesPerItem + j] + BufferRightChannel[i * SamplesPerItem + j])/ 2;
            }
            
            Average = Mathf.Abs(Average / SamplesPerItem) * Scale;
            if (Average > MaxHeights[i]) {
                MaxHeights[i] = Average;
            }

            Bars[i].transform.localScale = new Vector3(1, (Inverted ? -1 : 1) * Mathf.Clamp(MaxHeights[i], HeightClamp.x, HeightClamp.y), 1);
            Bars[i].transform.position = new Vector3(Bars[i].transform.position.x, RestPosition.y + Bars[i].transform.localScale.y/2 ,Bars[i].transform.position.z);

            SetHeightDependantColor(Bars[i], Bars[i].transform.localScale.y, 10f);

            MaxHeights[i] = MaxHeights[i] - (Differential * Time.deltaTime);
        }

    }   

    void SetHeightDependantColor(GameObject obj, float currentHeight, float MaxHeight) {
        obj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.HSVToRGB(Mathf.Clamp((Mathf.Abs(currentHeight/MaxHeight)), 0f, 1f) ,0.8f, 0.8f));
    }
    
}

[Serializable]
public struct DifferentialTrigger {
    public int Seconds;
    [Range(0, 40)]
    public int Differential;
};