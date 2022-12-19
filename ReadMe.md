What is Mockdoor
==============
Mockdoor is fundamentally a mocking and debugging service designed for a large number of microservices and environments. What makes Mockdoor different than the many other mocking services that exist its its self learning, multi tenancy and scalability features. With self learning and multi tenancy even a mature existing project can be brought in and mocked out in a few minutes.

[![Build Status](https://dev.azure.com/ace90210ace0586/Mockdoor/_apis/build/status/mymockdoor.mockdoor?branchName=main)](https://dev.azure.com/ace90210ace0586/Mockdoor/_build/latest?definitionId=9&branchName=main)

# How to run 

To run Mockdoor you can run the following command

```docker run -d -p 44304:80 --name mockdoor  mockdoor/server```

To run Mockdoor against a Microsoft SQL Server (replacing <<connection string>> with your connection string

```docker run -d -p 44304:80 --name mockdoor -e DeploymentConfiguration__DatabaseConfig__Provider=1 -e DeploymentConfiguration__DatabaseConfig__ConnectionString="<<connection string>>" mockdoor/server```

Once running navigate to http://localhost:44304

Dockhub: [https://hub.docker.com/r/mockdoor/server](https://hub.docker.com/r/mockdoor/server)
For more details see [https://www.mymockdoor.com/](https://www.mymockdoor.com/)