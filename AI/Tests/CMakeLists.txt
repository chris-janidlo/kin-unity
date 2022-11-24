cmake_minimum_required (VERSION 3.12)

include(FetchContent)

FetchContent_Declare(
  Catch2
  GIT_REPOSITORY https://github.com/catchorg/Catch2.git
  GIT_TAG        v3.1.1
)

FetchContent_MakeAvailable(Catch2)

add_executable (
	tests
	"Node.tests.cpp"	
)

target_link_libraries (tests PRIVATE KinAI)
# for ease of access to everything, include everything that KinAI includes:
get_target_property (KinAIIncludes KinAI INCLUDE_DIRECTORIES)
target_include_directories (tests PRIVATE ${KinAIIncludes})

target_link_libraries(tests PRIVATE Catch2::Catch2WithMain)

# TODO: actually use CTest
list(APPEND CMAKE_MODULE_PATH ${catch2_SOURCE_DIR}/extras)
include(CTest)
include(Catch)
catch_discover_tests(tests)
