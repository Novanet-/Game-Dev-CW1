using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioClip[] _backgroundMusicArray;
    // Use this for initialization
    void Start()
    {
        int randomMusicIndex = Random.Range(0, _backgroundMusicArray.Length);
        var audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = _backgroundMusicArray[randomMusicIndex];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
}