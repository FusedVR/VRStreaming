using System.Collections;
using Tilia.Interactions.Interactables.Interactables;
using Tilia.Interactions.Interactables.Interactables.Grab.Action;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private bool isFired = false;
    private bool canAnimate = false;

    private Rigidbody myPhys;

    public void SetDelayedFired(float sec)
    {
        canAnimate = true;
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
        if (canAnimate)
        {
            transform.LookAt ( transform.position + myPhys.velocity  , Vector3.up);
        }
    }

    public void OnParticleCollision(GameObject other)
    {
        if (isFired)
        {
            canAnimate = false;
            isFired = false; //TODO: decide if this should be false here or not
            myPhys.velocity = Vector3.zero;
            myPhys.isKinematic = true; 
        }
    }
}
