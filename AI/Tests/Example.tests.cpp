#include <catch2/catch_test_macros.hpp>
#include "Example1.h"
#include "Example2.h"

TEST_CASE("Multiply")
{
	REQUIRE(KinAI::multiply(5, 10) == 50);
	//REQUIRE(multiply(5, 11) == 50); // failing assertion
}

TEST_CASE("Mul by 5")
{
	REQUIRE(KinAI::mul_by_5(10) == 50);
	//REQUIRE(mul_by_5(11) == 50); // failing assertion
}
