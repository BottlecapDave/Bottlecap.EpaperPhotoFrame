FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base

WORKDIR /app
ARG environment
ENV ENVIRONMENT=$environment

ENV PORT=3000
ENV ASPNETCORE_URLS=http://*:$PORT
EXPOSE $PORT

COPY ./Bottlecap.EPaper.GoogleCloudStorage.Api/*.csproj ./Bottlecap.EPaper.GoogleCloudStorage.Api/
COPY ./Bottlecap.EPaper.GoogleCloudStorage.Services/*.csproj ./Bottlecap.EPaper.GoogleCloudStorage.Services/

RUN dotnet restore ./Bottlecap.EPaper.GoogleCloudStorage.Api --verbosity quiet

COPY ./ ./

FROM base AS builder

RUN dotnet publish ./Bottlecap.EPaper.GoogleCloudStorage.Api/Bottlecap.EPaper.GoogleCloudStorage.Api.csproj -c Release -o /app/output/ --verbosity quiet

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as final

RUN groupadd -r app && useradd --no-log-init -r -g app app

WORKDIR /app

COPY --from=builder /app/output ./output

ENV PORT=3000
ENV ASPNETCORE_URLS=http://*:$PORT
EXPOSE $PORT

RUN chown -R app:app .
USER app

CMD ["dotnet", "./output/Bottlecap.EPaper.GoogleCloudStorage.Api.dll"]