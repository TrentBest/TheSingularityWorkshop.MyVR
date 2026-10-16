# Experience Manifestation

An Experience is independent of its client. The same immutable Experience identity may be manifested through WebApp, AnyApp, or MyVR.

```
Experience
    |
BundleId + Version + ContentHash
    |
creator-declared requirements
    |
+---------+---------+---------+
|         |         |         |
WebApp   AnyApp    MyVR
browser  desktop   VR
```

## Identity

MyVR must never invent a second identity for an Experience. The canonical artifact identity is BundleId + Version + ContentHash. An immersive session has a separate session identity.

## Creator-owned requirements

The Experience creator defines what the Experience requires.

Requirements may include:

- **Required** capabilities needed for entry or a defined feature.
- **Preferred** capabilities or execution locations.
- **Optional** capabilities that improve the Experience when available.
- **Delegable** work that another manifestation may perform.
- **Frame-critical** work that must remain local to the manifestation responsible for immediate presentation or interaction.

A VR device can therefore be insufficient for a particular Experience without being insufficient for MyVR as a client.

## Capability combinations

The system must not assume a linear capability ladder.

Valid Experience arrangements may include:

```
MyVR
WebApp + MyVR
AnyApp
AnyApp + MyVR
WebApp + AnyApp
WebApp + AnyApp + MyVR
```

These are examples, not a fixed list. The Experience declares requirements; the coordination layer evaluates available capabilities.

## Session

A MyVR session should distinguish Disconnected, Discovering, Connecting, Connected, Synchronizing, Ready, Immersive, and Suspended.

## Shared state

MyVR owns device-local pose/input state. AnyApp owns services explicitly delegated to it. WebApp owns browser-local state and APIs within its authority. Experience/runtime ownership is defined by contract.

Conflicts are resolved by protocol semantics, not by whichever client sends last.

## Simultaneous participation

All manifestations may participate in one Experience at the same time.

For a sufficiently ambitious Experience, MyVR may provide frame-critical immersive interaction, WebApp may provide browser APIs or additional compute, and AnyApp may provide desktop CPU/GPU computation, persistence, artifact caching, or other explicitly delegated services.

The clients remain independent even while coordinated.

## Co-entanglement

**Co-entangled** is architectural shorthand for coordinated manifestations of the same Experience. It does not imply literal quantum entanglement.
