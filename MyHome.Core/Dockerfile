FROM microsoft/dotnet:runtime
WORKDIR /dotnetapp
COPY out .  
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "MyHome.Core.dll"]