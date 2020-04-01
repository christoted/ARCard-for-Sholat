using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class AnimHandler : MonoBehaviour, ITrackableEventHandler
{
    public Text debugText;
    
    [SerializeField]
    private AnimationClip[] animations;
    [SerializeField]
    private int[] frameStart, frameEnd;

    private Animator anim;
    protected TrackableBehaviour trackableBehavior;
    void Start()
    {
        

        anim = transform.GetComponent<Animator>();    
        trackableBehavior = GetComponentInParent<TrackableBehaviour>();
        if (trackableBehavior) trackableBehavior.RegisterTrackableEventHandler(this);    
        debugText.text = getFrame(animations[0]) + "";
    }

    
    void Update()
    {
        
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
    private void OnTrackingFound(){
        // anim.Play(animations[0].name,0,0.5F);    
        PlayAnim();
    }

    private void OnTrackingLost(){

    }

    private void PlayAnim(){
        
        // for(int )
        anim.Play(animations[0].name, 0, getNT(0));
        Invoke("PlayAnim", (frameEnd[0]-frameStart[0]) / 25);
    }

    private int getFrame(AnimationClip ac){
        //25frame = 1 detik
        int front = (int)ac.length;
        float back =  ((int)(ac.length * 100))%100;
        int convert = (int)((back/100)*25);
        return front * 25 + convert;
    }

    private float getNT(int id){

        int totalFrame = getFrame(animations[id]);
        return (float)frameStart[id]/totalFrame;
    }
}
