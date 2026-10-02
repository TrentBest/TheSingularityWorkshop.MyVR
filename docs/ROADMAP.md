# Roadmap

## Phase 0 - Theory

Define MyVR as the VR manifestation/client and lock boundaries with Experience, AnyApp, WebApp, GUI, and FSM_COS.

## Phase 1 - Device Skeleton

Establish the client project, detect VR/runtime capabilities, expose a minimal capability model, create a testable session state machine, and establish a semantic-to-VR boundary.

## Phase 2 - First Experience

Resolve an immutable published Experience identity, load its permitted representation, present a minimal spatial environment, and support one bounded interaction.

## Phase 3 - Companion

Connect MyVR to AnyApp, exchange capabilities, synchronize Experience/session identity, heartbeat and reconnect, and delegate one non-frame-critical operation.

## Phase 4 - Spatial Experience

Observer/camera model, spatial layout, detail horizon, interaction horizon, controller/hand interaction, and appropriate audio/haptic affordances.

## Phase 5 - Co-entangled Manifestations

WebApp <-> AnyApp <-> MyVR coordinated session, manifestation handoff, shared bounded Experience events, and capability-aware behavior.

## Phase 6 - Device Expansion

Additional runtimes, native/OpenXR implementations where appropriate, WebXR manifestation where appropriate, and device-specific optimization behind adapters.

## Non-goals

MyVR is not Unity infrastructure, a second desktop host, a second composition OS, a repository server, a remote-control shell, a replacement for WebApp, or a requirement for every Experience.