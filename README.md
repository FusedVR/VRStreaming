# Unity VR Render Streaming SDK

[![Release](https://img.shields.io/github/v/release/FusedVR/VRStreaming)](https://github.com/FusedVR/VRStreaming/releases)

This SDK package is built based on [Unity Render Streaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@2.0/manual/index.html) and [Unity WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html). By using this package, you can stream any Unity scene from the the Unity Editor or a Standalone build to a WebXR client on the FusedVR Website : [https://fusedvr.com/rendering](https://fusedvr.com/rendering). To try this this package, it is as simple as dragging the included **Render Streaming Services** prefab into your Unity scene that you would like to stream and setting up the connect to the WebRTC server. 

For an overview of this package, please refer to this video tutorial on the FusedVR Youtube Channel: [A First Look At CloudXR WebXR](https://youtu.be/cTd8VDqep1c)

[![CloudXR WebXR](https://raw.githubusercontent.com/FusedVR/VRStreaming/master/Images~/first-look.jpg)](https://youtu.be/cTd8VDqep1c)

# Setup

1. Import this Github Repo as a Unity Package via the Unity Package Manager **Add from Git URL** : https://github.com/FusedVR/VRStreaming.git
2. Drag the provided **Render Streaming Service** prefab into your Unity scene
3. Verify the WebRTC server address on the **RenderStreaming** Gameobject and Component is : wss://webrtc-pcehljv7ea-uw.a.run.app
4. Start the scene (click Play in Editor or create a standalone Build)
5. Open https://fusedvr.com/rendering in a WebRTC compatible browser (most standard browesers support this) with a WebXR compatible VR Headset connected.
  a. Please verify that the URL is being served over an https connection!
  b. This page will connect to the WebRTC server served at webrtc-pcehljv7ea-uw.a.run.app using the Google STUN Server (stun:stun.l.google.com:19302)
5. On the Browser, click "**Connect to Cloud VR Streaming Server**"
6. Once you see the video feed, click "**Enter Virtual Reality**"

# Camera Eye Resolution

At this time, the WebXR Client sends the resolution required to render per eye based on the connected VR device to Unity in order to adapt the resolution that is sent. However, IPD data is not sent and is hard-coded to a standard default IPD of 64mm. If you would like to adjust the resolution yourself, you may change the [VRInputManager](https://github.com/FusedVR/VRStreaming/blob/master/Runtime/Scripts/VRInputManager.cs) and change the code related to the VRDataType.Display.

# Cloud Support

Limited testing and Proof of Concepts have been done with deploying a standalone version of the Unity Render Streaming app onto a Cloud or Edge Service provider via either a Windows or Ubuntu Virtual Machine. More testing is required to be done to enabling this support. 

# GPU Recommendations

It is strongly recommended to utlize a NVIDIA GPU as these GPUs support Hardware Accelerated Encoding, which is a requirement for lower latency VR Streaming. You can check the fully compatability matrix on the [Unity WebRTC documentation](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html). If you are using a GPU that does not support Hardware Accelerated, you will need to uncheck this option on the **RenderStreaming** Gameobject and Component

# WebXR Input

Controller Input is captured via the [A-Frame Tracked-Controls component](https://aframe.io/docs/1.2.0/components/tracked-controls.html) and then sent over the data channel to the Unity SDK. This data protocol is adaopted from the Unity Broadcast system, which was also capable of sending Keyboard, Mouse, Touch, and Gamepad data back to Unity via the RemoteInput.cs script in Unity. As such, VR data is sent from the client to Unity as Data Array buffers, defined in bytes. 

The first byte of the data Array Byffer refers to the input mode to determine how to parse the data as an id. The following IDs were reservered for Web Input by the Unity Render Streaming system. 

- ID 0 = Keyboard
- ID 1 = Mouse
- ID 2 = MouseWheel
- ID 3 = Touch
- ID 4 = UI (legacy)
- ID 5 = Gamepad

ID 6 is what is used for all WebXR Input specific to VR. Within VR Input, we specify 3 different modes for sending data, which are:
- ID 0 = Positional and Rotational Data of the Headset and Hands
- ID 1 = Controller Button Data ( A , B , Trigger, Grip )
- ID 2 = Controller Axis Data (Joystick & Trackpad)

The Raw Data from the Client is passed to VRInputManager, who is responsible for transmitting events based on the data mode recieved. Controller Input is then parsed by the ControllerInputManager, which has events that can be subscribed to for VR Input. 

# Samples

Provided with the package are two samples to help with quick testing the SDK : **HDRP & VRTK**. Both these samples can be imported via the Package Manager once you have imported the SDK into your Unity project. 

The first one uses the Unity HDRP Built In Template to show case streaming a scene with the [High Definition Render Pipeline](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@11.0/manual/index.html) into WebXR. This one of the very few ways to play a HDRP game / app within Oculus Quest / WebXR. In order to utilize this sample, import the High Definition RP package from the Unity registry. 

![High Definition Render Pipeline](https://raw.githubusercontent.com/FusedVR/VRStreaming/master/Images~/hdrpvr.png)

The second provided sample shows integration with [VRTK](https://www.vrtk.io/) to showcase how to utilize the input that is streamed from the WebXR client into Unity. Using this input, we can build an Archery Sandbox. Simmply pick up the bow and then grab arrows from behind your back to begin shooting. Please note that to you use this sample you will need to manually also import the following [VRTK Tilia Packages](https://www.vrtk.io/tilia.html):

- **io.extendreality.tilia.camerarigs.trackedalias.unity**
- **io.extendreality.tilia.interactions.interactables.unity**
- **io.extendreality.tilia.interactions.snapzone.unity**

![VRTK Archery](https://raw.githubusercontent.com/FusedVR/VRStreaming/master/Images~/archery-sample.png)
