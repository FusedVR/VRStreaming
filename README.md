# Unity VR Render Streaming SDK

Version : 0.1.0 beta

This SDK package is built based on [Unity Render Streaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@2.0/manual/index.html) and [Unity WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html). By using this package, you can stream any Unity scene from the the Unity Editor or a Standalone build to a WebXR client on the FusedVR Website : [https://fusedvr.com/rendering](https://fusedvr.com/rendering). To try this this package, it is as simple as dragging the included **Render Streaming Services** prefab into your Unity scene that you would like to stream and setting up the connect to the WebRTC server. 

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

At this time, the WebXR Client does not send VR Device infomration from WebXR to Unity. As such a default IPD of 64mm is used and a resolution of 1440x1600 per eye (resolution for original Oculus Quest). Until device info is implemented, if you like, you may increase the resolution per eye to match the VR device you are using. To do so, simply change the Streaming Size Resolution denoted on the Camera Streamer Component for both the Left and Right Eye.

# Cloud Support

Limited testing and Proof of Concepts have been done with deploying a standalone version of the Unity Render Streaming app onto a Cloud or Edge Service provider via either a Windows or Ubuntu Virtual Machine. More testing is required to be done to enabling this support. 

# GPU Recommendations

It is strongly recommended to utlize a NVIDIA GPU as these GPUs support Hardware Accelerated Encoding, which is a requirement for lower latency VR Streaming. You can check the fully compatability matrix on the [Unity WebRTC documentation](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html). If you are using a GPU that does not support Hardware Accelerated, you will need to uncheck this option on the **RenderStreaming** Gameobject and Component
