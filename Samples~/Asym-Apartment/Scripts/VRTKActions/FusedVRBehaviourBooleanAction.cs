namespace FusedVR.VRStreaming.VRTK
{
    using UnityEngine;
    using Zinnia.Action;
    using static FusedVR.VRStreaming.ControllerInputManager;

    /// <summary>
    /// Listens for the linked boolean behavior and emits the appropriate action.
    /// </summary>
    public class FusedVRBehaviourBooleanAction : BooleanAction
    {
        [Tooltip("Manager to Listen for Input")]
        public ControllerInputManager inputManager;

        [Tooltip("Which hand should this behavior listen to") ]
        public Hand myHand;

        [Tooltip("Which button should this behavior listen to")]
        public Button myButton;

        private new void Start() {
            inputManager.VRButtonEvent.AddListener(OnRemoteUpdate);
        }

        private void OnDestroy() {
            inputManager.VRButtonEvent.RemoveListener(OnRemoteUpdate);
        }

        public void OnRemoteUpdate(Hand hand, Button button, bool pressed, bool touched) {
            if (hand == myHand && button == myButton) {
                Receive(pressed); // if my hand & button is pressed, then send data to VRTK
            }
        }
    }
}