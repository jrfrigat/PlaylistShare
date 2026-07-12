@echo off
REM Publish the stack with a CONTAINERIZED PostgreSQL: base + prod overlay + postgres overlay.
REM For the default external MS SQL Server, use run-docker-compose.bat instead.
REM For an EXTERNAL Postgres, set DB_PROVIDER=Postgres + a Postgres CONNECTION_STRING in .env and use
REM run-docker-compose.bat (this postgres overlay is only for the containerized database).

cd /d "%~dp0"

if not exist ".env" (
    echo [ERROR] .env not found next to this script.
    echo Copy .env.example to .env and set POSTGRES_PASSWORD (and the other values), then run again.
    pause
    exit /b 1
)

docker compose -f docker-compose.yml -f docker-compose.prod.yml -f docker-compose.postgres.yml up -d --build --force-recreate
pause
