<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# 06 — MyVR as a Peer Host of the Shared Runtime

## 01 What — a host is not an execution kernel

MyVR is an application host for a VR device. It receives an Experience/runtime arrangement and presents the permitted result through immersive device capabilities.

A host must not become a second composition engine simply because it is the application the user sees. It must not duplicate loading, arbitration, workflow scheduling, or shared execution lifecycle. Those responsibilities belong to the Workshop's shared runtime layers.

The same rule applies to desktop, browser, service, and VR hosts. They are peers that consume shared runtime capabilities; the shared kernel must not contain special cases for each host.

## 02 Why — one execution model, multiple manifestations

If every host owns a separate execution kernel, the same Experience can behave differently depending on which client launched it. Each host may implement different dependency resolution, lifecycle rules, cancellation behavior, and failure semantics. Fixes then have to be repeated and cross-host behavior becomes difficult to reason about.

A shared execution kernel gives the Workshop one place to define composition and execution behavior. Each host remains free to implement its own device lifecycle and presentation.

MyVR's distinctive responsibility is immersive manifestation:
- headset/runtime integration;
- VR-specific application and session lifecycle;
- spatial presentation and interaction;
- device tracking and input adaptation;
- haptics and spatial audio boundaries;
- optional, explicitly bounded companion connectivity.

Those responsibilities do not make MyVR the owner of Experience meaning or shared execution.

## 03 How — intended dependency and handoff

The current architectural target is:

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

The important boundary is the typed `RuntimeAssembly` handoff. FSM_COS is responsible for composing the runtime assembly from the manifest and configuration. FSM_Foundry owns the common execution lifecycle and orchestration. MyVR consumes the prepared runtime through a host-facing contract and contributes VR-specific behavior.

The diagram is the **target architecture**, not a claim that these references and calls are implemented in MyVR today.

### Responsibility matrix

| Concern | Owning domain | MyVR's relationship |
|---|---|---|
| Experience artifact identity and manifest semantics | Experiences / canonical artifact contracts | Consumes stable identity and declared requirements |
| MicroBundle discovery and composition | FSM_COS and its repository/domain contracts | Does not implement a second composer |
| Runtime assembly handoff | FSM_COS → FSM_Foundry contract | Receives the typed result through the agreed integration |
| Shared execution lifecycle and orchestration | FSM_Foundry | Consumes; does not fork or reschedule it |
| VR application/device lifecycle | MyVR | Owns |
| OpenXR, Android, Vulkan device adaptation | MyVR device host | Owns behind platform boundary |
| Spatial presentation and immediate VR interaction | MyVR + presentation domains | Owns the VR manifestation boundary |
| Portable Experience meaning and authorization | Experience/host policy contracts | Does not redefine or bypass |

## 04 See it in a minute — a host handoff in plain language

A user selects an Experience for immersive presentation.

1. The Experience is identified by its canonical artifact identity and declares the capabilities it needs.
2. The manifest and configuration are supplied to FSM_COS.
3. FSM_COS composes the required MicroBundles and produces the typed runtime assembly.
4. FSM_Foundry accepts that assembly and owns the shared execution lifecycle.
5. MyVR initializes or resumes its device host, evaluates its local presentation capabilities, and attaches the VR manifestation through the host contract.
6. MyVR translates permitted device observations into bounded semantic interactions. It does not interpret a controller button as blanket authorization.
7. If a required VR capability is unavailable, MyVR reports a host capability failure or graceful degradation according to the Experience contract. It does not silently substitute a different Experience or create a competing runtime.

The companion AnyApp may supply explicitly delegated, non-frame-critical services when the Experience requires them. Immediate pose, view, and frame-critical interaction remain locally viable on the headset.

## Implementation status and evidence

**Implemented in the current MyVR candidate:**
- portable Experience identity and MyVR session identity;
- a portable device capability snapshot and runtime-adapter seam;
- a deterministic headless lifecycle model;
- a separate Quest 2 Android host candidate that initializes OpenXR, selects the HMD system, creates a runtime-selected Vulkan device, creates an OpenXR session, and handles Android/OpenXR lifecycle events.

**Not yet integrated or verified in MyVR:**
- manifest/configuration ingestion through the canonical Experience contract;
- FSM_COS composition and typed RuntimeAssembly handoff;
- FSM_Foundry execution lifecycle integration;
- an end-to-end integration test spanning FSM_COS, Foundry, and MyVR;
- a VR presentation adapter consuming a live runtime assembly;
- semantic controller/hand input and bounded Experience events.

The MyVR repository tracks this work in [issue #27](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/27) and the wider peer-host/device domain in [issue #26](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/26). The typed handoff is coordinated with [FSM_Foundry issue #23](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/23). The conceptual cross-host plan is also tracked in [The issue #33](https://github.com/TrentBest/The/issues/33).

The Quest Android host candidate is still a draft PR and has not been physically verified on a Quest 2. Successful CI packaging is not a runtime or integration test.

## Practice

1. Why should FSM_Foundry remain unaware of MyVR, AnyApp, and WebApp?
2. What should MyVR receive from FSM_Foundry rather than reconstruct itself?
3. Which MyVR responsibilities remain valid even if the Experience runtime is headless?
4. Why is controller recognition different from authorization?
5. What would an integration test need to prove before this target flow can be called implemented?

**Exercise:** draw the exact object and lifecycle handoff between FSM_COS, FSM_Foundry, and MyVR after reading their current public contracts. Replace each conceptual arrow with the actual interface/type name. If an arrow has no implemented contract yet, label it as a gap rather than inventing an API.

## Further reading

- [Theory index](README.md)
- [MyVR architecture issue #26](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/26)
- [Manifest-to-Foundry integration issue #27](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/27)
- [FSM_Foundry typed RuntimeAssembly handoff issue #23](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/23)
- [The cross-host plan issue #33](https://github.com/TrentBest/The/issues/33)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
