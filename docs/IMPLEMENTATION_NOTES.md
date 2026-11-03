[![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)](https://github.com/TrentBest/TheSingularityWorkshop)

# Implementation Notes

These notes define implementation boundaries and evidence requirements. They do not imply that every described integration or device capability is already implemented.

## Dependency direction

```text
                 FSM_Foundry
              shared host kernel
                     |
          +----------+----------+
          |          |          |
        AnyApp     WebApp      MyVR
        host        host       host
                                |
                       MyVR-owned adapters
                                |
                       Android / OpenXR /
                       Vulkan / device APIs

          ServerApp = optional remote compute host
```

FSM_Foundry owns shared execution/loading lifecycle and orchestration. It must not depend on or special-case its consuming hosts. MyVR consumes the shared host contract and owns only VR-specific composition, device adapters, presentation, and lifecycle.

Keep the portable MyVR device/session core independent of Foundry implementation details. Once the shared Foundry host contract is confirmed against [FSM_Foundry issue #20](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/20), add only the narrow MyVR adapter needed to consume that lifecycle. Do not introduce Foundry execution policy, scheduling, artifact resolution, or composition into MyVR. Do not claim the adapter exists until its project/package/API reference is verified in source.

Experience semantics and artifact identity remain canonical upstream contracts. ServerApp is an optional remote computation host, not a peer client manifestation. Remote delegation must be explicit, bounded, user-authorized, and non-frame-critical.

## VR implementation order and physical gate

1. Establish the portable device/session boundary and deterministic tests.
2. Establish the Android/Quest application and OpenXR host bootstrap.
3. Verify the bootstrap on the physical target device, recording device/runtime, loader, graphics API, lifecycle, logs, and observed result.
4. Only after that gate passes, implement swapchain management, frame loop, frame submission, rendering, and recovery.
5. Add spatial tracking, controller/hand input, coordinate frames, play-space handling, and interaction affordances behind MyVR-owned abstractions.
6. Add haptics/audio and optional companion/remote-compute integration where an Experience requires them.

A CI build or Android package is not physical-device evidence. Do not advance past the physical bootstrap gate based only on CI success.

## First Experience

The first installed product is the Workshop Moniker Experience. It must work locally/standalone. The creator's moniker may compose before or after the Workshop moniker, following the canonical Experience contract. MyVR must not invent a duplicate Experience identity or semantics.

## Testing and evidence

Keep evidence categories separate:

- **Source implemented** — behavior exists in the reviewed source tree.
- **Automated tested** — deterministic unit/contract tests exercise it.
- **CI verified** — a named workflow passed against an exact commit SHA.
- **Runtime integrated** — behavior was exercised against the intended runtime.
- **Hardware verified** — behavior was observed on the named physical headset/device.

Test identity parsing, artifact identity validation, capability detection, session transitions, malformed messages, replay/expiry behavior, companion lifecycle, bounded event handling, graceful disconnect, and device-independent semantic logic. Keep platform/device tests separate from deterministic core tests.

## Technology rule

Do not select a VR engine merely because it is familiar. Keep the architecture engine-neutral until device targets, deployment constraints, licensing, and required APIs are understood. Runtime-specific dependencies belong behind MyVR-owned adapters.

---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="180" />
  <br />
  <strong>The Singularity Workshop</strong><br />
  <em>If a picture is worth a thousand words, what is a living picture worth?</em>
</p>
