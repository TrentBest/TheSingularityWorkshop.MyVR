<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# 04 — A Headless Session Walkthrough

## 01 What — the smallest portable slice

The smallest useful MyVR core demonstration is not a rendered room. It is a headless proof that the client can represent an immutable Experience identity, report adapter capabilities, and track one session through a validated lifecycle without importing a headset SDK.

The portable core and the physical device host are deliberately separate. A successful headless test proves deterministic core behavior; it does not prove Android, OpenXR, Vulkan, or headset behavior.

## 02 Why — test the contract before the hardware

Identity validation, capability snapshot rules, and lifecycle transitions are deterministic. They should be tested without a physical device. A hardware test is still necessary for the native loader, runtime extension support, graphics binding, and real application lifecycle.

The fake adapter is intentionally modest. It demonstrates the shape of the adapter contract, not the existence of a hardware adapter.

## 03 How — walk through the portable core

The following example uses the current core types:

```csharp
using TheSingularityWorkshop.MyVR;

var experience = new ExperienceIdentity(
    "experience.demo",
    "1.2.3",
    "sha256:0123456789abcdef");

var session = new MyVrSession(experience);

session.TransitionTo(ImmersiveSessionState.Discovering);
session.TransitionTo(ImmersiveSessionState.Connecting);
session.TransitionTo(ImmersiveSessionState.Connected);
session.TransitionTo(ImmersiveSessionState.Synchronizing);
session.TransitionTo(ImmersiveSessionState.Ready);
session.TransitionTo(ImmersiveSessionState.Immersive);
```

The session has its own GUID. It retains the immutable Experience reference. An attempt to skip from Disconnected directly to Immersive throws `InvalidOperationException`.

A simplified capability check looks like this:

```csharp
var capabilities = new DeviceCapabilities(
    "sample.adapter",
    ["immersive.display", "spatial.input"]);

bool canPresentImmersively =
    capabilities.Supports("immersive.display");
```

The capability names in this example are adapter-supplied strings. They are not a standardized Workshop capability vocabulary.

## 04 See it in a minute — reproduce the core evidence

With the .NET 8 SDK installed, run:

```bash
dotnet test tests/MyVR.Core.Tests/MyVR.Core.Tests.csproj --configuration Release
```

The test project is hardware-free. It covers identity validation/equality, capability snapshot behavior, fake adapter reporting, valid and invalid session transitions, suspension/resume, disconnect, and reconnect.

## Implementation map

| Concern | Owner / candidate implementation | Evidence boundary |
|---|---|---|
| Immutable artifact identity | `ExperienceIdentity` in MyVR.Core | Deterministic unit tests |
| Device-session identity and portable lifecycle | `MyVrSession`, `ImmersiveSessionState` | Deterministic unit tests |
| Capability snapshot | `DeviceCapabilities` | Deterministic unit tests |
| Runtime adapter seam | `IDeviceRuntimeAdapter` | Fake adapter contract test |
| Android Activity and OpenXR/Vulkan host | `MyVR.Quest2.Android` in draft PR #16 | CI build/package only; physical device unverified |
| Shared Experience composition and execution | FSM_COS and FSM_Foundry | Not yet integrated by MyVR |

The current Quest implementation is in [draft PR #16](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/16), not in the canonical `development` branch. The [core CI workflow](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41227205130) and [Quest Android workflow](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41227205152) passed for PR head `8c0dd80f9a51ab90fac034c435ed15134cdce949`. These runs are candidate evidence, not a release or hardware claim.

## Evidence, limits, and failure paths

The portable tests verify identity validation/equality, capability name validation/deduplication/stable ordering, fake adapter reporting, invalid transition rejection, suspend/resume restoration, idempotent disconnect, identity preservation, and reconnect through discovery.

The Android workflow verifies the host compiles and publishes a signed ARM64 APK containing the packaged OpenXR loader. It does **not** verify that a headset loads the library, accepts extensions, creates Vulkan resources, selects an HMD system, creates an OpenXR session, or responds correctly to pause/resume.

Neither layer currently verifies:
- swapchain creation, frame timing, frame submission, or rendering;
- live headset pose, tracking validity, controller/hand input, audio, or haptics;
- Experience artifact resolution, composition, or workflow execution;
- a network or companion protocol;
- physical Quest 2 runtime behavior.

## Practice

1. Make the direct Disconnected → Immersive call. What exception should be observed and why?
2. Disconnect a suspended session. Which identity values remain unchanged?
3. Replace the fake adapter with a hypothetical hardware adapter. Which responsibilities must remain behind the interface?
4. What additional evidence is needed to move the Android host from CI-verified to hardware-verified?

**Extension:** when the headset is available, follow Chapter 5's bootstrap checklist and capture a complete log before attempting any frame-loop or rendering work.

## Further reading

- [Theory index](README.md)
- [Quest 2 Android host bootstrap](05-quest2-android-host-bootstrap.md)
- [Architecture](../ARCHITECTURE.md)
- [Roadmap](../ROADMAP.md)
- [Candidate PR #16](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/16)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
