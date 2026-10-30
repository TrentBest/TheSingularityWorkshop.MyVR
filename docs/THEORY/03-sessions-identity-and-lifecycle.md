<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# 03 — Sessions, Identity, and Lifecycle

## 01 What — what is a session?

A MyVR session represents one live attempt to manifest an Experience through a VR client. It has a lifecycle: the client discovers a runtime, connects, synchronizes what is needed, becomes ready, and enters immersive presentation. The session may be suspended or disconnected.

A session is not the Experience artifact. It is not a repository record, composition result, workflow, or permission token.

## 02 Why — lifecycle must be explicit

Without an explicit lifecycle, unrelated events can become indistinguishable. A device can be disconnected while the Experience remains valid. A companion can disappear while the headset remains connected. A session can be suspended by the operating system and later resumed. A client may need to reconnect without changing the Experience identity.

A state model makes legal transitions visible and gives tests something concrete to challenge. It also prevents a caller from claiming the session is immersive merely because it has constructed a session object.

## 03 How — the candidate state model

The candidate enum contains these states:

| State | Meaning in the client lifecycle |
|---|---|
| Disconnected | No active device connection is represented |
| Discovering | A runtime/device discovery attempt is underway |
| Connecting | The client is establishing the device connection |
| Connected | Device connection is established |
| Synchronizing | Required session/Experience context is being synchronized |
| Ready | The session is prepared to enter immersive presentation |
| Immersive | Immersive presentation is active |
| Suspended | A previously active connected state is temporarily paused |

The allowed ordinary startup path is:

```text
Disconnected
    -> Discovering
    -> Connecting
    -> Connected
    -> Synchronizing
    -> Ready
    -> Immersive
```

The candidate permits suspension from Connected, Synchronizing, Ready, or Immersive. Resume restores the exact state that existed before suspension. Disconnect returns to Disconnected and clears the remembered suspended state. Reconnect begins again through Discovering.

This state machine is deliberately local. It does not encode composition arbitration, workflow execution, artifact download, authentication, or companion protocol state.

## 04 See it in a minute — a lifecycle walkthrough

1. Construct a validated `ExperienceIdentity`.
2. Construct `MyVrSession` for that Experience. The session receives its own non-empty GUID.
3. Move through Discovering, Connecting, Connected, Synchronizing, and Ready.
4. Enter Immersive only from Ready.
5. If the OS suspends the app, transition to Suspended.
6. Resume to the exact prior state, or disconnect.
7. After disconnect, reconnect through Discovering while retaining the Experience and session identities.

The candidate test [MyVrSessionTests.cs](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/tests/MyVR.Core.Tests/MyVrSessionTests.cs) exercises the startup path, invalid transitions, suspension/resume from four states, disconnect identity preservation, idempotent disconnect, and reconnect.

## Transition reasoning

| Current state | Permitted next state(s) |
|---|---|
| Disconnected | Discovering |
| Discovering | Connecting, Disconnected |
| Connecting | Connected, Disconnected |
| Connected | Synchronizing, Suspended, Disconnected |
| Synchronizing | Ready, Suspended, Disconnected |
| Ready | Immersive, Suspended, Disconnected |
| Immersive | Ready, Suspended, Disconnected |
| Suspended | Resume to remembered state, or Disconnect |

The final row is intentionally different: a suspended session cannot be transitioned directly to an arbitrary active state. It must use `Resume()` or `Disconnect()`.

## Implementation mapping

Source-checked against PR #12 candidate head `d0f365f74b9e567151a6d5570d99fb62d2391b75`:

- [`ImmersiveSessionState.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/src/MyVR.Core/ImmersiveSessionState.cs) — state vocabulary.
- [`MyVrSession.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/src/MyVR.Core/MyVrSession.cs) — transition map, suspend/resume, disconnect, and identity ownership.
- [`MyVrSessionTests.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/tests/MyVR.Core.Tests/MyVrSessionTests.cs) — lifecycle behavior and invalid transition checks.

The transition implementation is a small in-memory state holder. It is not thread-safe coordination, a persisted session protocol, or a full device lifecycle manager.

## Evidence, limits, and counterexamples

**Evidence:** candidate CI reports the lifecycle tests passing. Tests cover the explicit state transitions described above.

**Not established:** races between platform callbacks, process termination recovery, persisted sessions, authentication renewal, companion heartbeat/reconnect, input event delivery, or real OS suspend/resume behavior.

A counterexample to avoid: do not make Suspended a synonym for Disconnected. Suspension retains the prior active state so it can be resumed; disconnect ends the connection and clears the resume point. Another counterexample: do not make a reconnect silently jump to Immersive. The current model requires a fresh discovery/connection path.

## Practice

1. Why does Suspended remember a prior state instead of always resuming to Ready?
2. Which states should be allowed to suspend in a future platform-specific lifecycle, and what evidence would justify changing the set?
3. What should happen if a device is disconnected while Suspended?
4. Which transitions need to be persisted if process death and recovery become requirements?

**Exercise:** add a proposed state for a runtime that is connected but has lost tracking. Before changing code, write the invariant, legal incoming/outgoing transitions, recovery behavior, and tests needed. Explain why this is a MyVR lifecycle state rather than an Experience semantic state.

## Further reading

- [Implementation Notes](../IMPLEMENTATION_NOTES.md)
- [Security Model](../SECURITY_MODEL.md)
- [MyVR Core candidate PR #12](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/pull/12)

---

<p align="center"><strong>The Singularity Workshop</strong> · <a href="https://github.com/TrentBest">GitHub</a></p>
