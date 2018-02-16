using UnityEngine;
using UnityEngine.UI;

public class waveManager : MonoBehaviour
{

    Text score;

    string text = "Wave: ";

    void Start()
    {
        score = GetComponent<Text>();
    }

    void Update()
    {
        score.text = text + WorldState.Waves.ToString();
    }
}
