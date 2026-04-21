FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . . 
RUN find /app -name "*.csproj"
RUN dotnet restore SecretAgentGadgetLab/SecretAgentGadgetLab/SecretAgentGadgetLab.csproj
RUN dotnet publish SecretAgentGadgetLab/SecretAgentGadgetLab/SecretAgentGadgetLab.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "SecretAgentGadgetLab.dll"]
