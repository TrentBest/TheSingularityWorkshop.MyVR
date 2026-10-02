# Experience Manifestation

An Experience is independent of its client. The same immutable Experience identity may be manifested through WebApp, AnyApp, or MyVR.

```text
Experience
    |
BundleId + Version + ContentHash
    |
+---------+---------+---------+
|         |         |
WebApp   AnyApp    MyVR
browser  desktop   VR
```

## Identity

MyVR must never invent a second identity for an Experience. The canonical artifact identity is BundleId + Version + ContentHash. An immersive session has a separate session identity.

## Session

A MyVR session should distinguish Disconnected, Discovering, Connecting, Connected, Synchronizing, Ready, Immersive, and Suspended.

## Shared state

MyVR owns device-local pose/input state. AnyApp owns services explicitly delegated to it. Experience/runtime ownership is defined by contract. Conflicts are resolved by protocol semantics, not by whichever client sends last.

## Co-entanglement

**Co-entangled** is architectural shorthand for coordinated manifestations of the same Experience. It does not imply literal quantum entanglement.