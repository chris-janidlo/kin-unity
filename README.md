Project Structure:

* Frontend: Unity project for playing the game
* AI: Rust source for Kin AI

# Development Requirements
* Unity (version subject to change, install via latest Unity Hub)
* Rust 2021
* `pre-commit` - not strictly necessary for hacking, but expected to be run on every commit. The hooks in this repo depend on:
    * Rust again (with the added requirement of a nightly toolchain, to run `fmt`)
    * `csharpier`, for opinionated C# formatting

# Building

There isn't currently a one-button flow for building both the `AI` package and the Unity game in `Frontend`. Instead, build `AI` (see [`AI` README](AI/README.md)) and then build `Frontend`.

TODO: build automation
