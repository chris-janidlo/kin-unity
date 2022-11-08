cmake_minimum_required (VERSION 3.12)

project ("AI")

set (CMAKE_CXX_STANDARD 20)

add_subdirectory (MonteCarloTreeSearch)
add_subdirectory (KinAI)

# TODO: this project probably shouldn't be aware of the frontend's dependencies
set_target_properties ( KinAI
	PROPERTIES
	RUNTIME_OUTPUT_DIRECTORY "${PROJECT_SOURCE_DIR}/../Frontend/"
)
