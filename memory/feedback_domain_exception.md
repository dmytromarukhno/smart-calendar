---
name: DomainException must be concrete
description: DomainException in this project should be a concrete class, not abstract
type: feedback
---

DomainException should be a concrete (non-abstract) class despite CODE_FRAGMENTS.md showing `abstract` in the class definition.

**Why:** The Entity code fragments in CODE_FRAGMENTS.md directly instantiate `new DomainException("Title is required")` inside Event, Scene, Device, User. Making it abstract causes CS0144 compilation errors. The spec is internally inconsistent on this point; the Entity code takes precedence.

**How to apply:** When writing or reviewing domain exception code, keep `DomainException` as a plain non-abstract class. Specific sub-exceptions (SceneNotFoundException, DeviceNotFoundException, DeviceOfflineException) remain as sealed subclasses.
