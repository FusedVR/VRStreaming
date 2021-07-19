using Tilia.Interactions.Interactables.Interactables;
using Tilia.Interactions.Interactables.Interactors;
using UnityEngine;

public class SpawnArrow : MonoBehaviour
{

    public InteractableFacade arrowPrefab;

    public void OnTriggerEnter(Collider other) {
        CreateArrow(other);
    }

    public void OnTriggerStay(Collider other) {
        CreateArrow(other);
        //this might be spwaning too many arrows...
    }

    void CreateArrow(Collider other) {
        InteractorFacade interactor = other.gameObject.GetComponentInParent<InteractorFacade>();

        if (interactor != null && interactor.GrabAction.IsActivated && interactor.GrabbedObjects.Count == 0) {
            InteractableFacade arrow = Instantiate(arrowPrefab);
            arrow.GrabAtEndOfFrame(interactor); //for some reason if we dont wait till end of frame, it sets KinmeaticWhenInactive
        }
    }
}
