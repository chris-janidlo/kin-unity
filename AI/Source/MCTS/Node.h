#pragma once

#include <vector>
#include <memory>

namespace kin_ai::mcts
{
	class node
	{
	public:
		std::vector<std::shared_ptr<node>> children;
		std::shared_ptr<node> parent;

		static void detach(std::shared_ptr<node> child);
	};
}
