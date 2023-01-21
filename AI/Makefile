# make everything phony:
MAKEFLAGS += --always-make

BIN_EXT=

LIB_SRC_PRE=
LIB_SRC_EXT=
LIB_DEST_EXT=

ifeq ($(OS),Windows_NT)
    BIN_EXT=exe
	LIB_SRC_EXT=dll
	LIB_DEST_EXT=dll
else
    UNAME_S := $(shell uname -s)
    ifeq ($(UNAME_S),Linux)
		$(error linux not yet supported)
    endif
    ifeq ($(UNAME_S),Darwin)
		LIB_SRC_PRE=lib
		LIB_SRC_EXT=dylib
		LIB_DEST_EXT=bundle
    endif
endif

build-bootstrapper-to-frontend:
	cargo build -r -p bootstrapper
	cp target/release/${LIB_SRC_PRE}bootstrapper.${LIB_SRC_EXT} ../Frontend/bootstrapper.${LIB_DEST_EXT}

build-ai_server-to-frontend:
	cargo build -r -p ai_server
	cp target/release/ai_server.${BIN_EXT} ../Frontend/ai_server

build-to-frontend: build-bootstrapper-to-frontend build-ai_server-to-frontend

lint: clippy-fix format
format:
	cargo +nightly fmt
clippy-fix:
	cargo clippy --fix --allow-dirty --allow-staged
