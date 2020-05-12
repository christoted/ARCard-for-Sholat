using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class AnimHandler : MonoBehaviour, ITrackableEventHandler
{
    // public Text debugText;
    
    [SerializeField] private AnimationClip[] animations;
    [SerializeField] private int[] frameStart, frameEnd;
    [SerializeField] private AudioClip[] audios;    
    [SerializeField] private int[] loopingAudio;
    private Button buttonPlay, buttonPause;

    private int curAnim = 0, numLoop;
    private bool isTracked = false, isFinished, isPaused = false;
    private float NextAnimCD;
    private Animator anim;
    private AudioSource audioSource;
    protected TrackableBehaviour trackableBehavior;

    private string key_token = "TOKEN";

    [SerializeField] private GameObject btnPlay,btnPause;


    
    
    void Start()
    {
        anim = transform.GetComponent<Animator>();    
        audioSource = transform.GetComponent<AudioSource>();
        trackableBehavior = GetComponentInParent<TrackableBehaviour>();
        if (trackableBehavior) trackableBehavior.RegisterTrackableEventHandler(this);

    }   


   
    void Update()
    {
        if (!PlayerPrefs.HasKey(key_token)) {
            TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
            return;
        } else {
            TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
        }

        Debug.Log("masuk");

        if(!isTracked) return;
        if (isPaused) return;    
    
        if (loopingAudio[curAnim] == 0){
            //AGAR DIA HANYA DI RUN SEKALI            
            if (isFinished) {
                NextAnimCD -= Time.deltaTime;
                if (NextAnimCD < 0) PlayNextAnim();
                return;
            }
            isFinished = true;
            
            anim.Play(animations[curAnim].name, 0, getNT(curAnim));            
            audioSource.clip = audios[curAnim];
            audioSource.Play();

            NextAnimCD = ((float)frameEnd[curAnim]-frameStart[curAnim]) / 25;            
        } else {
            //LOOPING SELAMA AUDIO JALAN X KALI
            if (!audioSource.isPlaying){
                if (numLoop == 0){
                    PlayNextAnim();
                } else {
                    numLoop--;
                    audioSource.clip = audios[curAnim];
                    audioSource.Play();
                }
            } 
            // ANIMASI LOOPING SAMPAI AUDIO SELESAI
            NextAnimCD -= Time.deltaTime;
            if (NextAnimCD < 0){                                
                anim.Play(animations[curAnim].name, 0, getNT(curAnim));
                NextAnimCD = ((float)frameEnd[curAnim]-frameStart[curAnim]) / 25;
            }

        }
    }

    private void PlayNextAnim(){
        curAnim++;
        if (curAnim < animations.Length) numLoop = loopingAudio[curAnim];
        else Paused();
        NextAnimCD = 0;
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
            btnPlay.GetComponent<Button>().onClick.AddListener(Played);                             
            btnPause.GetComponent<Button>().onClick.AddListener(Paused);                                              
        }
        else
        {            
            OnTrackingLost();
        }
        
    }
    private void OnTrackingFound(){
        
        isTracked = true;
        isPaused = false;
        curAnim = 0;
        numLoop = loopingAudio[curAnim];
        
        anim.speed = 1;
        isFinished = false;    
        NextAnimCD = 0;        
        
        btnPause.SetActive(true);
    }

    private void OnTrackingLost(){
        isTracked = false;
        audioSource.Stop();
        
        btnPause.SetActive(false);
        btnPlay.SetActive(false);
    }



    private void Played(){
        if (!isPaused) return;
        isPaused = false;
        if (curAnim == animations.Length) {
            curAnim = 0;
            numLoop = loopingAudio[curAnim];
        }     
        if (audioSource.time > 0)  audioSource.Play();               
        anim.speed = 1;
        btnPlay.SetActive(false);
        btnPause.SetActive(true);        
    }

    private void Paused(){
        if (isPaused) return;
        isPaused = true;
        audioSource.Pause();        
        anim.speed = 0F;
        btnPlay.SetActive(true);
        btnPause.SetActive(false);        
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
