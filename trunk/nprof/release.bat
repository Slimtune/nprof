set VERSION=0.6-alpha

if "%1"=="upload" (
	build\NAnt\NAnt -f:build\nprof.build -D:build.version=%VERSION% -D:build.configuration=release upload
) else (
	build\NAnt\NAnt -f:build\nprof.build -D:build.version=%VERSION% -D:build.configuration=release package
)