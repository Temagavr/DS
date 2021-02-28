cd ../Valuator
start dotnet run --urls "http://localhost:5001"
start dotnet run --urls "http://localhost:5002"

cd ../../../Redis-x64-3.2.100
start redis-server.exe

cd ../../DS/DS/nginx-1.19.7/nginx-1.19.7/
start nginx.exe