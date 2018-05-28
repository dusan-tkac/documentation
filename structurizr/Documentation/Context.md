<!-- # Hidden by Structurizr -->
<!-- ## Also hidden by Structurizr -->

### Enterprise context

This document describes some of the software systems used by IPRO Supply Chain Planning (SCP).

*Note: The "Software System" and other terms used in this document are terms adopted from the [C4 model](https://c4model.com/) - a model for describing software architecture.*

Following diagram provides an overview of majority of related software systems used by SCP:

![Enterprise context diagram](embed:EnterpriseContext)

The SCP "is in the business" of answering question (most often) related to supply chain by creating and running models and analyzing and interpreting results of these model runs.

Software systems displayed in the diagram above support users in these tasks.

**TIBCO Spotfire** is the main tool used by SCP to visualize and analyze available data (e.g. results of model runs).

**Runs Controller** is a system that allows for execution of model runs. It distributes model runs to available resources and keeps track of executed, executing and queued model runs.

**Assumption Management System (AMS)** allows user to configure model runs:
*select model version
*define assumptions that are used as model inputs
*configure other model run parameters

**Model Outputs Storage** stores results of model runs so they are available for further analysis (using Spotfire or other tools)

**Analytical Data Storage** in the context of this document is a collection of databases that store "raw" model inputs and other data required for questions from the business.

Another two software systems - **Hangfire (AMS Service)** and **Control Framework** have a similar roles of executing background tasks - either recurring or on demand. Historically, Control Framework was created first and implemented as a set of SSIS (SQL Server Integration Services) packages, because most of the identified background tasks were very data (and database) centric. Hangfire (AMS Service) was created later to support tasks that were not easily implementable in SSIS.

It is worth noting that some **users outside of SCP** have (or will have) an access to some dashboards in SCP Spotfire and that large part of data managed by **Analytical Data Store** are imported from **systems external to SCP**.

The rest of this document describes AMS, Hangfire and Runs Controller in greater details.

### AMS and Hangfire System Context

![AMS system context diagram](embed:AMSSystemContext)

AMS and Hangfire could really be considered as a single software system. Hangfire deployment unit (windows service) is even called "AMS Service".

Indeed, many of background tasks are just "offloaded" by AMS to Hangfire.
However, there are also some tasks (e.g. model outputs registration and retention policy) that are really not that much related to AMS - hence the separation to two software systems.

Although for the purpose of this document they are formally separated, they also share some software components and most of their code base and that is why they are documented in same sections.

AMS is the main system for storing and managing assumptions which are used as inputs for model runs.
It also allows user to manage model versions and specify model run parameters.

AMS provides a user interface (web application) that allow users to edit "states" - collections of values that used as model inputs manually using generic and specialized tools. It allows for for bulk import of values from Excel spreadsheets and Analytical Data Store.
AMS also provides a user interface for Control Framework configuration.
User create their "submissions" (model runs) in AMS and AMS displays basic information about queued and running submissions.

Long-running background actions triggered in AMS (like data imports from Analytical Data Store and creation of submissions in Runs Controller) are executed by Hangfire.

Hangfire (AMS Service) also executes recurring tasks like model outputs registration and retention policy.

Hangfire (AMS Service) use [Hangfire](https://www.hangfire.io/) framework.
The service implementation shares its code base with AMS; AMS web application also hosts a user interface for Hangfire administration. Permanent job storage for Hangfire is a part of the AMS database (separate database schema).

Hangfire also sends notifications to AMS about finished background jobs which AMS created. AMS then displays notification to a user that initiated the task.

### Runs Controller System Context

![Runs Controller system context diagram](embed:RunsControllerSystemContext)

Runs Controller's main function is to execute "submissions".

In general, a submission consist of number of packages which share the same executable program and most of their inputs. Packages within the submission typically only differ in a "seed number" - a number which is used to seed random number generators within the executable.

Runs Controller currently supports an execution of following types of submissions:

* **P2C models (AnyLogic)** - an executable is a Java program created using AnyLogic; input is an AMS state + model version parameters + a seed number
* **Arena models (RunInfoTextFile)** - an executable a Rockwell Software Arena model; inputs for these models are not managed by AMS
* **P2C post-processing (PostProcessing)** - an executable is R script; each post-processing submission has a single package; these submissions are created automatically - one for each "successful" P2C submissions
* **P2C quick tests (AnyLogicQuickTest)** - essentially same as P2C submissions; however there is a single package per submission. These submissions are used to "quickly" test new P2C model versions

Runs Controller is a distributed system with one distributor and a number of client nodes. Distributor distributes packages to clients which execute them. Distributor then collects data about running packages.
When clients' capacity is not sufficient to execute or created packages distributor maintains a queue of packages to be executed.

Runs Controller also provides user interface which allow users to monitor and control their submissions and/or individual packages. Users can cancel their submissions and packages and change their priority.
Package priority controls its place in the queue.
Users can check standard output of their running packages and their progress.

Apart of standard output which is displayed in Runs Controller user interface, P2C models also produce output files which are copied to Model Outputs Storage.