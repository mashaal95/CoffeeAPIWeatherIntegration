# CoffeeAPIMinimal

This is a project where a minimal .Net 6 Core API has been built to act as an imaginary coffee machine. It uses Redis to use a cache based counter to track the number of times the API has been called. 

## Installation

Clone the project and install all the dependencies on VS 2022. You will need to use a Redis docker instance to use Redis, the command for which has been given below.

```d
docker run --name redis -p 6379:6379 -d redis:latest
```
The command will install a docker Redis instance with 6379 as the port number and "Redis" as the instance name.
## Usage

```powershell
dotnet run 
```
## Secrets Manager
The application uses a Secret Manager for the OpenWeather API key. To use user secrets, run the following command in the project directory

```bash
dotnet user-secrets init
```

### Set secret

Define an app secret consisting of a key and its value. The secret is associated with the project's UserSecretsId value. For example, run the following command from the directory in which the project file exists:
```bash
dotnet user-secrets set "OpenWeatherApi:ServiceApiKey" "YOUR_API_KEY_HERE"
```
After this, you should be able to use the OpenWeatherAPI. 
