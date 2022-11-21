# Building

## Windows
Easiest build system is through Visual Studio:
* Generate build files by saving a CMake file - one of the `CMakeLists.txt` files, or `CMakePresets.json`.
* Run the build by pressing `ctrl-shift-b`.

## macOS/Linux
Note: builds are untested on Linux.

First, generate the build system by running CMake in this directory:

```
cmake . --preset unix
```

Then you can build the project by running the `Makefile` in `build/cmake_bin`.

With a terminal in the `build` directory, you can use the following commands to regenerate the build system and build the project, respectively:

```
cmake .. --preset unix
make -C cmake_bin
```

# Testing

Simply build the project and run `build/tests[.extension]` (where the value of `[.extension]` depends on your OS). Since this is a Catch2 test executable, you can always pass `-?` as an argument for command-line help.
