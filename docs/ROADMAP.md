# Roadmap

## Phase 0 - Theory

Define MyVR as the VR manifestation/client and lock boundaries with Experience, AnyApp, WebApp, GUI, and FSM_COS.

## Phase 1 - Device Skeleton

<badge>In progress</badge>

Implemented in the headless `.NET 8` core:

- [x] Portable core project and deterministic test project.
- [x] Immutable capability snapshot with adapter-owned capability names.
- [x] Device/runtime adapter interface; fake adapter exercised by tests.
- [x] Separate immutable Experience identity and MyVR session identity.
- [x] Explicit session lifecycle with invalid-transition rejection, suspend/resume, disconnect, and reconnect tests.
- [x] Pull-request and `development` build/test workflow.

Still outstanding:

- [ ] Select an actual headset/device deployment target and its supported runtime.
- [ ] Implement and hardware-test a concrete device/runtime adapter.
- [ ] Add a semantic input adapter after the FSM_UserIO dependency-policy gate is available and approved.
- [ ] Verify the first real device lifecycle without making frame-critical work network-dependent.

The current fake adapter proves only the contract and deterministic core behavior. It is not hardware detection.

## Phase 2 - Creator-defined Experience requirements

Resolve an immutable published Experience identity and its declared capability requirements.

Determine whether the current VR device can satisfy the required immersive and interaction capabilities locally, and identify optional/delegable work that may be supplied by WebApp and/or AnyApp.

A requirement failure is an Experience capability mismatch, not a reason to redefine MyVR's architectural role.

## Phase 3 - First Experience

Load the permitted representation, present a minimal spatial environment, and support one bounded interaction.

The first Experience should explicitly demonstrate at least one requirement that can be satisfied by MyVR alone.

## Phase 4 - Companion

Connect MyVR to AnyApp when the Experience requires or benefits from desktop capabilities, exchange capabilities, synchronize Experience/session identity, heartbeat and reconnect, and delegate one non-frame-critical operation.

AnyApp remains independently usable without MyVR.

## Phase 5 - WebApp + MyVR

Coordinate browser and VR manifestations when an Experience requires browser capabilities, browser computation, distribution, or other WebApp-native capabilities that are not available locally on the VR device.

## Phase 6 - Co-entangled Manifestations

WebApp <-> AnyApp <-> MyVR coordinated session, manifestation handoff, shared bounded Experience events, and capability-aware behavior.

Demonstrate an Experience that intentionally uses all three manifestations simultaneously.

## Phase 7 - Spatial Experience

Observer/camera model, spatial layout, detail horizon, interaction horizon, controller/hand interaction, and appropriate audio/haptic affordances.

## Phase 8 - Device Expansion

Additional runtimes, native/OpenXR implementations where appropriate, WebXR manifestation where appropriate, and device-specific optimization behind adapters.

## Non-goals

MyVR is not Unity infrastructure, a second desktop host, a second composition OS, a repository server, a remote-control shell, a replacement for WebApp, or a requirement for every Experience.
