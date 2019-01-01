using UnityEngine;

[DisallowMultipleComponent]
public class PlatformOscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 7f;

    //todo remove from inspector
    [Range(0,1)][SerializeField] float movementFactor;       //0 not moved - 1 fully moved 

    Vector3 startingPos;

	void Start ()
    {
        startingPos = transform.position;
	}
	
	void Update ()
    {
        if (period == 0){ return; } // prevent NAN
        //set movement factor 

        //todo prevent divided by zero
        float cycles = Time.time / period;          //grows greater away from zero

        const float tau = Mathf.PI * 2f;
        float rawSineWave = Mathf.Sin(cycles * tau);    //range from -1 to 1

        movementFactor = rawSineWave / 2f + 0.5f;
        print(rawSineWave);

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
	}
}
