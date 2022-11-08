cmake_minimum_required (VERSION 3.12)

add_library (KinAI SHARED "KinAI.cpp" "KinAI.h")

target_link_libraries (KinAI PRIVATE MonteCarloTreeSearch)
target_include_directories (KinAI PRIVATE "${CMAKE_CURRENT_SOURCE_DIR}/../MonteCarloTreeSearch")
