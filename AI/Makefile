# make everything phony:
MAKEFLAGS += --always-make

# we could differentiate these by CPU architecture in addition to OS, especially since
# Unity parses the Plugin directory structure by CPU arch rather than by OS, but for now
# I'm assuming that the builder worker machines will always match the target OS/arch, so
# that builder machines can just blindly copy the binaries they build.
AI_SERVER_SRC=$(error AI_SERVER_SRC must be overridden. hint: target/release/server...)
AI_SERVER_DEST=$(error AI_SERVER_DEST must be overridden. hint: ../Frontend/server...)

BOOTSTRAPPER_SRC=$(error BOOTSTRAPPER_SRC must be overridden. hint: target/release/bootstrapper...)
BOOTSTRAPPER_DEST=$(error BOOTSTRAPPER_DEST must be overridden. hint: ../Frontend/Assets/Plugins/bootstrapper...)

ifeq ($(OS),Windows_NT)
	include platform-make-directives/windows.mk
else
    UNAME_S := $(shell uname -s)
    ifeq ($(UNAME_S),Linux)
		include platform-make-directives/linux.mk
    else ifeq ($(UNAME_S),Darwin)
		include platform-make-directives/mac.mk
	else
		$(error unsupported OS)
    endif
endif

build-bootstrapper-to-frontend:
	cargo build -r -p bootstrapper
	cp ${BOOTSTRAPPER_SRC} ${BOOTSTRAPPER_DEST}

build-server-to-frontend:
	cargo build -r -p server
	cp ${AI_SERVER_SRC} ${AI_SERVER_DEST}

build-all-to-frontend: build-bootstrapper-to-frontend build-server-to-frontend

lint: clippy-fix format
format:
	cargo +nightly fmt
clippy-fix:
	cargo clippy --fix --allow-dirty --allow-staged
