### AMS Architecture

![AMS Container Diagram](embed:AMSContainerDiagram)

AMS was initially created as a LightSwitch application with HTML client.

Later, parts of the user interface were implemented as ASP.NET MVC application.
LightSwitch HTML client screens were also customized by implementing custom controls - most of them using KnockoutJS and jQuery.

Apart of standard browsing and editing functionality for basic entities provided by LighSwitch, all other functionality is implemented in custom API controllers (and services and repositories). 

AMS stores data in Microsoft SQL Server database.

Data access for the LightSwitch part is done using LightSwitch data sources; most of the implemented repositories use Entity Framework 6 ORM.
There are also some stored procedures used where necessary.

AMS uses Redis as a out-of-process cache for frequently accessed and/or hard-to-read data.
This cache is shared with Hangfire service.

AMS runs in IIS and uses Windows Authentication.

Users are authorized using LightSwitch system of permissions and roles.

AMS web application also hosts Hangfire dashboard.

AMS provides an API for import of model version used by P2C developers.

Some of the AMS data are exposed through "public database views" and a server link to Analytical Data Store databases (SCKBReportingDB).

AMS enqueues background tasks to be executed by Hangfire using MSMQ (indirectly - this mechanism is hidden by Hangfire standard API).

Redis is also used for messaging from Hangfire to AMS. AMS subscribes to notifications sent by Hangfire using Redis Pub/Sub.

### Hangfire Architecture

![Hangfire Container Diagram](embed:HangfireContainerDiagram)

Hangfire is framework used for execution of background and recurrent jobs.

The dashboard - user interface for job management - is hosted by AMS web site.
The processing part is deployed as a windows service and shares its code base with AMS - it can use all services and repositories that exist in AMS.
It means that this processing part uses AMS database and cache in the same way as AMS.

Hangfire notifies AMS about some finished jobs using Redis Pub/Sub.

Scheduled jobs are received through MSMQ.

Job definitions and results are saved in the Hangfire database ("Hangfire" schema within AMS database).

### Runs Controller Architecture

![Runs Controller Container Diagram](embed:RCContainerDiagram)

Runs controller distributor runs as a windows service.
It is the only component that can save data to Runs Controller database.

Distributor implement a set of APIs (WCF) that

* expose data to Runs Controller Portal
* allow for package, submission and node management through the Portal
* allow fro creation of new packages, submissions, projects and customers - used by AMS, Hangfire, Command Line Tool
* publish notifications about finished submissions (to Notification client)
* receive updates from Runs Controller clients containing reports and status changes of running packages

Runs Controller client instances also run as windows services on client machines.
They may have a different configuration allowing them to process only certain types of packages and defining how many packages can be executed in parallel (number of slots).

Most of runs controller clients are currently configured to execute AnyLogic and Arena packages.

A few client are dedicated to post-processing package. The distinction was made due to different requirements for post-processing package execution - post-processing package requires more CPU and RAM.

Runs Controller clients request packages to execute from the distributor and send package updates back to distributor. Running packages also produce an output independently - in most case as (zip) files copied to models outputs shared drives when package execution is finished.

The reporting API was created to lift some work off the distributor.
This reporting API, which runs as a windows service on a separate machine, executes read-only queries on Runs Controller database and only queries status of Runs Controller Distributor queue when required.
The reporting API provides access to Runs Controller data without loading the distributor when not necessary.

The Submission Tool and Command Line Tool are legacy applications for creation of Arena submissions and packages.
The Submission Tool is a Windows Forms application with ClickOnce deployment which uses the Command Line Tool internally.

Notification tool starts automatically for all SIMCITY users as defined by group policy.