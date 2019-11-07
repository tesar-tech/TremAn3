# APPNAME__TEMPLATE

Version: VERSION__TEMPLATE

- Install Certificate (instructions below)
- [Download app installer](APPNAME__TEMPLATE.appinstaller)
- Open app installer. Click install:

  <img src="media/click_install.png" alt="install ce" height="200"/>
  
- If instalation completes with error:
  
   *Either you need a new certificate installed for this app package, or you need a new app package with trusted certificates. Your system administrator or the app developer can help. A certificate chain processed, but terminated in a root certificate which isn't trusted (0x800B0109)*
  
  Follow steps for installing certificate first:

## How to install certificate

- <a href="APPNAME__TEMPLATE_VERSION__TEMPLATE_Test/APPNAME__TEMPLATE_VERSION__TEMPLATE_x86.cer" download="APPNAME__TEMPLATE_VERSION__TEMPLATE_Test/APPNAME__TEMPLATE_VERSION__TEMPLATE_x86.cer">Download .cer file</a>
- Open certificate. You will be asked if you want to open this file. Click **Open**. 
- Click **Install Certificate**:
  
  <img src="media/install_cer.png" alt="install ce" height="300"/>

- Select **Local Machine** (not the default option!):
  
  <img src="media/local_mahine.png" alt="install ce" height="300"/>


- Click **Next** and **Yes** in next prompt.
- On Certificate store screen, select **Place all certificates in the following store** option and click **Browse**
  
  <img src="media/cert_store.png" alt="install ce" height="300"/>


- Select **Trusted People** and click **OK**. Then clik **Next**
  
  <img src="media/trusted_people.png" alt="install ce" height="200"/>


- Click **Finish**:
  
  <img src="media/finish.png" alt="install ce" height="300"/>


- Cerificate was successfully imported:
  
  <img src="media/import_succes.png" alt="success" height="100"/>

