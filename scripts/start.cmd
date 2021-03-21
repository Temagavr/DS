cd ../../nats-server-v2.1.9-windows-386
start nats-server.exe

cd ../DS/Common
start dotnet build

cd ../Valuator
start dotnet run --urls "http://localhost:5001"
start dotnet run --urls "http://localhost:5002"

cd ../RankCalculator
start dotnet run
start dotnet run

cd ../../../Redis-x64-3.2.100
start redis-server.exe

cd ../../DS/DS/nginx-1.19.7/nginx-1.19.7/
start nginx.exe