FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
Copy . .

RUN dotnet restore
RUN dotnet build -c Release -o /app

FROM build as publish
RUN dotnet publish -c Releease -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MI.Web.dll"]