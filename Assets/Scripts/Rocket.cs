using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    AudioSource thrustSound;
    Rigidbody rigidBody;

    bool isAudioPlaying;
    [SerializeField] float sideThrust = 100f;
    [SerializeField] float mainThrust = 50f;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        thrustSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        Thrust();
        Rotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("Friend");
                break;
            case "Deadly":
                Debug.Log("Dead");
                break;
            default:
                Debug.Log("Safe");
                break;
        }
    }

    private void Thrust()
    {

        if (Input.GetKey(KeyCode.Space))    //can thrust with rotation
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);

            if (!thrustSound.isPlaying)                 //avoid layering audio
            {
                thrustSound.Play();
            }
        }
        else
        {
            thrustSound.Stop();
        }
    }

    private void Rotate()
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
