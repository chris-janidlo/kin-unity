#include "Example2.h"
#include "Logging.h"

namespace KinAI
{
	int multiply(const int a, const int b)
	{
		get_logger()->info("multiplying {} by {}", a, b);
		return a * b;
	}
}
