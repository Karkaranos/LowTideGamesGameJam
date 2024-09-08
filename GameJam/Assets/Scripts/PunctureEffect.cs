using UnityEngine;

public class PunctureEffect : MonoBehaviour
{
    [SerializeField] private float timeBeforeDestroy;

    private float time;

    private void Start()
    {
        time = 0;
    }
    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if (time > timeBeforeDestroy)
            Destroy(gameObject);
    }
}
