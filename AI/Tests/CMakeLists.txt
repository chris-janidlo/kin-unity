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
	
	"KinAi/example.cpp"

	"MonteCarloTreeSearch/example.cpp"
)

target_link_libraries (tests PRIVATE MonteCarloTreeSearch)
target_include_directories (tests PRIVATE "${CMAKE_CURRENT_SOURCE_DIR}/../MonteCarloTreeSearch")

target_link_libraries (tests PRIVATE KinAI)
target_include_directories (tests PRIVATE "${CMAKE_CURRENT_SOURCE_DIR}/../KinAI")

target_link_libraries(tests PRIVATE Catch2::Catch2WithMain)

# TODO: necessary?
list(APPEND CMAKE_MODULE_PATH ${catch2_SOURCE_DIR}/extras)
include(CTest)
include(Catch)
catch_discover_tests(tests)
