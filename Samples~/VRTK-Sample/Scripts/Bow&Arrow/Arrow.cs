using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zinnia.Tracking.Collision;

public class Arrow : MonoBehaviour
{
    private bool isFired = false;

    private Rigidbody myPhys;

    public void SetDelayedFired(float sec)
    {
        StartCoroutine(SetFired(sec));
    }

    IEnumerator SetFired(float sec)
    {
        yield return new WaitForSeconds(sec);
        isFired = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        myPhys = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFired)
        {
            transform.LookAt ( transform.position + myPhys.velocity  , Vector3.up);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (isFired)
        {
            Debug.LogError(collision.contacts[0].otherCollider.gameObject.name);
        }
    }
}
