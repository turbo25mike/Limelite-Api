FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /build
COPY . /build
RUN echo $(ls -1 /build)

ENV API_PLATFORM=SERVER

ENV NUGET_SOURCE="https://nuget.pkg.github.com/leupoldoptics/index.json"
ENV NUGET_USERNAME="turbo25mike"
ENV NUGET_READONLY_PASSWORD="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"

RUN dotnet nuget add source ${NUGET_SOURCE} --name="Leupold" --username ${NUGET_USERNAME} --valid-authentication-types basic --store-password-in-clear-text --password ${NUGET_READONLY_PASSWORD}

RUN dotnet restore
RUN dotnet publish -o /publish -c Release

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /publish/ .

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Api.dll"]