What is Mockdoor
==============
Mockdoor is fundamentally a mocking and debugging service designed for a large number of microservices and environments. What makes Mockdoor different than the many other mocking services that exist its its self learning, multi tenancy and scalability features. With self learning and multi tenancy even a mature existing project can be brought in and mocked out in a few minutes.

[![Build Status](https://dev.azure.com/ace90210ace0586/Mockdoor/_apis/build/status/mymockdoor.mockdoor?branchName=main)](https://dev.azure.com/ace90210ace0586/Mockdoor/_build/latest?definitionId=9&branchName=main)

# How to run 
## Prerequisites
In order to run Mockdoor and these projects you need the following installed.

- dotnet 6.0 SDK
- A dotnet IDE/Editor such as Visual Studio, VS Code or Jet brains Rider
- If using Tye or Local IDE setup download and open the repository
- For easy environment running/setup you will need
    - Microsoft tye installed, see [https://github.com/dotnet/tye](https://github.com/dotnet/tye)
  
## Using Docker
To run Mockdoor you can run the following command

```docker run -d -p 44304:80 --name mockdoor  mockdoor/server```

To run Mockdoor against a Microsoft SQL Server (replacing <<connection string>> with your connection string

```docker run -d -p 44304:80 --name mockdoor -e DeploymentConfiguration__DatabaseConfig__Provider=1 -e DeploymentConfiguration__DatabaseConfig__ConnectionString="<<connection string>>" mockdoor/server```

Once running navigate to http://localhost:44304

## Additional Resources
- Dockhub: [https://hub.docker.com/r/mockdoor/server](https://hub.docker.com/r/mockdoor/server)
- Wiki: [https://wiki.mymockdoor.com/](https://wiki.mymockdoor.com/)
- API Documentation: [https://app.swaggerhub.com/apis-docs/MOCKDOOR/mockdoor_server/0.50.03](https://app.swaggerhub.com/apis-docs/MOCKDOOR/mockdoor_server/0.50.03)
- Mockdoor playground/demo environment: [https://github.com/mymockdoor/mockdoor.playground](https://github.com/mymockdoor/mockdoor.playground) 
- Website: [https://www.mymockdoor.com/](https://www.mymockdoor.com/)


## Using Microsoft Tye

Ensure you have Microsoft Tye installed.

to install run this command (for full instructions see [https://github.com/dotnet/tye](https://github.com/dotnet/tye))

```  dotnet tool install -g Microsoft.Tye --version "0.11.0-alpha.22111.1" ```

Note: Avast and possibly other Anti virus software,have been known to block the install of Microsoft Tye. It is an official microsoft project and you can verify it for yourself if desired in the above link on github. If this happens and assuming you are comfortable doing so just set an exclusion for Tye and retry the install.

To start Mockdoor using tye

``` tye run tye.yaml```

You can open Mockdoor at [https://localhost:44304/](https://localhost:44304/)

## Locally with IDE

- Open the Mockdoor.sln file in your IDE of choice
- Set Mockdoor.Server as your start up project 
- Start Mockdoor

Mockdoor should open automatically but you can manually open Mockdoor at [https://localhost:44304/](https://localhost:44304/)

# Pricing and Licence
For the current preview version (Pre V1) all tiers are **Free**. For personal use Mockdoor will remain free even after release but donations should be approciated.

See [Pricing](Pricing.md) for more details on planned tiers and pricing.

Or

<a style="background: #41a2d8;color: #fff;text-decoration: none;font-family: Verdana,sans-serif;display: inline-block;font-size: 16px;padding: 15px 38px;-webkit-border-radius: 2px;-moz-border-radius: 2px;border-radius: 2px;box-shadow: 0 1px 0 0 #1f5a89;text-shadow: 0 1px rgba(0, 0, 0, 0.3);" href="https://donorbox.org/mockdoor-development-donations?default_interval=o">Buy me a drink?</a>

The Licence agreement can be view [here](LICENCE.md)

# Contact and questions
For more details or any questions please email [mockdoor@gmx.co.uk](mailto:mockdoor@gmx.co.uk)
