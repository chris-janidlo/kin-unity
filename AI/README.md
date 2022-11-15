# Structure

* KinAI - public interface (as shared library) that the Frontend calls into for getting AI decisions
* MonteCarloTreeSearch - generic (game-independent) implementation of Monte Carlo Tree Search
* Tests - tests for every subproject

# Building

TODO: how to build

Build artefacts for every subproject are output to `./build/`.

# Testing

Simply build the project and run `./build/tests[.extension]` (where the value of `[.extension]` depends on your OS). Since this is a Catch2 test executable, you can always pass `-?` as an argument for command-line help.
