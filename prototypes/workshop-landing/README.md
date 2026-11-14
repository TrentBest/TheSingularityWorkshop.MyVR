# Workshop Landing Scene Prototype

This is an early, runnable visual prototype of the MyVR arrival sequence:

1. The canonical animated Workshop moniker is the threshold.
2. Entering reveals a spatial Workshop landing environment.
3. Little Lenny is presented as a destination within that environment.

The landing environment is intentionally reusable. It is not a Lenny-specific menu, and the Lenny destination is not a second story reader or a replacement for the Experience artifact.

## Run locally

Serve this directory over HTTP (ES modules and WebXR require a browser origin; WebXR immersive sessions generally require HTTPS or localhost):

```sh
cd prototypes/workshop-landing
python -m http.server 8080
```

Open `http://localhost:8080` in a recent browser. The prototype loads Three.js 0.170.0 from jsDelivr and the moniker SVG from the Renderer development branch, so those resources require network access.

On a compatible Quest browser, open the same page from an HTTPS host and use the browser's immersive VR control. This is a WebXR prototype, not a claim that Quest launch or controller interaction has been physically verified.

## Current scope

- Animated canonical moniker entrance, linked by source URL rather than copied into MyVR.
- Spatial landing environment with horizon, water, light architecture, and an illuminated destination marker.
- Little Lenny destination preview state, reachable from the sign or the on-screen control.
- Desktop fallback and reduced-motion-friendly HTML transition.
- WebXR renderer session entry when supported by the browser/device.
- Two WebXR controller ray pointers; pressing either controller's select/trigger event activates the Little Lenny destination, without relying on the desktop DOM controls.

## Deliberate limitations

- The Lenny view is a placeholder destination, not the authored beach scene or a full retelling.
- It does not yet resolve or validate the immutable Experience artifact identity at runtime.
- It does not yet consume StoryBuilder composition output, canonical page/source IDs, or approved visual assets.
- Hand tracking, locomotion, audio, haptics, comfort settings, and session restoration remain future work.
- Controller selection is implemented in code but has not been verified in a physical headset.
- The Three.js scene is an implementation experiment behind the MyVR manifestation boundary, not a decision that MyVR's architecture is Three.js-specific.

The next integration step is to replace the placeholder destination with a reader projection sourced from the existing Little Lenny Experience and its approved composition/assets. StoryBuilder remains the authoring authority; Experiences remains the Experience identity and lifecycle owner.
