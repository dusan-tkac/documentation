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

It is worth noting that some **users outside of SCP** have (or will) have and access to some dashboards in SCP Spotfire and that large part of data managed by **Analytical Data Store** are imported from **systems external to SCP**.

The rest of this document describes AMS, Hangfire and Runs Controller in greater details.

### AMS and Hangfire System Context

AMS and Hangfire could really be considered as a single software system. Hangfire deployment unit (windows service )is even called "AMS Service".

Indeed, many of background tasks are just "offloaded" by AMS to Hangfire.
However, there are also some tasks (e.g. model outputs registration and retention policy) that are really not that much related to AMS - hence the separation to two software systems.

Although for the purpose of this document they are formally separated, they also share some software components and most of their code base and that is why they are documented in same sections.

AMS is the main system for storing and managing assumptions which are used as inputs for model runs.
It also allows user to manage model versions and specify model run parameters.

Hangfire (AMS Service) ...

AMS creates submissions in Runs Controller - submission being a set of model runs with same inputs and model version with the only difference being a seed number used for random number generators.

![AMS system context diagram](embed:AMSSystemContext)

### Runs Controller System Context

![Runs Controller system context diagram](embed:RunsControllerSystemContext)