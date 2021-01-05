## Durin Media Azure Deployment Guide

### Deploy Connections
Click below to create connection objects in the azure. Once you click on the button below, you will be redirected to azure portal. Please select the subscription and resource group(prefer to create new). 
[![Deploy Components](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fv-sosah%2FAzureMediaDeployment%2Fdurinmedia-azure-function%2Fdeployment%2Fdeployconnection.json)

### Authorize connections
* after first steps, the connection object would have been created. Please visit the portal.azure.com -> select subscription-> select resource group.
* See the commmondataservice connection. click on it, edit api connection -> autorize-> enter cds tenant admin credential-> after authentication , please save
* select your subscription and resource group. 

[![Deploy Connection](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fv-sosah%2FAzureMediaDeployment%2Fdurinmedia-azure-function%2Fdeployment%2Fdeploy.json)
