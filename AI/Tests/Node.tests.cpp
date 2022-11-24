#include <catch2/catch_test_macros.hpp>

#include "MCTS/Node.h"
#include <memory>

TEST_CASE( "Detached trees are destroyed", "[mcts][node]" ) {
	auto parent = std::make_shared<kin_ai::mcts::node>();
	parent->children = {
		std::make_shared<kin_ai::mcts::node>(),
		std::make_shared<kin_ai::mcts::node>(),
		std::make_shared<kin_ai::mcts::node>()
	};
	for (auto &child : parent->children)
	{
		child->parent = parent;
	}

	auto first_level_child = parent->children[1];
	first_level_child->children = {
		std::make_shared<kin_ai::mcts::node>(),
		std::make_shared<kin_ai::mcts::node>(),
		std::make_shared<kin_ai::mcts::node>()
	};
	for (auto &child : first_level_child->children)
	{
		child->parent = first_level_child;
	}

	auto second_level_child = first_level_child->children[2];

	// TODO: no idea how to test that destroying the parent destroys the children. maybe something 
}
