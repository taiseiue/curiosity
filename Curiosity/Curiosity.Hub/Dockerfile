# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY Curiosity.sln .
COPY Curiosity.Hub/Curiosity.Hub.csproj ./Curiosity.Hub/
COPY Curiosity.Adapter/Curiosity.Adapter.csproj ./Curiosity.Adapter/
RUN dotnet restore Curiosity.Hub

# copy everything else and build app
COPY Curiosity.Hub/. ./Curiosity.Hub/
COPY Curiosity.Adapter/. ./Curiosity.Adapter/
WORKDIR /source/Curiosity.Hub
RUN dotnet publish Curiosity.Hub.csproj -c Release -o /app --use-current-runtime true

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
# 日本語の設定
ENV LANG ja_JP.UTF-8  
ENV LANGUAGE ja_JP:ja  
ENV LC_ALL ja_JP.UTF-8
WORKDIR /app
COPY --from=build /app/ ./
CMD ["dotnet", "Curiosity.Hub.dll"]