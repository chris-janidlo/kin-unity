#include "Logging.h"

#include <memory>
#include <vector>
#include <string>
#include <stdlib.h>

#include "spdlog/spdlog.h"
#include "spdlog/sinks/rotating_file_sink.h"
#include "spdlog/sinks/stdout_color_sinks.h"

namespace KinAI
{
	std::string get_logfile_folder()
	{
		// https://docs.unity3d.com/Manual/LogFiles.html
		// platform detection from https://stackoverflow.com/a/5920028/5931898

#if defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__)
		const std::string home = getenv("USERPROFILE");
		return home + R"(\AppData\LocalLow\crass_sandwich\Kin\)";
#elif __APPLE__
		return "~/Library/Logs/crass_sandwich/Kin/";
#elif __linux__
		return "~/.config/unity3d/crass_sandwich/Kin/";
#else
		#error "Unknown compiler"
#endif	
	}

	std::shared_ptr<spdlog::logger> get_logger()
	{
		if (auto logger = spdlog::get(logging::logger_name); logger != nullptr) return logger;

		std::vector<spdlog::sink_ptr> sinks;

		const auto filename = get_logfile_folder() + logging::logfile_name;
		constexpr auto max_size = 1024 * 1024;
		constexpr auto max_files = 5;
		const auto file_sink = std::make_shared<spdlog::sinks::rotating_file_sink_mt>(filename, max_size, max_files);
		sinks.push_back(file_sink);

		// TODO: only add this if we're in debug mode
		const auto stderr_sink = std::make_shared<spdlog::sinks::stderr_color_sink_mt>();
		sinks.push_back(stderr_sink);

		auto new_logger = std::make_shared<spdlog::logger>(logging::logger_name, std::begin(sinks), std::end(sinks));
		register_logger(new_logger);
		return new_logger;
	}
}
