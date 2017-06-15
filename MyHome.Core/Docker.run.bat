dotnet build -c Release
dotnet publish -c Release -o out
docker build -t dotnetapp .
docker run -it -p 5000:5000 dotnetapp