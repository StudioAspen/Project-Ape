using UnityEngine;

public class Shake : MonoBehaviour
{
    public float strength = 0.03f;
    public float speed = 20f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * speed) * strength;
        float y = Mathf.Cos(Time.time * speed * 1.3f) * strength;

        transform.localPosition = startPosition + new Vector3(x, y, 0);
    }
}