FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Mail/Admission.MailManager/Admission.MailManager.csproj", "src/Mail/Admission.MailManager/"]
COPY ["src/MessagingContracts/MailContracts/MailContracts.csproj", "src/MessagingContracts/MailContracts/"]

RUN dotnet restore "src/Mail/Admission.MailManager/Admission.MailManager.csproj"

COPY ["src/Mail/Admission.MailManager/", "src/Mail/Admission.MailManager/"]
COPY ["src/MessagingContracts/MailContracts/", "src/MessagingContracts/MailContracts/"]

WORKDIR /src/src/Mail/Admission.MailManager
RUN dotnet publish "Admission.MailManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Admission.MailManager.dll"]
