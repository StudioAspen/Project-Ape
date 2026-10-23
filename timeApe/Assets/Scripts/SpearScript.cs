using System.Collections;
using UnityEngine;

public class SpearScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody rb;
    private bool targetHit;
    
    


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Awake()
    {
        StartCoroutine(Waiter());
    }

    IEnumerator Waiter()
    {   
        yield return new WaitForSeconds(3);
        Object.Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit)
            return;
        else
            targetHit = true;

        rb.isKinematic = true;

        transform.SetParent(collision.transform);
      

    }
}

