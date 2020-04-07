using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class AnimHandler : MonoBehaviour, ITrackableEventHandler
{
    public Text debugText;
    
    [SerializeField] private AnimationClip[] animations;
    [SerializeField] private int[] frameStart, frameEnd;
    [SerializeField] private AudioClip[] audios;    
    [SerializeField] private int[] loopingAudio;

    private int curAnim = 0, numLoop;
    private bool isTracked = false, isFinished;
    private float curNextAnim;
    private Animator anim;
    private AudioSource audioSource;
    protected TrackableBehaviour trackableBehavior;
    void Start()
    {
        anim = transform.GetComponent<Animator>();    
        audioSource = transform.GetComponent<AudioSource>();
        trackableBehavior = GetComponentInParent<TrackableBehaviour>();
        if (trackableBehavior) trackableBehavior.RegisterTrackableEventHandler(this);                    
    }

    
    void Update()
    {
        if(!isTracked) return;
                
        // debugText.text = Time.time + " " + nextAnim;

        if (loopingAudio[curAnim] == 0){
            //AGAR DIA HANYA DI RUN SEKALI
            if (isFinished) return;            
            isFinished = true;
            
            anim.Play(animations[curAnim].name, 0, getNT(curAnim));            
            audioSource.clip = audios[curAnim];
            audioSource.Play();
            
            
            Invoke("PlayNextAnim",((float)frameEnd[curAnim]-frameStart[curAnim]) / 25);
        } else {
            //LOOPING SELAMA AUDIO JALAN X KALI
            if (!audioSource.isPlaying){
                if (numLoop == 0){
                    Invoke("PlayNextAnim",0);                    
                } else {
                    numLoop--;
                    audioSource.clip = audios[curAnim];
                    audioSource.Play();
                }
            } 
            // ANIMASI LOOPING SAMPAI AUDIO SELESAI
            if (curNextAnim < Time.time){                
                curNextAnim = Time.time + (((float)frameEnd[curAnim]-frameStart[curAnim]) / 25);
                anim.Play(animations[curAnim].name, 0, getNT(curAnim));
            }

        }
    }

    private void PlayNextAnim(){
        curAnim = (curAnim+1) % animations.Length;
        numLoop = loopingAudio[curAnim];
        curNextAnim = Time.time;
        isFinished = false;
        audioSource.Stop();
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
        curAnim = 0;
        isTracked = true;
        curNextAnim = Time.time;
    }

    private void OnTrackingLost(){
        isTracked = false;
        audioSource.Stop();
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
