using UnityEngine;
using UnityEngine.InputSystem;

namespace FusedVR.VRStreaming {

    /// <summary>
    /// Fly Camera Writen by Windexglow 11-13-10. And Modified for usage with VR Render Streaming
    /// Original Source : https://gist.github.com/gunderson/d7f096bd07874f31671306318019d996
    /// </summary>
    public class CameraControls : MonoBehaviour {

        #region Public Properties
        public float mainSpeed = 10.0f;   // Regular speed
        public float shiftAdd = 25.0f;   // Amount to accelerate when shift is pressed
        public float maxShift = 100.0f;  // Maximum speed when holding shift
        public float camSens = 0.15f;   // Mouse sensitivity
        #endregion

        #region Private Properties
        private Mouse myMouse; // This Controllers Mouse Input
        private Touchscreen myTouch; // This Controllers Touch Input
        private Keyboard myKeyboard; // This Controllers Keyboard Input

        private Pointer lastPointer; // For Mouse / Touch Input, what pointer was used last
        private Vector2 lastPointerPosition = Vector2.zero; // The last position of the pointer
        private bool isFirstPress = true; // whether the pointer has been previously pressed in the prior frame
        private float totalRun = 1.0f; // speed control for the camera
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a Unity Input Device (Mouse, Touch, Keyboard) to this controller to be used for Camera Controls
        /// Used in the VR Input Manager to assign devices
        /// </summary>
        public void AddDevice(InputDevice device) {
            switch (device) {
                case Mouse mouse:
                    myMouse = mouse;
                    lastPointer = myMouse;
                    break;
                case Keyboard keyboard:
                    myKeyboard = keyboard;
                    break;
                case Touchscreen touch:
                    myTouch = touch;
                    lastPointer = myTouch;
                    break;
            }
        }

        /// <summary>
        /// Reset and Remove all Devices that have prior assigned
        /// Used in the VR Input Manager to remove devices for the camera control on disconnect
        /// </summary>
        public void RemoveDevices() {
            myMouse = null;
            myKeyboard = null;
            myTouch = null;
            lastPointer = null;
        }
        #endregion

        #region Camera Controls
        void Update() {
            // Mouse camera angle
            if (myMouse != null && myMouse.leftButton.isPressed) {
                if (isFirstPress) {
                    lastPointerPosition = myMouse.position.ReadValue();
                    isFirstPress = false;
                }

                UpdateMouseTouch(myMouse.position.ReadValue());
            } else if (myTouch != null && myTouch.primaryTouch.press.isPressed ) {
                if (isFirstPress) {
                    lastPointerPosition = myTouch.primaryTouch.position.ReadValue();
                    isFirstPress = false;
                }

                UpdateMouseTouch(myTouch.primaryTouch.position.ReadValue());
            } else {
                isFirstPress = true;
            }

            // Keyboard commands
            Vector3 p = GetBaseInput();

            if ( myKeyboard != null && myKeyboard.leftShiftKey.isPressed ) {
                totalRun += Time.deltaTime;
                p *= totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            } else {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p *= mainSpeed;
            }

            p *= Time.deltaTime;
            transform.Translate(p);
        }

        // Updates the Rotation based on the Mouse 2D Position
        void UpdateMouseTouch(Vector2 position) {
            Vector3 delta = position - lastPointerPosition;
            delta = new Vector3(-delta.y * camSens, delta.x * camSens, 0);
            delta = new Vector3(transform.eulerAngles.x + delta.x, transform.eulerAngles.y + delta.y, 0);
            transform.eulerAngles = delta;

            lastPointerPosition = position;
        }

        // Returns the basic values, if it's 0 than it's not active.
        private Vector3 GetBaseInput() {
            if (myKeyboard == null) {
                return Vector3.zero;
            }

            Vector3 p_Velocity = new Vector3();

            // Forwards
            if (myKeyboard.wKey.isPressed)
                p_Velocity += new Vector3(0, 0, 1);

            // Backwards
            if (myKeyboard.sKey.isPressed)
                p_Velocity += new Vector3(0, 0, -1);

            // Left
            if (myKeyboard.aKey.isPressed)
                p_Velocity += new Vector3(-1, 0, 0);

            // Right
            if (myKeyboard.dKey.isPressed)
                p_Velocity += new Vector3(1, 0, 0);

            // Up
            if (myKeyboard.spaceKey.isPressed)
                p_Velocity += new Vector3(0, 1, 0);

            // Down
            if (myKeyboard.leftCtrlKey.isPressed)
                p_Velocity += new Vector3(0, -1, 0);

            return p_Velocity;
        }
        #endregion

    }
}