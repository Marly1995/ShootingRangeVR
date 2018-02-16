using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class clayShooter : MonoBehaviour
{
    public AudioSource shot;
    public AudioSource info;
    public AudioClip[] audioArray;

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

    public int gestureNumber = 100;

    List<int> numbers;

    private void Start()
    {
        shot = GetComponent<AudioSource>();
        numbers = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        WorldState.Waves = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) { started = true; }
        if (WorldState.Waves >= 10)
        {
            instructions.text = "Thanks For Playing!";
        }
        else if (started)
        {
            if (Time.time - timeTillNextShot >= lastTime &&
                !gestureTime)
            {
                int num = Random.Range(0, positions.Length);
                if (num == prevPosition) { num = (num == 0) ? num + 1 : num - 1; }

                prevPosition = num;
                GameObject temp = Instantiate(pidgeon, positions[num].position, Quaternion.identity);
                temp.GetComponent<Rigidbody>().AddForce(Vector3.up * Random.Range(minHeight, maxHeight), ForceMode.Impulse);
                temp.GetComponent<Rigidbody>().AddTorque(Vector3.up * Random.Range(-1f, 1f), ForceMode.Impulse);
                shot.Play();

                lastTime = Time.time;
                timeTillNextShot = Random.Range(minDelay, maxDelay);
                pidgeonsReleased++;
            }

            if (pidgeonsReleased >= 10)
            {
                gestureTime = true;
                int numPos = Random.Range(0, numbers.Count);
                gestureNumber = numbers[numPos];
                info.clip = audioArray[numbers[numPos]];
                info.Play(40000);
                numbers.RemoveAt(numPos);
                instructions.text = "Draw:  " + gestureNumber.ToString();
                pidgeonsReleased = 0;
            }
            else
            {
                if (OVRInput.GetDown(OVRInput.Button.One))
                {
                    rightTrail.transform.position = rightHand.transform.position;
                    leftTrail.transform.position = leftHand.transform.position;
                    rightTrail.transform.parent = rightHand.transform;
                    leftTrail.transform.parent = leftHand.transform;
                    gesturing = true;
                }
                if (OVRInput.GetUp(OVRInput.Button.One))
                {
                    rightTrail.transform.parent = null;
                    leftTrail.transform.parent = null;
                    gesturing = false;
                    WorldState.Waves++;
                    Invoke("StopGestureTime", 1.5f);
                }
            }
        }
    }

    void StopGestureTime()
    {
        gestureTime = false;
        instructions.text = "Shoot the targets!";
    }
}
