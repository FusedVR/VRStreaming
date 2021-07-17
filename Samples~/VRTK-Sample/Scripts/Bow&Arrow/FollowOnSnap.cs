using System;
using System.Collections;
using System.Collections.Generic;
using Tilia.Interactions.Interactables.Interactables;
using Tilia.Interactions.Interactables.Interactors;
using Tilia.Interactions.SnapZone;
using UnityEngine;

public class FollowOnSnap : MonoBehaviour
{
    public SnapZoneFacade facade;

    private InteractorFacade hand;
    private int snappedState = 0;
    private GameObject prevArrow;

    private Transform bowString;
    private Vector3 stringStartPos;
    private bool nockable = true;

    private void Start()
    {
        bowString = transform.parent;
        stringStartPos = transform.parent.localPosition;
    }

    public void OnEnterSnap(GameObject arrow)
    {
        if (nockable)
        {
            InteractableFacade interactable = arrow.GetComponent<InteractableFacade>(); 
            hand = interactable.GrabbingInteractors[0];
            interactable.LastUngrabbed.AddListener(UnGrabbed);
            facade.Snap(arrow);

            nockable = false;
        }
    }

    private void UnGrabbed(InteractorFacade arg0)
    {
        facade.Unsnap(); 
    }

    public void OnSnap(GameObject arrow)
    {
        snappedState = 1;
    }

    public void OnRelease(GameObject arrow)
    {
        arrow.GetComponent<InteractableFacade>().LastUngrabbed.RemoveListener(UnGrabbed);
        hand = null;

        prevArrow = arrow;
        snappedState = 2;
        StartCoroutine(DisableNockTill(.1f));

    }

    private IEnumerator DisableNockTill(float secs)
    {
        nockable = false;
        prevArrow.GetComponent<Arrow>().SetDelayedFired(secs);

        yield return new WaitForSeconds(secs);

        nockable = true;
    }

    private void Update()
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
            case 2:

                facade.Unsnap(); // unsnapping again because the SnapZone caught the arrow after the first ungrab

                Rigidbody r = prevArrow.GetComponent<Rigidbody>(); //janky because after release kinimatic gets set for some reasdon
                r.isKinematic = false;
                r.velocity = prevArrow.transform.forward * GetPower() * 25f;

                snappedState = 0;
                break;
        }
    }

    private float GetPower()
    {
        return Vector3.Distance(bowString.localPosition, stringStartPos); //can convert to a 0-1 Range based on max
    }
}
