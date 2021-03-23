cd ../Common
start dotnet build

cd ../Valuator
start dotnet run --urls "http://localhost:5001"
start dotnet run --urls "http://localhost:5002"

cd ../RankCalculator
start dotnet run
start dotnet run

cd ../nginx
start nginx.exe