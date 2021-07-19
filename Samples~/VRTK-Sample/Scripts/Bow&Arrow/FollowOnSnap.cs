using System.Collections;
using Tilia.Interactions.Interactables.Interactables;
using Tilia.Interactions.Interactables.Interactors;
using Tilia.Interactions.SnapZone;
using UnityEngine;

public class FollowOnSnap : MonoBehaviour
{
    public const float MAX_SPEED = 30f;

    public SnapZoneFacade facade;

    private InteractorFacade hand;
    private int snappedState = 0;
    private GameObject prevArrow;

    private Transform bowString;
    private Vector3 stringStartPos;
    private bool nockable = true;

    private InteractableFacade bow;

    void Start()
    {
        bow = gameObject.GetComponentInParent<InteractableFacade>();
        bowString = transform.parent;
        stringStartPos = transform.parent.localPosition;
    }

    public void OnEnterSnap(GameObject arrow)
    {
        if (nockable)
        {
            InteractableFacade interactable = arrow.GetComponent<InteractableFacade>(); 
            hand = interactable.GrabbingInteractors[0];

            facade.Snap(arrow); //snap arrow to the bow
            bow.Grab(hand); //second hand grabs bow and drops arrow, which has been snapped

            hand.Ungrabbed.AddListener(UnGrabbed); //listen for UnGrab 
            
            snappedState = 1;
            nockable = false;
        }
    }

    private void UnGrabbed(InteractableFacade bow)
    {
        facade.Unsnap(); //when ungrabbing bow, unsnap the arrow
    }

    public void OnRelease(GameObject arrow)
    {
        StartCoroutine(DisableNockTill(arrow, .15f));
    }

    private IEnumerator DisableNockTill(GameObject arrow,  float secs)
    {
        Vector3 position = arrow.transform.position;
        Vector3 forward = arrow.transform.forward; //save for before unlocked

        nockable = false;
        hand.Ungrabbed.RemoveListener(UnGrabbed);

        yield return null; //wait a frame to let unlock settle

        Rigidbody r = arrow.GetComponent<Rigidbody>();
        r.position = position;
        r.detectCollisions = false; //remove short term detections to avoid collisions with bow
        r.isKinematic = false;
        r.velocity = forward.normalized * GetPower() * MAX_SPEED;

        snappedState = 0;
        hand = null;

        arrow.GetComponent<Arrow>().SetDelayedFired(secs); //to avoid re-nocking

        yield return new WaitForSeconds(secs);
        r.detectCollisions = true; //to by pass the bow colldier

        nockable = true;
    }

    void Update()
    {
        switch (snappedState)
        {
            case 0:
                bowString.localPosition = stringStartPos;
                break;
            case 1:
                bowString.localPosition = stringStartPos + 
                    Mathf.Min ( (Vector3.Distance(hand.transform.position, bowString.parent.position) ) , .45f) * -Vector3.up;
                break;
        }
    }

    private float GetPower()
    {
        //TODO consider normalizing in the future
        return Vector3.Distance(bowString.localPosition, stringStartPos); //can convert to a 0-1 Range based on max
    }
}
