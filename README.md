## Durin Media Azure Deployment Guide

### Deploy Connections
Click below to deploy connection objects in the azure. Once you click on the button below, you will be redirected to azure portal. Please select the subscription and resource group(prefer to create new), click on review + create, then hit on create button. Sooner the deployment will start happen for connecton object. Now wait unless it completes.

[![Deploy Components](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fv-sosah%2FAzureMediaDeployment%2Fdurinmedia-azure-function%2Fdeployment%2Fdeployconnection.json)



### Authorize connections
Once the deployment of connection completes, you should see <b>"Go to resource group" </b> Button. Click on that button, it should take you to page where all created object will listed. follow below steps
* See the </b>commmondataservice</b> connection. click on it-> see at left pane-> click on Edit Api connection  -> Authorize-> enter cds tenant admin credential. After successfull authentication, Please save.
* Similarly select <b>azureeventgrid</b>-> Edit Api Connection -> Authorize with you own Azure logged-in credential.
* Same with <b>office365</b>. Please authorize with the user credential which can be use for sending Emails.
* Same with <b>Virustotal</b>. Please authorize with "x-api_key". Check out [Virus Total](https://www.virustotal.com/) for api key.

### Deploy Azure components

[![Deploy Connection](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fv-sosah%2FAzureMediaDeployment%2Fdurinmedia-azure-function%2Fdeployment%2Fdeploy.json)
