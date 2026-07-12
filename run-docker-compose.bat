@echo off
REM Publish the stack: base compose + production overlay (secrets from .env + Data Protection volume).
REM The auto-merged docker-compose.override.yml holds Visual Studio debug settings (UserSecrets mounts,
REM 8080/8081) that must NOT apply here, so it is intentionally not passed.

REM Always run from this script's own directory so the relative -f paths and .env resolve correctly,
REM no matter where the script is launched from.
cd /d "%~dp0"

REM Production secrets come from .env (see .env.example). Fail fast with a clear message if it is absent
REM instead of silently starting containers with empty connection string / keys.
if not exist ".env" (
    echo [ERROR] .env not found next to this script.
    echo Copy .env.example to .env and fill in the real secrets, then run again.
    pause
    exit /b 1
)

docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build --force-recreate
pause
