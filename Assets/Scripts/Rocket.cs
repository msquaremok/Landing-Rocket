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

    enum State {Alive, Dying, Transcending};
    State state = State.Alive;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {   
        if(state == State.Alive)
        {
            ProcessInput();
        }
    }

    private void ProcessInput()
    {
        RespondToThrust();
        RespondToRotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive){ return; } //ignore collisions when "dead"

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("Friend");
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
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(finishSFX);
        mainEngineVFX.Stop();
        finishVFX.Play();
        Invoke("LoadNextLevel", levelLoadDelay); //parameterise time
    }

    private void StartDeathProcedure()
    {
        state = State.Dying;
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
        SceneManager.LoadScene(1);      //todo load more than 2 levels
    }

    private void RespondToThrust()
    {

        if (Input.GetKey(KeyCode.Space))    //can thrust with rotation
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineVFX.Stop();
        }
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
        rigidBody.freezeRotation = true;    //manual control of rotation
        float rotationThisFrame = sideThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))        
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;   //resume Physics
    }
}
