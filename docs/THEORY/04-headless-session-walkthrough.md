<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# 04 — A Headless Session Walkthrough

## 01 What — the smallest real slice

The smallest useful MyVR core demonstration is not a rendered room. It is a headless proof that the client can represent an immutable Experience identity, report adapter capabilities, and track one session through a validated lifecycle without importing a headset SDK.

This matters because device-independent behavior can be tested before a hardware target is selected. It also prevents a premature engine/runtime choice from shaping the Experience contract.

## 02 Why — test the contract before the hardware

A headset test is valuable, but it is not the only useful test. Identity validation, capability snapshot rules, and lifecycle transitions are deterministic. They should be testable without a physical device.

The fake adapter is intentionally modest. It demonstrates the shape of the adapter contract, not the existence of a concrete runtime integration.

## 03 How — walk through the candidate

The following example uses the public types in the PR #12 candidate. It is a source-grounded illustration; run the repository tests for executable verification.

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

The capability names in this example are adapter-supplied strings. They are not a standardized Workshop capability vocabulary yet.

## 04 See it in a minute — reproduce the evidence

With the .NET 8 SDK installed, run:

```bash
dotnet test tests/MyVR.Core.Tests/MyVR.Core.Tests.csproj --configuration Release
```

Candidate CI evidence (candidate head `d0f365f74b9e567151a6d5570d99fb62d2391b75`):

- [Feature-branch push run #8](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41124322535) — successful restore, Release build, and tests.
- [Pull-request validation run #9](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/actions/runs/41124330475) — successful PR validation.
- Release build: 0 warnings, 0 errors.
- Tests: 13 passed, 0 failed, 0 skipped.

These runs are evidence for the candidate commit. They do not mean the documentation branch itself has been built, and they do not verify hardware.

## Implementation map

| Concern | Candidate implementation | Evidence |
|---|---|---|
| Artifact identity | `ExperienceIdentity` | `ExperienceIdentityTests` |
| Session identity and lifecycle | `MyVrSession`, `ImmersiveSessionState` | `MyVrSessionTests` |
| Capability snapshot | `DeviceCapabilities` | `DeviceCapabilitiesTests` |
| Runtime boundary | `IDeviceRuntimeAdapter` | fake adapter test |

All links point to the [candidate source branch](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/tree/feature/myvr-device-session-core), not to `development`. The implementation remains under review in draft PR #12.

## Evidence, limits, and failure paths

The tests verify:
- blank identity parts are rejected and valid identity parts are trimmed;
- identity values compare by their three artifact identity fields;
- capability names are validated, deduplicated, and exposed in stable order;
- a fake adapter can report a capability snapshot;
- invalid lifecycle transitions are rejected;
- suspension and resume restore the previous state;
- disconnect is idempotent and preserves both identities;
- a disconnected session can begin the discovery path again.

The tests do not verify:
- physical device enumeration or runtime API behavior;
- frame timing, rendering, tracking, controller/hand input, audio, or haptics;
- a network or companion protocol;
- Experience artifact resolution, composition, or workflow execution;
- real application suspension and OS-driven recovery.

## Practice

1. Make the direct Disconnected → Immersive call. What exception should be observed and why?
2. Disconnect a suspended session. Which identity values remain unchanged?
3. Replace the fake adapter with a hypothetical hardware adapter. Which responsibilities must remain behind the interface?
4. Add a test for cancellation. What behavior should the adapter contract guarantee when cancellation is requested?

**Extension:** choose a real target device only after collecting its supported runtime APIs, deployment constraints, licensing, input modalities, and performance requirements. Record evidence before selecting a runtime.

## Further reading

- [Theory index](README.md)
- [Architecture](../ARCHITECTURE.md)
- [Roadmap](../ROADMAP.md)
- [Candidate PR #12](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/12)

---

<p align="center"><strong>The Singularity Workshop</strong> · <a href="https://github.com/TrentBest">GitHub</a></p>
