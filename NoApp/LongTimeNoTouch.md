# Long time no touch

Usual issues when updating TremAn app after longer period of time.

## No UWPSDK installed or something

- Best is to install everything from Visual Studio Installer. 

## Certificate issue (APPX0108) on develop


> ##[error]C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Microsoft\VisualStudio\v17.0\AppxPackage\Microsoft.AppXPackage.Targets(4745,5): Error APPX0108: The certificate specified has expired. For more information about renewing certificates, see http://go.microsoft.com/fwlink/?LinkID=241478.

- develop uses its own certificate
- this needs to be provided to the build agent and is used in `- task: DownloadSecureFile@1`
- https://stackoverflow.com/questions/40126286/windows-store-app-test-certificate-expired

### Solution

- Open in Visual Studio
- Publish on Solution, Create App Packages, Sideload, Yes, Create certificate, (no password)
- That will create `TremAn_TemporaryKey.pfx` in the project folder
- Upload this file to the build agent
  - Pipelines->Library->Secure files
  - change the file
- Then grant access to the file in the pipeline (pipeline will prompt you to do so when arriving to the task)
- Change the certification thumbprint in csproj `<PackageCertificateThumbprint>NewThumbPrint</PackageCertificateThumbprint>`
  - Not sure how to obtain in, other than the pipeline will fail with proper error message: `Certificate does not match supplied signing thumbprint`

