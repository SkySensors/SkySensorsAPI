ARG BUILDIMAGE=8.0

FROM mcr.microsoft.com/dotnet/sdk:${BUILDIMAGE} as base
WORKDIR /src

FROM base as build

COPY *.sln nuget.config *.dcproj ./

COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

WORKDIR /src
RUN dotnet restore SkySensorsAPI.sln

COPY . .
RUN dotnet build SkySensorsAPI.sln -c Release 

FROM build as publish
RUN dotnet publish SkySensorsAPI/SkySensorsAPI.csproj -o /app -c Release --no-restore --no-self-contained

FROM mcr.microsoft.com/dotnet/aspnet:${BUILDIMAGE} AS api
RUN addgroup --system --gid 10001 customgroup \
    && adduser --system --uid 10001 --ingroup customgroup --shell /bin/sh customuser
WORKDIR /app
COPY --from=publish /app .
RUN chown 10001:10001 /app
USER 10001
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SkySensorsAPI.dll"]

FROM api as test
USER 0
RUN apt-get update && apt-get install -y curl
USER 10001