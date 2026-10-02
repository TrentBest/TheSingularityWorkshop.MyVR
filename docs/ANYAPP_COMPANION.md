# AnyApp Companion Boundary

AnyApp can act as a desktop companion to MyVR.

```text
MyVR
immersive client
    |
bounded companion protocol
    |
AnyApp
 desktop services
```

A desktop can provide larger local storage, artifact caching, CPU-heavy computation, suitable desktop GPU workloads, persistence, authoring tools, and local-system connections.

## Not a remote shell

MyVR must not send arbitrary shell commands, filesystem paths, assemblies, process launches, or unrestricted reflection instructions. The companion contract exposes named, bounded capabilities.

## Availability

MyVR must remain meaningful without AnyApp. A disconnected companion causes capability loss, not semantic corruption.

## Bridge alignment

The existing WebApp <-> AnyApp bridge establishes useful vocabulary: protocol version, session identity, Experience identity, capabilities, lifecycle state, heartbeat, bounded events, and explicit errors. MyVR should reuse these concepts where appropriate while keeping transport replaceable.