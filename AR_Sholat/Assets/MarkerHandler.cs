using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class MarkerHandler : MonoBehaviour, ITrackableEventHandler
{
    [SerializeField]
    private Text debugText;
    
    [SerializeField]    
    private AudioSource audioSource;
    [SerializeField]        
    private Animator animator;
    [SerializeField]
    private AudioClip[] audioClips;
    private int clipnumber = 0;
    private bool isFound;
    protected TrackableBehaviour trackableBehavior;


    private void Awake()
    {
        trackableBehavior = GetComponentInParent<TrackableBehaviour>();
        if (trackableBehavior) trackableBehavior.RegisterTrackableEventHandler(this);    
        
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {        
            OnTrackingFound();            
        }
        else
        {            
            OnTrackingLost();
        }
        
    }

    private void Update(){
        debugText.text =  isFound + " " + clipnumber + " yeah";
        if(!isFound) return;

        
        if (!audioSource.isPlaying) {
            audioSource.clip = audioClips[clipnumber];            
            audioSource.Play();
            clipnumber = (clipnumber + 1) % audioClips.Length;
        }
        
    }

    private void OnTrackingFound(){
        
        Vector3 scaling = transform.localScale/10;
        iTween.ScaleFrom(gameObject, iTween.Hash(
            "scale", scaling
        ));      
        
        
        isFound = true;
        animator.speed = 1;
        
    }

    private void OnTrackingLost(){
        isFound = false;
        audioSource.Pause();        
        animator.speed = 0;
    }

}
