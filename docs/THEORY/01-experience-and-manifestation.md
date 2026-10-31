<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# 01 — An Experience, Many Manifestations

## 01 What — the problem

Imagine opening the same collaborative workspace on a laptop, in a browser, and inside a headset. The screen layout changes. The available inputs change. The amount of local storage and compute changes. Yet the workspace is still the same workspace.

That distinction sounds obvious until software starts treating each client as a separate application with a separate identity and separate meaning. A VR client can accidentally become the owner of a scene, a desktop client can become the owner of state simply because it has more resources, or a browser client can become the canonical source because it was first.

MyVR exists to avoid that confusion. It gives an Experience an immersive manifestation without becoming the Experience itself.

## 02 Why — identity must outlive presentation

A useful analogy is a stage production. The same play can be performed in a small theater, outdoors, or as an audio drama. The staging and available effects differ, but those differences do not create a new play.

The analogy stops at the point where software must define identity precisely and coordinate concurrent behavior. Workshop Experiences are software artifacts with explicit identity and contracts, not scripts interpreted freely by each client.

The canonical artifact identity is:

`BundleId + Version + ContentHash`

A device session is a different thing. One immutable Experience can be opened by several clients and can have multiple sessions over time. A headset reconnect must not silently create a different Experience; a new headset session must not redefine the artifact.

## 03 How — capability arrangement

An Experience creator declares what the Experience needs. The system can then evaluate those requirements against available manifestations:

```text
                    Experience
                        |
            BundleId + Version + ContentHash
                        |
              creator-declared requirements
                        |
             +----------+----------+
             |          |          |
           WebApp     AnyApp      MyVR
           browser    desktop     VR client
```

This is not a ladder. A valid arrangement might be MyVR alone, AnyApp + MyVR, WebApp + MyVR, or all three. The right arrangement depends on the Experience's requirements and the capabilities actually available.

Requirements can be understood as:
- **Required:** absence prevents entry or a defined required feature.
- **Preferred:** a desirable capability or execution location.
- **Optional:** useful when present, but not necessary.
- **Delegable:** work another manifestation may perform under an explicit contract.
- **Frame-critical:** work needed for immediate presentation or interaction and therefore kept locally viable.

MyVR owns device-local pose and input state. It does not become the owner of all Experience state. AnyApp owns only services explicitly delegated to it. WebApp owns browser-local state and browser APIs within their authority. The Experience/runtime contract defines the remaining semantic ownership.

## 04 See it in a minute — identity is not session

Consider one artifact and two headset sessions:

```text
Experience artifact:
  BundleId     = "experience.demo"
  Version      = "1.2.3"
  ContentHash  = "sha256:0123456789abcdef"

MyVR session A:
  SessionId    = 6a...

MyVR session B:
  SessionId    = b4...
```

The artifact identity is unchanged. Session identifiers distinguish the two manifestations. This example is conceptual; the exact GUIDs are illustrative.

The candidate core encodes this distinction in `ExperienceIdentity` and `MyVrSession`. The constructor requires all three artifact identity parts, trims their values, and stores the Experience reference separately from a non-empty session GUID.

## Implementation mapping

Source-checked against PR #12 candidate head `d0f365f74b9e567151a6d5570d99fb62d2391b75`:

- [`ExperienceIdentity.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/src/MyVR.Core/ExperienceIdentity.cs) — immutable record containing BundleId, Version, and ContentHash.
- [`MyVrSession.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/src/MyVR.Core/MyVrSession.cs) — owns a separate SessionId and lifecycle state.
- [`ExperienceIdentityTests.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/tests/MyVR.Core.Tests/ExperienceIdentityTests.cs) — checks required values, trimming, and value equality.
- [`MyVrSessionTests.cs`](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/blob/feature/myvr-device-session-core/tests/MyVR.Core.Tests/MyVrSessionTests.cs) — checks the Experience reference and session identifier remain distinct.

The tests establish constructor/value behavior and identity separation in the headless core. They do not establish multi-client synchronization or artifact retrieval from a repository.

## Evidence, limits, and counterexamples

**Evidence:** source inspection and the linked deterministic tests support the identity distinction in the PR candidate. The candidate's GitHub Actions run reports 13 passing tests.

**Not established:** no implementation in this slice resolves an artifact from MicroBundleRepository, coordinates several manifestations, or synchronizes shared semantic state. Those responsibilities belong to the appropriate Workshop domain/runtime contracts, not to this local identity value object.

A useful counterexample: if a client invents a new BundleId every time a user enters VR, the same artifact appears to be a different Experience. If it reuses SessionId as artifact identity, reconnect and concurrent-client semantics become ambiguous. The two identifiers answer different questions and must remain separate.

## Practice

1. Why is a content hash part of artifact identity rather than a session identifier?
2. Give an example of a capability that is optional for one Experience but required for another.
3. What should happen to Experience identity when a headset disconnects and reconnects?
4. Design a test that would catch accidental coupling between SessionId and BundleId.

**Investigation:** inspect a future multi-client Experience and identify which state is device-local, which is delegated, and which is semantically shared. Do not assume the client with the most compute owns the shared state.

## Further reading

- [Experience Manifestation](../EXPERIENCE_MANIFESTATION.md)
- [Architecture](../ARCHITECTURE.md)
- [AnyApp Companion](../ANYAPP_COMPANION.md)
- [MicroBundleDomain artifact boundary](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/issues/18)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
