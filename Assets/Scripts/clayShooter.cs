using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class clayShooter : MonoBehaviour
{
    public AudioSource shot;
    public AudioSource info;
    public AudioClip wavecompleted;
    public AudioClip shotclip;

    [SerializeField]
    GameObject rightHand;
    [SerializeField]
    GameObject rightTrail;
    [SerializeField]
    GameObject leftHand;
    [SerializeField]
    GameObject leftTrail;

    [Space]
    [SerializeField]
    Text instructions;
    [SerializeField]
    GameObject pidgeon;
    [Space]
    [SerializeField]
    Transform[] positions;

    [Space]
    [SerializeField]
    float maxHeight;
    [SerializeField]
    float minHeight;
    [Space]
    [SerializeField]
    float maxDelay;
    [SerializeField]
    float minDelay;

    float timeTillNextShot;
    float lastTime;
    int prevPosition;
    int pidgeonsReleased;
    bool playGame = false;

    public bool started = false;

    bool gestureTime;
    public bool GestureTime
    {
        get { return gestureTime; }
        set { gestureTime = value; }
    }
    bool gesturing;
    public bool Gesturing
    {
        get { return gesturing; }
        set { gesturing = value; }
    }

    private void Start()
    {
        shot = GetComponent<AudioSource>();
        WorldState.Waves = 0;
        instructions.text = "Press a to begin";
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One)) { started = true; playGame = true; }
        if (WorldState.Waves >= 5)
        {
            instructions.text = "Thanks For Playing!";
        }
        else if (started)
        {
            if (Time.time - timeTillNextShot >= lastTime && playGame)
            {
                int num = Random.Range(0, positions.Length);
                if (num == prevPosition) { num = (num == 0) ? num + 1 : num - 1; }

                prevPosition = num;
                GameObject temp = Instantiate(pidgeon, positions[num].position, Quaternion.identity);
                temp.GetComponent<Rigidbody>().AddForce(Vector3.up * Random.Range(minHeight, maxHeight), ForceMode.Impulse);
                temp.GetComponent<Rigidbody>().AddTorque(Vector3.up * Random.Range(-1f, 1f), ForceMode.Impulse);
                info.clip = shotclip;
                shot.Play();

                lastTime = Time.time;
                timeTillNextShot = Random.Range(minDelay, maxDelay);
                pidgeonsReleased++;
                instructions.text = "Shoot the targets!";
            }

            if (pidgeonsReleased >= 10)
            {
                info.clip = wavecompleted;
                info.Play();
                WorldState.Waves++;
                instructions.text = "Wave " + WorldState.Waves.ToString();
                pidgeonsReleased = 0;
                playGame = false;
                Invoke("NextWave", 3.0f);
            }
        }
    }

    void NextWave()
    {
        playGame = true;
    }
}
