FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
RUN mkdir -p /opt
COPY ["mca_light_extension_core/MCA light", "/opt"]
RUN  ["chmod", "+x", "/opt/MCA_light_linux_2.0"]
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["mca_light_extension_core/mca_light_extension_core.csproj", "mca_light_extension_core/"]
COPY ["mca_light_smo_nl_extended/mca_light_smo_nl_extended.csproj", "mca_light_smo_nl_extended/"]

RUN dotnet restore "mca_light_extension_core/mca_light_extension_core.csproj"
RUN dotnet restore "mca_light_smo_nl_extended/mca_light_smo_nl_extended.csproj"
COPY . .
WORKDIR "/src/mca_light_smo_nl_extended"
RUN dotnet build "mca_light_smo_nl_extended.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "mca_light_smo_nl_extended.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
#### Add application environment variables
#ENV AWS_ACCESS_KEY=
#ENV AWS_SECRET_ACCESS_KEY=
#ENV AWS_SESION_TOKEN=
ENV MCA_PATH=/opt
ENV MCA_SIMULATION_FLAGS=bl
ENV MCA_OUTPUT_EXTENSIONS=.txt;.csv
ENV AWS_LOADFLOW_QUEUE_URL=https://sqs.eu-central-1.amazonaws.com/{ACCOUNT_NR_PLACEHOLDER}/gridmaster_gasunie_loadflow
ENV AWS_OUTPUT_VALIDATION_QUEUE_URL=https://sqs.eu-central-1.amazonaws.com/{ACCOUNT_NR_PLACEHOLDER}/gridmaster_output_validation
ENV AWS_LOADFLOW_QUEUE_TIMEOUT=30
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mca_light_smo_nl_extended.dll"]