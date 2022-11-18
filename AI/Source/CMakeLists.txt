cmake_minimum_required (VERSION 3.12)

set ("KINAI_ROOT_DIR" "${PROJECT_SOURCE_DIR}/Source")

set ("FILES"
    "${KINAI_ROOT_DIR}/KinAI.h"
    "${KINAI_ROOT_DIR}/InterfaceMacros.h"
    "${KINAI_ROOT_DIR}/Example1.cpp" "${KINAI_ROOT_DIR}/Example1.h"
    "${KINAI_ROOT_DIR}/Example2.cpp" "${KINAI_ROOT_DIR}/Example2.h"
)

add_library (KinAI SHARED ${FILES})

target_compile_definitions (KinAI PRIVATE _KINAI_COMPILING) # see InterfaceMacros.h

# TODO: this project probably shouldn't be aware of the frontend's dependencies
add_custom_command(TARGET KinAI POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:KinAI> "${PROJECT_SOURCE_DIR}/../Frontend/"
)
