#include "Node.h"

#include <memory>
#include <vector>
#include <algorithm>

namespace kin_ai::mcts
{
	void node::detach(std::shared_ptr<node> child)
	{
		// fail fast if the child has an invalid pointer to its parent:
		auto parent = std::shared_ptr(child->parent);

		std::remove(parent->children.begin(), parent->children.end(), child);
	}
}
