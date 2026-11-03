<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# 07 — The MyVR Domain: Responsibilities, Evidence, and Gates

## 01 What — a VR host is a collection of distinct responsibilities

A VR application is not one undifferentiated "renderer." It coordinates a device runtime, an application lifecycle, an immersive session, spatial observations, input, presentation, and optional services around an Experience.

MyVR owns the VR manifestation and the device-facing responsibilities. It does not own the portable meaning of an Experience, artifact discovery/composition, shared execution orchestration, or a second general-purpose physics/rendering engine.

This chapter is an implementation map, not a claim that every listed subsystem exists. Its purpose is to keep the domain complete in our reasoning while keeping implementation claims tied to source and evidence.

## 02 Why — separate ownership prevents a second platform from forming

Without explicit boundaries, a VR host can gradually absorb unrelated responsibilities: loading artifacts, resolving dependencies, scheduling work, defining domain policy, authoring Experience semantics, or duplicating physics and rendering primitives.

That creates a second platform whose behavior drifts from the rest of the Workshop. The intended direction is:

```text
Experience manifest + configuration
              |
              v
            FSM_COS
      composition / RuntimeAssembly
              |
              v
          FSM_Foundry
    shared execution lifecycle
              |
              v
             MyVR
   VR host / device / presentation
```

FSM_Foundry is the shared host kernel. It must remain unaware of AnyApp, WebApp, ServerApp, and MyVR. Those are peer hosts. MyVR supplies the VR-specific adapter and manifestation; it does not fork shared loading or execution lifecycle.

The flow above is the target architecture. The typed handoff and end-to-end integration are not yet source-verified in MyVR.

## 03 How — responsibility and evidence map

| Domain responsibility | Current evidence | MyVR ownership / next boundary |
|---|---|---|
| Experience artifact identity | Source exists in `src/MyVR.Core/ExperienceIdentity.cs`: immutable BundleId, Version, ContentHash value | Consume the canonical published artifact identity; do not redefine publication or manifest semantics |
| MyVR session identity and lifecycle | Source exists in `MyVrSession.cs` and `ImmersiveSessionState.cs`; deterministic core tests cover transitions | Own the device manifestation session, separate from artifact identity |
| Device capability model | Source exists in `DeviceCapabilities.cs`; adapter-owned capability names are normalized into immutable snapshots | Report actual runtime capabilities; do not claim real headset discovery from the fake adapter |
| Runtime adapter boundary | `IDeviceRuntimeAdapter.cs` exposes asynchronous capability detection | Keep Android/OpenXR/vendor calls behind MyVR-owned adapters |
| Android / Quest application lifecycle | Candidate implementation in `src/MyVR.Quest2.Android/MainActivity.cs` | Own Activity integration, loader setup, lifecycle coordination, and teardown |
| OpenXR and Vulkan session bootstrap | Candidate host contains loader initialization, extension checks, HMD selection, runtime-selected Vulkan setup, and OpenXR session creation | Physical Quest 2 execution is still required to validate these paths |
| Frame loop, swapchain, frame submission | Not implemented in the current host candidate | Next device-runtime phase, gated on successful physical bootstrap |
| Pose, tracking validity, controller/hand input | Not implemented in the current host candidate | Add runtime-specific sampling and translate only required observations into bounded MyVR semantics |
| Spatial observer, coordinate frames, play-space boundary | Architectural work is tracked separately; no live spatial frame pipeline is present | Define coordinate ownership and reference-frame conversions before spatial features depend on them |
| Spatial presentation and interaction affordances | No live VR presentation pipeline is present | MyVR adapts the shared presentation capabilities; reuse Visualize/Physics rather than creating parallel domains |
| Haptics and spatial audio | Not implemented | Add only against demonstrated device capabilities and Experience requirements |
| Experience capability negotiation and semantic events | Portable capability snapshots exist, but no canonical manifest negotiation or live semantic event path is integrated | Coordinate with Experiences and FSM_UserIO; recognition is not authorization |
| Companion connectivity and offline behavior | Architectural constraints exist; no complete bounded companion protocol is integrated | Standalone MyVR must remain useful without AnyApp, WebApp, or network; delegated work must be explicit and non-frame-critical |
| Shared composition and execution | Described as target architecture; no MyVR FSM_COS → FSM_Foundry integration test exists | Consume the typed RuntimeAssembly handoff; never implement a second composer or scheduler |

### Evidence vocabulary

Use these terms consistently:

- **Source-implemented** — the behavior exists in the identified source.
- **Deterministically tested** — automated tests exercise the behavior without requiring a headset.
- **CI-verified** — a named workflow passed for an exact commit. State the commit and workflow.
- **Package-verified** — the expected deployable artifact was produced and inspected.
- **Physically verified** — the behavior was observed on the named headset/runtime, with logs and version details preserved.
- **Conceptual** — architecture or theory only; no integrated implementation is claimed.
- **Not implemented** — no current implementation exists in the candidate being described.

These are separate dimensions, not a ladder where one automatically implies the next. A CI-built APK is not proof of a running OpenXR session. A simulated capability snapshot is not proof of headset capability. A passing core test is not proof of frame timing or input on device.

## 04 See it in a minute — trace the existing portable core

The current portable core can be inspected without a headset:

- `src/MyVR.Core/ExperienceIdentity.cs` — artifact identity.
- `src/MyVR.Core/MyVrSession.cs` — session identity and lifecycle transitions.
- `src/MyVR.Core/ImmersiveSessionState.cs` — lifecycle vocabulary.
- `src/MyVR.Core/DeviceCapabilities.cs` — immutable runtime capability snapshot.
- `src/MyVR.Core/IDeviceRuntimeAdapter.cs` — device/runtime boundary.
- `tests/MyVR.Core.Tests/` — deterministic tests for the portable contract.

The Android candidate is separate:

- `src/MyVR.Quest2.Android/MainActivity.cs` — Android/OpenXR/Vulkan host bootstrap and lifecycle integration.
- `src/MyVR.Quest2.Android/MyVR.Quest2.Android.csproj` — Android application target and dependencies.
- `docs/QUEST2_ANDROID_HOST_BOOTSTRAP.md` — deployment and first-device verification procedure.

The core and host source can be reviewed independently. Do not treat their existence as proof that the physical device has executed the complete path.

## 05 The physical-device gate — stop at the right boundary

Before implementing swapchains, frame submission, tracking, or controller input, complete the narrow Quest 2 bootstrap check:

1. Record the exact headset model and Meta OS/runtime version.
2. Install and launch the candidate APK on the physical headset.
3. Preserve the complete MyVR host log.
4. Verify Android OpenXR loader initialization and required extension enumeration.
5. Verify OpenXR instance creation and HMD system selection.
6. Verify runtime-selected Vulkan instance/device creation.
7. Verify OpenXR session creation.
8. Exercise Activity pause/resume and the OpenXR READY/STOPPING lifecycle.
9. Verify ordered teardown and record failures without silently retrying into an unknown state.

A failure at this gate is useful evidence. Fix the bootstrap before adding more runtime complexity. Simulator output and GitHub-hosted CI cannot substitute for this physical test.

## 06 Priority order after bootstrap

Once the physical bootstrap passes, advance the VR domain in dependency order:

1. **Frame substrate:** session-owned swapchain, frame timing, acquire/wait/release, and frame submission with strict lifecycle cleanup.
2. **Spatial observer:** view/pose acquisition, tracking validity, coordinate spaces, and reference-frame conversion.
3. **Input substrate:** controller/hand capability discovery and device sampling, kept separate from semantic actions.
4. **Semantic interaction:** one bounded select/confirm action mapped to the portable interaction contract, with authorization remaining in the owning host/Experience.
5. **Presentation vertical slice:** render a simple real immersive scene and the Workshop moniker Experience; record frame behavior on the headset.
6. **Play-space model:** Experience-declared clearance recommendation, MyVR-owned local boundary presentation, and explicit relationship to the platform Guardian.
7. **Haptics and spatial audio:** capability-gated affordances with graceful fallback.
8. **Companion and offline behavior:** prove standalone operation first, then add bounded, authenticated, non-frame-critical delegation.
9. **Shared runtime integration:** consume the agreed manifest → FSM_COS → RuntimeAssembly → FSM_Foundry contract and prove cancellation/cleanup in an end-to-end test.

Do not reverse this order by building a large scene or interaction layer on top of an unverified session substrate.

## Implementation coordination

- [MyVR domain and peer-host architecture issue #26](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/26) tracks the broad responsibility map.
- [Manifest-to-Foundry integration issue #27](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/27) tracks the typed runtime handoff.
- [Quest 2 standalone proof issue #14](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/14) tracks the first complete headset Experience.
- [Play-space boundary draft PR #18](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/18) documents the Experience-scaled clearance boundary and its current non-implementation.
- [Quest Android host draft PR #16](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/16) contains the candidate host implementation and its explicit hardware gate.

## Practice

1. Why is a capability snapshot not equivalent to successful runtime discovery?
2. Which responsibilities must remain local for an Experience to be independently usable on Quest?
3. Why must frame submission wait until the physical session bootstrap is verified?
4. What is the difference between a device input event, a semantic intent, and authorization?
5. Which interface and object types must be inspected before the conceptual RuntimeAssembly arrow can become an implemented integration?

**Exercise:** choose one row in the responsibility table and trace it through source, tests, CI, and (where applicable) physical evidence. If one of those links does not exist, record the gap rather than filling it with an architectural assumption.

## Further reading

- [Theory index](README.md)
- [Quest 2 Android host bootstrap](05-quest2-android-host-bootstrap.md)
- [MyVR as a peer host of the shared runtime](06-myvr-as-a-peer-host.md)
- [MyVR domain architecture issue #26](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/26)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
