using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{

    public float timeToSelfDestruct;
    public float timeToFade;
    public Renderer[] renderers;
    public bool fade;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestructCounter());
        renderers = GetComponentsInChildren<Renderer>();
    }

    IEnumerator SelfDestructCounter()
    {
        yield return new WaitForSeconds(timeToSelfDestruct);
        if (fade) {
            float t = timeToFade;
            while (t > 0)
            {
                foreach(Renderer r in renderers)
                {
                    if (r is SpriteRenderer)
                        ((SpriteRenderer)r).color = new Color(((SpriteRenderer)r).color.r, ((SpriteRenderer)r).color.g, ((SpriteRenderer)r).color.b, t / timeToFade);
                    else
                        r.material.SetFloat("_Alpha", t / timeToFade);
                //r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, t / timeToFade);
                }
                //renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, t / timeToFade);
                t -= Time.deltaTime;
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}
