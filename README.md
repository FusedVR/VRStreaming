# Unity VR Render Streaming SDK

[![Release](https://img.shields.io/github/v/release/FusedVR/VRStreaming)](https://github.com/FusedVR/VRStreaming/releases) [![Discord](https://img.shields.io/discord/871764886563196948?color=6a0dad&label=discord&logo=discord)](https://discord.gg/rV8fEAmG5B) [![YouTube Channel Subscribers](https://img.shields.io/youtube/channel/subscribers/UCLO98KHpNx6JwsdnH04l9yQ?style=social)](https://www.youtube.com/FusedVR?sub_confirmation=1)

This SDK package is built based on [Unity Render Streaming](https://docs.unity3d.com/Packages/com.unity.renderstreaming@2.0/manual/index.html) and [Unity WebRTC](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html). By using this package, you can stream any Unity scene from the the Unity Editor or a Standalone build to a WebXR client on the FusedVR Website : [https://fusedvr.com/rendering](https://fusedvr.com/rendering). To try this this package, it is as simple as dragging the included **Render Streaming Services** prefab into your Unity scene that you would like to stream and setting up the connect to the WebRTC server. 

For an overview of this package, please refer to this video tutorial on the FusedVR Youtube Channel: [Streaming Your First WebXR Unity Game to Oculus Quest](https://youtu.be/di18sWRlbFs)

[![CloudXR WebXR Hello World](https://raw.githubusercontent.com/FusedVR/VRStreaming/master/Images~/hello-world.jpg)](https://youtu.be/di18sWRlbFs)

# Setup

1. Import this Github Repo as a Unity Package via the Unity Package Manager **Add from Git URL** : https://github.com/FusedVR/VRStreaming.git
2. Drag the provided **Render Streaming Service** prefab into your Unity scene
3. Verify the WebRTC server address on the **Render Streaming Service** Gameobject and Component is : [wss://webrtc-pcehljv7ea-uw.a.run.app](https://webrtc-pcehljv7ea-uw.a.run.app)
4. Start the scene (click Play in Editor or create a standalone Build). **NOTE** : The Game View must be selected otherwise streaming will not work as documented in issue [#12](https://github.com/FusedVR/VRStreaming/issues/12) 
5. Open https://fusedvr.com/rendering in a WebRTC compatible browser  with a WebXR compatible VR Headset connected (most standard browsers support this).
  a. Please verify that the URL is being served over an https connection!
  b. This page will connect to the WebRTC server served at [webrtc-pcehljv7ea-uw.a.run.app](https://webrtc-pcehljv7ea-uw.a.run.app) using the Google STUN Server (stun:stun.l.google.com:19302)
5. On the Browser, click "**Connect to Cloud VR Streaming Server**"
6. Once you see the video feed, click "**Enter Virtual Reality**"

# Camera Eye Resolution

At this time, the WebXR Client sends the resolution required to render per eye based on the connected VR device to Unity in order to adapt the resolution that is sent. However, IPD data is not sent and is hard-coded to a standard default IPD of 64mm. If you would like to adjust the resolution yourself, you may change the [VRInputManager](https://github.com/FusedVR/VRStreaming/blob/master/Runtime/Scripts/VRInputManager.cs) and change the code related to the VRDataType.Display.

# Cloud Support

Limited testing and Proof of Concepts have been done with deploying a standalone version of the Unity Render Streaming app onto a Cloud or Edge Service provider via either a Windows or Ubuntu Virtual Machine. More testing is required to be done to enable this support. 

# GPU Recommendations

It is strongly recommended to utlize a NVIDIA GPU as these GPUs support Hardware Accelerated Encoding, which is a requirement for lower latency VR Streaming. You can check the fully compatability matrix on the [Unity WebRTC documentation](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html). If you are using a GPU that does not support Hardware Accelerated, you will need to uncheck this option on the **Render Streaming Service** Gameobject and Component

# WebXR Input

Controller Input is captured via the [A-Frame Tracked-Controls component](https://aframe.io/docs/1.2.0/components/tracked-controls.html) and then sent over the data channel to the Unity SDK. This data protocol is adapted from the Unity Broadcast system, which was also capable of sending Keyboard, Mouse, Touch, and Gamepad data back to Unity via the RemoteInput.cs script in Unity. As such, VR data is sent from the client to Unity as Data Array buffers, defined in bytes. 

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

# Blockchain / Ethereum Web Wallet Integration

In order to support decentralized payments, the SDK now supports the ability to send and recieve information from any client with a web wallet that supports **window.ethereum** i.e. [Metamask](https://metamask.io/) or [Coinbase Wallet](https://wallet.coinbase.com/). The implementation for this is located with in the BlockchainData.cs script, which is a static class that allows you to call 3 key functions that will broadcast your message over the Data Channel to all connected clients :

- GetAccount() : returns the currently active Blockchain account
- Signature(string message) : returns the signed hash of the input message
- SendTransaction(string to, string value) : returns the transaction hash for the ethereum transaction that will send *value* ethereum to the *to* address from the active web wallet account

Once any of these methods has been called, the data will be sent over each Client's Data Channel to be processed on the client. If the Client supports the Web Wallet, then the message will be processed and returned back to the server. From Unity, you can listen from the return values on the BlockchainData.CryptoEvent event handler, which will return the event name and the result depending on which method was called. 

## How To Use

The most common scenario where this can be integrated is when a VR device like Oculus Quest **NOT** support Blockchain integration and would like to have a secondary device (i.e. smart phone) to be paired with the headset in order to authorize payments. A user could simply connect both the VR headset and phone to the same server and the headset would be responsible for displaying VR, while the phone / computer would be responsible for authorizing any payments using Metamask.

To implement this, you will need to :

- Increase the number of clients that can connect to the server from 1 to 2.
- If desired, create a specific CLientStreamer prefab dedicated for Blockchain support and assign that prefab to the **VRBroadcast**. If unchanged, the video stream will be passed through, which may or may not be desired. 

## More Blockchain Requests?

Please file [Github feature request issues](https://github.com/FusedVR/VRStreaming/issues) for additional methods and web wallet support that you would like to see implemented on clients. 

# Samples

Provided with the package are two samples to help with quick testing the SDK : **HDRP & VRTK**. Both these samples can be imported via the Package Manager once you have imported the SDK into your Unity project. 

The first one uses the Unity HDRP Built In Template to show case streaming a scene with the [High Definition Render Pipeline](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@11.0/manual/index.html) into WebXR. This one of the very few ways to play a HDRP game / app within Oculus Quest / WebXR. In order to utilize this sample, import the High Definition RP package from the Unity registry. 

![High Definition Render Pipeline](https://raw.githubusercontent.com/FusedVR/VRStreaming/master/Images~/hdrpvr.png)

The second provided sample shows integration with [VRTK](https://www.vrtk.io/) to showcase how to utilize the input that is streamed from the WebXR client into Unity. Using this input, we can build an Archery Sandbox. Simmply pick up the bow and then grab arrows from behind your back to begin shooting. Please note that to you use this sample you will need to manually also import the following [VRTK Tilia Packages](https://www.vrtk.io/tilia.html):

- **io.extendreality.tilia.camerarigs.trackedalias.unity**
- **io.extendreality.tilia.interactions.interactables.unity**
- **io.extendreality.tilia.interactions.snapzone.unity**

![VRTK Archery](https://raw.githubusercontent.com/FusedVR/VRStreaming/master/Images~/archery-sample.png)
