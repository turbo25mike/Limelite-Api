FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /build
COPY . /build
RUN echo $(ls -1 /build)

ENV API_PLATFORM=SERVER

RUN dotnet restore
RUN dotnet publish -o /publish -c Release

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /publish/ .

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Api.dll"]