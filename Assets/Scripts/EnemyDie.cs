using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    public GameObject obj;
    MeshRenderer render;
    
    Transform textPosition;
    [SerializeField]
    GameObject add;
    [SerializeField]
    GameObject minus;

    bool hit = false;

    int points = 500;

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        textPosition = GameObject.FindGameObjectWithTag("textPosition").transform;
    }

    private void FixedUpdate()
    {
        if (!hit)
        {
            points -= 2;
        }
    }

    public void Die()
    {
        if (!hit)
        {
            hit = true;
            StartCoroutine(DissolveOut());
            StartCoroutine(ScoreAdd());
            GameObject obj = Instantiate(add, textPosition);
            obj.GetComponent<Text>().text = "+" + points.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "killzone" && !hit)
        {
            StartCoroutine(DissolveOut());
            StartCoroutine(ScoreMinus());
            Instantiate(minus, textPosition);
        }
    }

    IEnumerator DissolveOut()
    {
        for (float i = 0f; i <= 1f; i += 0.02f)
        {
            render.material.SetFloat("_SliceAmount", i);
            yield return new WaitForSeconds(0.0002f);
        }
    }

    IEnumerator ScoreAdd()
    {
        for (int i = 0; i < points; i++)
        {
            WorldState.Score += 1;
            yield return new WaitForSeconds(0.0001f);
        }
        Destroy(obj);
    }

    IEnumerator ScoreMinus()
    {
        for (int i = 0; i < 100; i++)
        {
            WorldState.Score -= 1;
            yield return new WaitForSeconds(0.0001f);
        }
        Destroy(obj);
    }
}
