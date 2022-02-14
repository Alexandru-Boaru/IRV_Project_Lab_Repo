using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BulletHole : MonoBehaviour, IPooledObject
{
    public float timeToLive = 10f;
    public ParentConstraint pc;
    public int index = 0;
    public Color defaultColor;

    public void OnObjectSpawn(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        StopCoroutine("Live");
        StartCoroutine(Live(position, rotation, parent));
    }

    IEnumerator Live(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        ConstraintSource cs = new ConstraintSource();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Renderer parentRendered = parent.GetComponent<Renderer>();
        spriteRenderer.color = defaultColor;

        cs.sourceTransform = parent;
        cs.weight = 1;
        pc.constraintActive = false;
        pc.locked = false;
        pc.SetSource(0, cs);
        pc.translationAtRest = transform.position;
        pc.rotationAtRest = transform.rotation.eulerAngles;
        pc.SetTranslationOffset(index, Quaternion.Inverse(parent.rotation) * (transform.position - parent.position));
        pc.SetRotationOffset(index, (Quaternion.Inverse(parent.rotation) * transform.rotation).eulerAngles);
        pc.locked = true;
        pc.constraintActive = true;
;
        float t = timeToLive;
        while (t > 0)
        {
            t -= Time.deltaTime;
            if (parent == null || !parent.gameObject.activeInHierarchy)
            {
                break;
            }
            if (parentRendered.material.HasProperty("_Alpha"))
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, parentRendered.material.GetFloat("_Alpha"));
            yield return null;
        }
        gameObject.SetActive(false);
        pc.constraintActive = false;
        pc.locked = false;
    }
}
