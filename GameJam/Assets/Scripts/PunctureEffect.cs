using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunctureEffect : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] private float timeToWaitBeforeFading;
    [SerializeField] private float decreaseSizeSpeed;
    [SerializeField] private float fadeSpeed;

    private float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;

        if (time >= timeToWaitBeforeFading)
        {
            if (spriteRenderer.color.a > 0.5)
            {
                //Fades color a second
                Color c = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a);
                spriteRenderer.color = new Color(c.r, c.g, c.b, c.a - fadeSpeed);
            }

            //Shrinks puncture
            transform.localScale -= new Vector3(decreaseSizeSpeed, decreaseSizeSpeed, 0);
        }

        if (transform.localScale.x <= 0 || transform.localScale.y <= 0)
        {
            Destroy(gameObject);
        }
    }
}
