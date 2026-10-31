<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# 02 — The VR Client and Its Device Boundary

## 01 What — a device is not a domain

A headset knows things an ordinary desktop does not: its display configuration, tracked pose, available controllers, perhaps hand tracking, audio input, or room-scale sensors. Those facts matter to an immersive client.

But hardware capability is not the same as Experience meaning. A headset can report that hand tracking exists; it cannot decide what a raised hand means in a particular Experience. A controller can report a trigger press; it cannot authorize a purchase, grant access, or execute a desktop command.

MyVR is the boundary that turns device/runtime observations into bounded client capabilities and, eventually, Experience interactions under explicit contracts.

## 02 Why — isolate volatility and authority

Device APIs vary by vendor, runtime, browser, and deployment model. If platform types leak into the portable Experience contract, every consumer becomes coupled to the device stack. If the headset becomes a general-purpose remote control for AnyApp, a local interaction can accidentally acquire desktop authority.

The boundary therefore has two jobs:

1. **Contain platform variation.** Device/runtime API usage belongs behind a MyVR-owned adapter.
2. **Contain authority.** Device capability, client authority, Experience authority, and desktop authority remain distinct.

A familiar analogy is a translator at a border crossing. The translator can describe what a visitor said and whether a document is present. The translator does not grant the visitor legal authority to act. The analogy stops because software contracts must also validate message shape, identity, lifecycle, and authorization on every relevant boundary.

## 03 How — capability snapshots and adapters

The current candidate core defines `IDeviceRuntimeAdapter` as a narrow asynchronous interface:

```csharp
public interface IDeviceRuntimeAdapter
{
    ValueTask<DeviceCapabilities> DetectCapabilitiesAsync(
        CancellationToken cancellationToken = default);
}
```

The adapter owns platform/vendor API usage. It reports a `DeviceCapabilities` snapshot with a RuntimeId and adapter-supplied capability names. The core deliberately does not impose a vendor taxonomy.

The current `DeviceCapabilities` implementation:
- rejects a blank RuntimeId or null capability sequence;
- rejects blank capability names;
- trims names and removes duplicates using ordinal identity;
- exposes a stable sorted read-only collection;
- supports an ordinal capability lookup.

A capability snapshot describes what an adapter reports. It does not prove that a capability works correctly, is currently authorized, or is suitable for a particular Experience. Those are separate questions.

## 04 See it in a minute — fake adapter

A headless test can exercise the adapter contract without owning a headset:

```csharp
IDeviceRuntimeAdapter adapter = new FakeDeviceRuntimeAdapter(
    new DeviceCapabilities("fake.runtime", ["immersive.display"]));

DeviceCapabilities capabilities =
    await adapter.DetectCapabilitiesAsync();

bool hasDisplay = capabilities.Supports("immersive.display");
```

The actual test uses a private fake adapter in [DeviceCapabilitiesTests.cs](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/tests/MyVR.Core.Tests/DeviceCapabilitiesTests.cs). It verifies the returned RuntimeId and capability lookup.

This is **Verified in CI as a fake-adapter contract test**, not a hardware detection test.

## The frame boundary

Immediate tracking, pose, view transforms, local visual updates, and immediate interaction must remain locally viable. Repository queries, artifact downloads, indexing, persistence, analytics, and other non-frame-critical work may be delegated when an explicit contract permits it.

```text
                 Experience semantics
                         |
              future semantic adapter
                         |
               MyVR-owned client logic
                         |
             +-----------+-----------+
             |                       |
       frame-critical            delegable
       local device work         bounded request
             |                       |
        VR runtime              companion/service
```

A companion round trip must not be required for basic immersive presentation. Losing a companion should reduce optional capability, not corrupt the Experience's meaning or stop the device from maintaining its immediate presentation.

## Implementation mapping

Source-checked against PR #12 candidate head `d0f365f74b9e567151a6d5570d99fb62d2391b75`:

- [`IDeviceRuntimeAdapter.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/src/MyVR.Core/IDeviceRuntimeAdapter.cs) — platform adapter contract.
- [`DeviceCapabilities.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/src/MyVR.Core/DeviceCapabilities.cs) — immutable capability snapshot.
- [`DeviceCapabilitiesTests.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/tests/MyVR.Core.Tests/DeviceCapabilitiesTests.cs) — deterministic fake adapter and snapshot behavior.

No concrete device adapter exists in this candidate. There is no headset discovery, controller input, hand tracking, pose sampling, spatial rendering, or vendor API integration in the core.

## Evidence, limits, and poor-fit cases

**Evidence:** the core contract compiles and its fake adapter test passes in the candidate's GitHub Actions runs. The build reports zero warnings and zero errors.

**Not established:** runtime capability truthfulness, timing, thermal behavior, tracking quality, device compatibility, or performance. A fake adapter proves that the interface can be tested without hardware; it does not prove that any hardware adapter exists.

MyVR is a poor place to put:
- MicroBundle arbitration or Experience composition;
- domain-owned scheduling or workflow execution;
- arbitrary assembly loading;
- unrestricted shell, filesystem, or process operations;
- generic browser UI or desktop service ownership;
- a mandatory network dependency in the frame loop.

Those responsibilities belong to their respective Workshop owners and contracts.

## Practice

1. Why should a capability report not itself grant permission?
2. Which operations in a VR application must remain local to preserve immediate presentation?
3. What evidence would be needed before claiming a headset supports hand tracking reliably?
4. Propose a fake-adapter test for cancellation and explain what it still cannot prove.

**Investigation:** compare two hypothetical runtimes that report different capability vocabularies. Design an adapter mapping that preserves semantic capability without leaking vendor enums into Experience definitions.

## Further reading

- [VR Boundary](../VR_BOUNDARY.md)
- [Security Model](../SECURITY_MODEL.md)
- [AnyApp Companion](../ANYAPP_COMPANION.md)
- [FSM_UserIO dependency gate](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/21)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
