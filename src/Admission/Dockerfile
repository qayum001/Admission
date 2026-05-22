FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Admission/Admission.Api/Admission.Api.csproj", "src/Admission/Admission.Api/"]
COPY ["src/Admission/Admission.FileStorage/Admission.FileStorage.csproj", "src/Admission/Admission.FileStorage/"]
COPY ["src/Admission/Admission.Infrastructure/Admission.Infrastructure.csproj", "src/Admission/Admission.Infrastructure/"]
COPY ["src/Admission/Admission.Persistence/Admission.Persistence.csproj", "src/Admission/Admission.Persistence/"]
COPY ["src/Admission/Admission.Messaging/Admission.Messaging.csproj", "src/Admission/Admission.Messaging/"]
COPY ["src/MessagingContracts/MailContracts/MailContracts.csproj", "src/MessagingContracts/MailContracts/"]
COPY ["src/Admission/Admission.DictionaryClient/Admission.DictionaryClient.csproj", "src/Admission/Admission.DictionaryClient/"]
COPY ["src/Admission/Admission.Application/Admission.Application.csproj", "src/Admission/Admission.Application/"]
COPY ["src/Admission/Admission.Domain/Admission.Domain.csproj", "src/Admission/Admission.Domain/"]

RUN dotnet restore "src/Admission/Admission.Api/Admission.Api.csproj"

COPY ["src/Admission/Admission.Api/", "src/Admission/Admission.Api/"]
COPY ["src/Admission/Admission.FileStorage/", "src/Admission/Admission.FileStorage/"]
COPY ["src/Admission/Admission.Infrastructure/", "src/Admission/Admission.Infrastructure/"]
COPY ["src/Admission/Admission.Persistence/", "src/Admission/Admission.Persistence/"]
COPY ["src/Admission/Admission.Messaging/", "src/Admission/Admission.Messaging/"]
COPY ["src/MessagingContracts/MailContracts/", "src/MessagingContracts/MailContracts/"]
COPY ["src/Admission/Admission.DictionaryClient/", "src/Admission/Admission.DictionaryClient/"]
COPY ["src/Admission/Admission.Application/", "src/Admission/Admission.Application/"]
COPY ["src/Admission/Admission.Domain/", "src/Admission/Admission.Domain/"]

WORKDIR /src/src/Admission/Admission.Api
RUN dotnet publish "Admission.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends wget \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Admission.Api.dll"]
