#include <catch2/catch_test_macros.hpp>
#include "MonteCarloTreeSearch.h"

TEST_CASE("Multiply")
{
	REQUIRE(multiply(5, 10) == 50);
	//REQUIRE(multiply(5, 11) == 50); // failing assertion
}
