DOTNET=dotnet
PUBFLAGS=-p:PublishSingleFile=true

ifeq ($(OS),Windows_NT)
	PUBFLAGS += -p:IncludeNativeLibrariesForSelfExtract=true
	ifeq ($(PROCESSOR_ARCHITEW6432),AMD64)
		RID = win-x64
	else
		ifeq ($(PROCESSOR_ARCHITECTURE),AMD64)
			RID = win-x64
		endif
		ifeq ($(PROCESSOR_ARCHITECTURE),x86)
			RID = win-x32
		endif
	endif
else
	UNAME_S := $(shell uname -s)
	UNAME_P := $(shell uname -p)
	ifeq ($(UNAME_S),Linux)
		ifeq ($(UNAME_P),x86_64)
			RID = linux-x64
		endif
		ifneq ($(filter %86,$(UNAME_P)),)
			RID = linux-x86
		endif
	endif
	ifeq ($(UNAME_S),Darwin)
		PUBFLAGS += -p:IncludeNativeLibrariesForSelfExtract=true
		RID = osx-x64
	endif
endif

termios: Program.cs termios.csproj
	$(DOTNET) publish $(PUBFLAGS) -r $(RID) -o publish && \
	cp publish/termios termios