using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float sideThrust = 100f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] float levelLoadDelay = 2f;

    [Header ("SFX Audio Clip")]
    [SerializeField] AudioClip mainEngineSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip finishSFX;

    [Header("VFX Particles")]
    [SerializeField] ParticleSystem mainEngineVFX;
    [SerializeField] ParticleSystem deathVFX;
    [SerializeField] ParticleSystem finishVFX;

    AudioSource audioSource;
    Rigidbody rigidBody;

    bool isTransitioning = false;
    bool collisionsAreDisabled = false;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {   
        if(!isTransitioning)
        {
            ProcessInput();
        }
    }

    private void ProcessInput()
    {
        RespondToThrust();
        RespondToRotate();

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            //toggle collision
            collisionsAreDisabled = !collisionsAreDisabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isTransitioning || collisionsAreDisabled){ return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Deadly":
                StartDeathProcedure();
                break;
            case "Finish":
                StartFinishProcedure();
                break;
            default:
                Debug.Log("Safe");
                break;
        }
    }

    private void StartFinishProcedure()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(finishSFX);
        mainEngineVFX.Stop();
        finishVFX.Play();
        Invoke("LoadNextLevel", levelLoadDelay); //parameterise time
    }

    private void StartDeathProcedure()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSFX);
        mainEngineVFX.Stop();
        deathVFX.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)     //loop back level 1
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);      
    }

    private void RespondToThrust()
    {

        if (Input.GetKey(KeyCode.Space))    //can thrust with rotation
        {
            ApplyThrust();
        }
        else
        {
            StopApplyThrust();
        }
    }

    private void StopApplyThrust()
    {
        audioSource.Stop();
        mainEngineVFX.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        if (!audioSource.isPlaying)                 //avoid layering audio
        {
            audioSource.PlayOneShot(mainEngineSFX);
        }
        mainEngineVFX.Play();
    }

    private void RespondToRotate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(sideThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-sideThrust * Time.deltaTime);
        }
    }

    private void RotateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true;    //manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false;   //resume Physics
    }
}
