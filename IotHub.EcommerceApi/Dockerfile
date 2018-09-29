#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 50468
EXPOSE 44383

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["IotHub.EcommerceApi/IotHub.EcommerceApi.csproj", "IotHub.EcommerceApi/"]
RUN dotnet restore "IotHub.EcommerceApi/IotHub.EcommerceApi.csproj"
COPY . .
WORKDIR "/src/IotHub.EcommerceApi"
RUN dotnet build "IotHub.EcommerceApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IotHub.EcommerceApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IotHub.EcommerceApi.dll"]