#pragma once

#include <memory>

#include "spdlog/spdlog.h"

namespace KinAI
{
	namespace logging
	{
		constexpr auto logger_name = "kin_ai_logger";
		constexpr auto logfile_name = "AI.log";
	}

	std::shared_ptr<spdlog::logger> get_logger();
}
