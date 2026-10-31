![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

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

## Experience-scaled play-space boundary

MyVR must not ask a player to draw or trace an in-Experience safety boundary. The default Experience play-space envelope is generated automatically:

- The Experience declares a recommended clearance radius using an explicit, validated unit.
- MyVR centers and scales a spherical boundary around the active play/observer origin using that recommendation.
- The Experience owns the semantic recommendation; MyVR owns device-local origin, geometry generation, and presentation.
- Missing or invalid recommendations require a deterministic policy; they must not silently become an arbitrary radius or trigger a drawing workflow.

This sphere is an Experience-specific clearance envelope. It does **not** replace, disable, or claim authority over the headset platform's system Guardian or other physical safety boundary. A creator recommendation cannot prove that a real room is clear.

### Future scanned-area geometry

A later, optional custom mode may let a player prescan the available play area. MyVR can then provide captured geometry to an Experience that declares support for it, allowing the Experience to adapt world scale and layout to the scanned space rather than using the default sphere.

Scanned geometry is a separate capability and data contract, not a user-authored drawing tool and not a prerequisite for the default sphere. Its coordinate system, validity, freshness, privacy, and fallback behavior must be defined before implementation.

The canonical Experience contract belongs to the Experiences ecosystem. MyVR must not invent a parallel Experience manifest or put creator-authored clearance metadata in its device/session model. See [issue #17](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/17).

### Implementation boundary

The current Quest host has not yet established a swapchain, frame loop, spatial rendering, or live play-space tracking. Sphere visualization therefore remains planned, not implemented. Implement portable recommendation/geometry logic with deterministic tests first; add device presentation only after the frame pipeline and physical-device gates are proven.


---

<p align="center">
  <a href="https://github.com/TrentBest/TheSingularityWorkshop" aria-label="The Singularity Workshop">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="112" />
  </a><br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em>
</p>
