## Durin Media Azure Deployment Guide

### Deploy Connections
Click below to deploy connection objects in the azure. Once you click on the button below, you will be redirected to azure portal. Please select the subscription and resource group(prefer to create new), click on review + create, then hit on create button. Sooner the deployment will start happen for connecton object. Now wait unless it completes.

[![Deploy Connection](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fv-sosah%2FAzureMediaDeployment%2Fdurinmedia-azure-function%2Fdeployment%2Fdeployconnection.json)



### Authorize connections
Once the deployment of connection completes, you should see <b>"Go to resource group" </b> Button. Click on that button, it should take you to page where all created object will listed. follow below steps
* See the </b>commmondataservice</b> connection. click on it-> see at left pane-> click on Edit Api connection  -> Authorize-> enter cds tenant admin credential. After successfull authentication, Please save.
* Similarly select <b>azureeventgrid</b>-> Edit Api Connection -> Authorize with you own Azure logged-in credential.
* Same with <b>office365</b>. Please authorize with the user credential which can be use for sending Emails.
* Same with <b>Virustotal</b>. Please authorize with "x-api_key". Check out [Virus Total](https://www.virustotal.com/) for api key.

### Deploy Azure components

Click the below button, it will again  redirect you to azure portal, Please select the <b>same subscription and resource group</b> and enter the CDS Org Unique name.
this unique name can be found in your CDS tenant(login to cds) -> Setting-> Customization -> Developer Resource -> Unique name. Attached the image for the reference

[![Deploy Component](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fv-sosah%2FAzureMediaDeployment%2Fdurinmedia-azure-function%2Fdeployment%2Fdeploy.json)

### Mac Client Connection configuration

Once the azure component deployment completes, Expand the Deployment details Secction ->  Click on the MacClient Connection configuration -> Left pane "output link" -> Copy the configuration. you will need this configuration, while setting up your Mac Client.
