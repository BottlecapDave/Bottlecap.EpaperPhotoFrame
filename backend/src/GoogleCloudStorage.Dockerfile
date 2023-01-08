FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base

WORKDIR /app
ARG environment
ENV ENVIRONMENT=$environment

ENV PORT=8080
ENV ASPNETCORE_URLS=http://*:$PORT
EXPOSE $PORT

COPY ./GoogleCloudStorage/Bottlecap.EPaper.GoogleCloudStorage.Api/*.csproj ./GoogleCloudStorage/Bottlecap.EPaper.GoogleCloudStorage.Api/
COPY ./GoogleCloudStorage/Bottlecap.EPaper.GoogleCloudStorage.Services/*.csproj ./GoogleCloudStorage/Bottlecap.EPaper.GoogleCloudStorage.Services/
COPY ./Bottlecap.EPaper.Services/*.csproj ./Bottlecap.EPaper.Services/

RUN dotnet restore ./GoogleCloudStorage/Bottlecap.EPaper.GoogleCloudStorage.Api --verbosity quiet

COPY ./Bottlecap.EPaper.Services ./Bottlecap.EPaper.Services
COPY ./GoogleCloudStorage ./GoogleCloudStorage

FROM base AS builder

RUN dotnet publish ./GoogleCloudStorage/Bottlecap.EPaper.GoogleCloudStorage.Api/Bottlecap.EPaper.GoogleCloudStorage.Api.csproj -c Release -o /app/output/ --verbosity quiet

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as final

RUN groupadd -r app && useradd --no-log-init -r -g app app

WORKDIR /app

COPY --from=builder /app/output ./output

ENV PORT=8080
ENV ASPNETCORE_URLS=http://*:$PORT
EXPOSE $PORT

RUN chown -R app:app .
USER app

CMD ["dotnet", "./output/Bottlecap.EPaper.GoogleCloudStorage.Api.dll"]