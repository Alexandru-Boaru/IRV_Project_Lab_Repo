using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{

    public float timeToSelfDestruct;
    public float timeToFade;
    public new Renderer renderer;
    public bool fade;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestructCounter());
        renderer = GetComponent<Renderer>();
    }

    IEnumerator SelfDestructCounter()
    {
        yield return new WaitForSeconds(timeToSelfDestruct);
        if (fade) {
            float t = timeToFade;
            while (t > 0)
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, t / timeToFade);
                t -= Time.deltaTime;
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}
