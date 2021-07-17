namespace FusedVR.VRStreaming.VRTK {
    using UnityEngine;
    using Zinnia.Action;
    using static FusedVR.VRStreaming.ControllerInputManager;

    /// <summary>
    /// Listens for the linked boolean behavior and emits the appropriate action.
    /// </summary>
    public class FusedVRBehaviourVector2Action : Vector2Action {
        [Tooltip("Which hand should this behavior listen to")]
        public Hand myHand;

        [Tooltip("Which button should this behavior listen to")]
        public Button myButton;

        public void OnRemoteUpdate(Hand hand, Button button, float x, float y) {
            if (hand == myHand && button == myButton) {
                Receive(new Vector2(x , y)); // if my hand & button is pressed, then send data to VRTK
            }
        }
    }
}