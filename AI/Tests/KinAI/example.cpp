#include <catch2/catch_test_macros.hpp>
#include "KinAI.h"

TEST_CASE("Mul by 5")
{
	REQUIRE(mul_by_5(10) == 50);
	//REQUIRE(mul_by_5(11) == 50); // failing assertion
}
