# VR Boundary

MyVR translates between Workshop semantics and VR-device capabilities.

```text
Experience semantics
        |
MyVR semantic adapter
        |
scene / spatial layout / interaction / audio / haptics
        |
VR runtime and device APIs
```

The lower boundary is platform-specific; the upper boundary remains platform-neutral.

## Observer

```text
Observer
 |-- position
 |-- orientation
 |-- viewport
 |-- visible extent
 |-- detail horizon
 `-- interaction horizon
```

MyVR maps this semantic observer onto headset tracking. Detail and interaction horizons remain semantic concepts rather than vendor-specific rendering APIs.

## Frame boundary

Frame-critical work includes tracking, view transforms, pose, immediate interaction, and local visual updates. Delegable work includes repository queries, artifact downloads, indexing, non-frame-critical computation, persistence, analytics, and content preparation.

Delegation must never make basic immersive presentation depend on a round trip to AnyApp.

## Runtime choices

WebXR is a possible manifestation route where a browser is available. MyVR is not defined as a WebXR application. The architecture leaves room for WebXR, native clients, device-specific adapters, and future OpenXR implementations.