using System;
using System.IO;
using OpenSoftware.DgmlTools.Model;
using OpenSoftware.Structurizr.Dgml;
using Structurizr;
using Structurizr.Api;
using Structurizr.Documentation;
using Structurizr.IO.PlantUML;

namespace structurizr
{
    /// <summary>
    /// This is a simple example of how to get started with Structurizr for .NET.
    /// </summary>
    class Program
    {
        #region Id, Api key and secret

        private const long WorkspaceId = 39062;
        private const string ApiKey = "648b7f84-9f44-4358-85f0-697658d74bfe";
        private const string ApiSecret = "5e7ba796-d848-40dc-b26a-1b1eb0bb1011";

        #endregion

        static void Main(string[] args)
        {
            // a Structurizr workspace is the wrapper for a software architecture model, views and documentation
            Workspace workspace = new Workspace("Supply Chain Planning", "Model of software systems used by Supply Chain Planning.");

            #region Model

            Model model = workspace.Model;

            #region Users

            Person scpUser = model.AddPerson(Location.Internal, "SCP User", "Modeller, Data Analyst or other users.");
            Person adminUser = model.AddPerson(Location.Internal, "Administrator", "SCP Administrators.");
            Person externalUser = model.AddPerson(Location.External, "External User", "Other users outside of SCP");

            #endregion

            #region Software systems

            SoftwareSystem amsSoftwareSystem = model.AddSoftwareSystem(
                Location.Internal,
                "AMS",
                "Assumption Management System\n\nStores and allows user to manage model versions and model inputs (assumptions)");

            SoftwareSystem runsControllerSoftwareSystem = model.AddSoftwareSystem(
                Location.Internal,
                "Runs Controller",
                "Executes model submissions and post-processing tasks and allows user to monitor and manage submissions");

            SoftwareSystem modelOutputsStorageSoftwareSystem = model.AddSoftwareSystem(
                Location.Internal,
                "Model Outputs Storage",
                "Databases and shared file system that stores outputs of model runs");

            SoftwareSystem hangfireSoftwareSystem = model.AddSoftwareSystem(
                Location.Internal,
                "Hangfire",
                "Recurring and background jobs processing\n\n" +
                "Executes long-running background tasks (like submission creation in Runs Controller) " +
                "and recurring tasks (like model outputs registration and retention policy)");

            SoftwareSystem analyticalDataStoreSoftwareSystem = model.AddSoftwareSystem(
                Location.Internal,
                "Analytical Data Store",
                "This includes all databases and other data stores that can be used as data sources for Spotfire or AMS");

            SoftwareSystem spotfireSoftwareSystem = model.AddSoftwareSystem(
                Location.Internal,
                "Spotfire",
                "TIBCO Spotfire dashboards visualize data (model outputs and other available data)");

            SoftwareSystem externalDataSourcesSoftwareSystem = model.AddSoftwareSystem(
                Location.External,
                "External Data Source",
                "All other source of data external to SCP from which import data");

            SoftwareSystem controlFrameworkSoftwareSystem = model.AddSoftwareSystem(
                Location.Internal,
                "Control Framework",
                "Collection of configurable SSIS packages running as SQL jobs perfroming data-centric background and reccuring tasks");

            SoftwareSystem p2cModelDevelopmentSoftwareSystem = model.AddSoftwareSystem(
                Location.Internal,
                "P2C Model Development",
                "Toolset for development and testing of P2C model");

            #endregion

            #region Containers

            #region AMS Containers

            var amsLightswitchWebApplicationContainer = amsSoftwareSystem.AddContainer("LightSwitch Web Application", 
                "Main navigation, browse and edit screens, shell for some custom JavaScript controls", 
                "LightSwitch HTML Client (SPA), Knockout JS");

            var amsAspNetMvcWebApplicationContainer = amsSoftwareSystem.AddContainer("MVC Web Application",
                "Dashboard, Type system versions, State Validation, Key Inputs, State Merging, Submission Progress",
                "ASP.NET MVC, Kendo UI, Knockout JS");

            var amsApiApplicationContainer = amsSoftwareSystem.AddContainer("AMS API Application", "AMS back-end", "ASP.NET Web API, Entity Framework, LightSwitch Server");

            var amsDatabaseContainer = amsSoftwareSystem.AddContainer("AMS Database", "Stores project hierarchy, assumptions, type system definition and other data managed by AMS", "MS SQL Server 2016");

            var amsCacheContainer = amsSoftwareSystem.AddContainer("AMS Distributed Cache", "In-memory cache for frequently accessed and hard-to-read data; also used for messaging between Hangfire and AMS", "Redis, Windows Service");

            #endregion

            #region Hangfire Containers

            var hangfireDashboardContainer = hangfireSoftwareSystem.AddContainer("Hangfire Dashboard", "User interface for Hangfire monitoring and management", "Web Application");

            var hangfireServiceContainer = hangfireSoftwareSystem.AddContainer("Windows Service", "Executes background tasks and recurrent jobs", ".Net Framework Windows Application, Shares code base with AMS API Application");

            var hangfireDatabaseContainer = hangfireSoftwareSystem.AddContainer("Hangfire Database", "Stores hangfire jobs, results and other Hangfire data", "MS SQL Server 2016");

            var hangfireMessageQueuesContainer = hangfireSoftwareSystem.AddContainer("Message Queues", "Queues for jobs to be executed by Hangfire", "Microsoft Message Queues");

            #endregion

            #region Control Framework Containers

            var controlFrameworkDatabaseContainer = controlFrameworkSoftwareSystem.AddContainer("ControlFrameworkDB", "Control Framework database", "MS SQL Server 2016 database");
            //var controlFrameworkJobs = 

            #endregion

            #region Runs Controller Containers

            var rcDistributorContainer = runsControllerSoftwareSystem.AddContainer("Runs Controller Distributor", 
                "Maintains package queue; distributes packages to clients; implements APIs for submission creation and portal", 
                ".Net, C#, Windows Service");

            var rcClientContainer = runsControllerSoftwareSystem.AddContainer("Runs Controller Client", "Executes packages", ".Net, C#, Windows Service");

            var rcDatabaseContainer = runsControllerSoftwareSystem.AddContainer("Runs Controller Database", "Stores Runs Controller data", "SQL Server 2016 database");

            var rcReportingAPIServiceContainer = runsControllerSoftwareSystem.AddContainer("RunsController Reporting API Service", "Provides APIs for reporting on Runs Controller data", "ASP.NET Core, Windows Service");

            var rcCommandLineToolContainer = runsControllerSoftwareSystem.AddContainer("Runs Controller Command Line Tool", 
                "Command Line interface to Runs Controller Distributor; allows for creation of Arena packages", 
                ".Net, C#, console application");

            var rcSubmissionToolContainer = runsControllerSoftwareSystem.AddContainer("Runs Controller Submission Tool", 
                "The tools for creation of Arena submissions", 
                "C# Windows Forms application, ClickOnce deployment");

            var rcNotificationClientContainer = runsControllerSoftwareSystem.AddContainer("Runs Controller Notification Client", 
                "Monitors user's submissions and displays notification when submission is finished", 
                "C# Windows Forms application");

            var rcPortalContainer = runsControllerSoftwareSystem.AddContainer("Runs Controller Portal", "Runs Controller user interface", "ASP.NET MVC web application");

            #endregion

            #region Analytical Data Store Containers

            var adsDatabaseContainer = analyticalDataStoreSoftwareSystem.AddContainer("AnalyticalDataStore", "ADS database", "MS SQL Server 2016 database");
            var actualsTimeSeriesDatabaseContainer = analyticalDataStoreSoftwareSystem.AddContainer("Actuals_TimeSeries", "Actuals Time Series database", "MS SQL Server 2016 database");
            var projectDatabaseContainer = analyticalDataStoreSoftwareSystem.AddContainer("Project", "Project database", "MS SQL Server 2016 database");
            var sckbDatabaseContainer = analyticalDataStoreSoftwareSystem.AddContainer("SCKBDatabase", "SCKB database", "MS SQL Server 2016 database");
            var sckbReportingDatabaseContainer = analyticalDataStoreSoftwareSystem.AddContainer("SCKBReportingDB", "SCKB Reporting database", "MS SQL Server 2016 database");

            #endregion

            #region Model Outputs Storage Containers

            var modelOutputsFileSharesContainer = modelOutputsStorageSoftwareSystem.AddContainer("Model Outputs File Shares", "Store model output files and post-processed data", "File Share");
            var modelOutputsKpiDatabaseContainer = modelOutputsStorageSoftwareSystem.AddContainer("ModelOutputs_KPI", "Model Outputs KPI database", "MS SQL Server 2016 database");
            var modelOutputsTimeSeriesDatabaseContainer = modelOutputsStorageSoftwareSystem.AddContainer("ModelOutputs_TimeSeries", "Model Outputs Time Series database", "MS SQL Server 2016 database");
            var modelOutputsProcessDatabaseContainer = modelOutputsStorageSoftwareSystem.AddContainer("ModelOutputsProcess", "Model Outputs Process database", "MS SQL Server 2016 database");

            #endregion

            #region Deployment Nodes

            var amsWebServer = model.AddDeploymentNode("IORPER-WEB01", "AMS Web Server", "Windows Server 2012 R2");

            amsWebServer.Add(hangfireServiceContainer);
            amsWebServer.Add(hangfireMessageQueuesContainer);
            amsWebServer.Add(amsCacheContainer);

            var amsIis = amsWebServer.AddDeploymentNode("IIS", "Internet Information Services", "IIS 8.5");
            amsIis.Add(amsApiApplicationContainer);
            amsIis.Add(amsLightswitchWebApplicationContainer);
            amsIis.Add(amsAspNetMvcWebApplicationContainer);
            amsIis.Add(hangfireDashboardContainer);

            var clusterNode1 = model.AddDeploymentNode("IORPER-C02SQ01", "Node 1 of the failover cluster", "Windows Server 2012 R2");
            var node1databaseServer = clusterNode1.AddDeploymentNode("SQ2014SCAP01", "Database Server for Analytical Data Store", "SQL Server 2016");
            node1databaseServer.Add(adsDatabaseContainer);
            node1databaseServer.Add(controlFrameworkDatabaseContainer);
            node1databaseServer.Add(actualsTimeSeriesDatabaseContainer);
            node1databaseServer.Add(modelOutputsKpiDatabaseContainer);
            node1databaseServer.Add(modelOutputsTimeSeriesDatabaseContainer);
            node1databaseServer.Add(modelOutputsProcessDatabaseContainer);
            node1databaseServer.Add(projectDatabaseContainer);
            node1databaseServer.Add(sckbDatabaseContainer);
            node1databaseServer.Add(sckbReportingDatabaseContainer);

            var clusterNode2 = model.AddDeploymentNode("IORPER-C02SQ02", "Node 2 of the failover cluster", "Windows Server 2012 R2");

            var node2databaseServer = clusterNode2.AddDeploymentNode("SQ2014SCAP02", "Database Server for operational databases", "SQL Server 2016");

            var amsDatabase = node2databaseServer.AddDeploymentNode("AMS", "AMS database - the dbo schema contains AMS tables and vies, hangfire schema contains Hangfire tables", "SQL Server Database");
            amsDatabase.Add(amsDatabaseContainer);
            amsDatabase.Add(hangfireDatabaseContainer);

            var runsControllerDistributorServer = model.AddDeploymentNode("IOPL-S0044", "Runs Controller Distributor Server (alias UnityServer)", "Windows Server 2008 R2");
            runsControllerDistributorServer.Add(rcDistributorContainer);

            node2databaseServer.Add(rcDatabaseContainer);

            var gen8processingClients = model.AddDeploymentNode("Gen8 Runs Controller clients servers (Arena and AnyLogic)", "Gen8 Runs Controller client servers (16 processing slots)", "Windows Server 2012 R2", 31);
            var gen8postProcessingClients = model.AddDeploymentNode("Gen8 Runs Controller clients server (post-processing)", "Gen8 Runs Controller client servers (1 processing slot)", "Windows Server 2012 R2", 1);

            var gen9processingClients = model.AddDeploymentNode("Gen9 Runs Controller clients servers (Arena and AnyLogic)", "Gen8 Runs Controller client servers (24 processing slots)", "Windows Server 2012 R2", 24);
            var gen9postProcessingClients = model.AddDeploymentNode("Gen9 Runs Controller clients server (post-processing)", "Gen8 Runs Controller client servers (2 processing slot)", "Windows Server 2012 R2", 2);

            gen8processingClients.Add(rcClientContainer);
            gen8postProcessingClients.Add(rcClientContainer);
            gen9processingClients.Add(rcClientContainer);
            gen9postProcessingClients.Add(rcClientContainer);

            #endregion

            #endregion

            #region Relationships (what uses what)

            scpUser.Uses(amsSoftwareSystem, "Manages model versions, model inputs and creates submissions");

            scpUser.Uses(amsLightswitchWebApplicationContainer, "Manages model versions, model inputs and creates submissions");
            scpUser.Uses(amsAspNetMvcWebApplicationContainer, "Uses specialized AMS screens");

            scpUser.Uses(runsControllerSoftwareSystem, "Monitors and manages submissions");
            scpUser.Uses(spotfireSoftwareSystem, "Uses dashboards to visualize and analyze data");

            adminUser.Uses(amsSoftwareSystem, "Manages system settings and other protected data");

            adminUser.Uses(amsLightswitchWebApplicationContainer, "Manages system settings and other protected data");

            adminUser.Uses(runsControllerSoftwareSystem, "Manages Runs Controller setting (e.g. which clients are active)");
            adminUser.Uses(hangfireSoftwareSystem, "Manages Hangfire settings; can restart failed jobs etc.");
            adminUser.Uses(hangfireDashboardContainer, "Manages Hangfire settings; can restart failed jobs etc.");

            externalUser.Uses(spotfireSoftwareSystem, "Uses public dashboards or outputs produced by project teams using Spotfire");

            amsSoftwareSystem.Uses(runsControllerSoftwareSystem, "Fetches submission status");
            amsSoftwareSystem.Uses(hangfireSoftwareSystem, "Triggers submission creation in Runs Controller");
            amsSoftwareSystem.Uses(analyticalDataStoreSoftwareSystem, "Imports data from analytical data store that are used as model inputs (e.g. actual supply chain data)");
            amsSoftwareSystem.Uses(controlFrameworkSoftwareSystem, "Provides user interface for Control Framework configuration; references Control Framework messages as model inputs");

            controlFrameworkSoftwareSystem.Uses(analyticalDataStoreSoftwareSystem, "Imports (into) and transforms data");
            controlFrameworkSoftwareSystem.Uses(externalDataSourcesSoftwareSystem, "Imports data (from)");

            runsControllerSoftwareSystem.Uses(modelOutputsStorageSoftwareSystem, "Generates model outputs");

            hangfireSoftwareSystem.Uses(modelOutputsStorageSoftwareSystem, "Registers created model outputs, applies retention policy");
            hangfireSoftwareSystem.Uses(runsControllerSoftwareSystem, "Creates model submissions and post-processing submissions");

            analyticalDataStoreSoftwareSystem.Uses(amsSoftwareSystem, "Has read-only access to AMS data (project hierarchy, object model, reference data and objects)");
            analyticalDataStoreSoftwareSystem.Uses(externalDataSourcesSoftwareSystem, "Imports data from external systems.");

            spotfireSoftwareSystem.Uses(modelOutputsStorageSoftwareSystem, "Visualizes model outputs");
            spotfireSoftwareSystem.Uses(analyticalDataStoreSoftwareSystem, "Visualizes other available data");
            spotfireSoftwareSystem.Uses(runsControllerSoftwareSystem, "Reads submission details for environment performance dashboard");

            p2cModelDevelopmentSoftwareSystem.Uses(amsSoftwareSystem, "Exports model versions");

            amsLightswitchWebApplicationContainer.Uses(amsApiApplicationContainer, "Uses", "JSON/HTTP");
            amsAspNetMvcWebApplicationContainer.Uses(amsApiApplicationContainer, "Uses", "JSON/HTTP");

            amsApiApplicationContainer.Uses(amsDatabaseContainer, "Reads from and writes to", "ADO.Net");

            amsApiApplicationContainer.Uses(amsCacheContainer, "Reads from, writes to and invalidates data in", "StackExchange.Redis client");
            amsApiApplicationContainer.Uses(amsCacheContainer, "Subscribes to notifications from Hangfire", "Redis Pub/Sub");

            hangfireServiceContainer.Uses(hangfireMessageQueuesContainer, "Processes queued jobs");
            hangfireServiceContainer.Uses(hangfireDatabaseContainer, "Persists jobs data");
            hangfireServiceContainer.Uses(amsCacheContainer, "Reads from, writes to and invalidates in", "StackExchange.Redis client");
            hangfireServiceContainer.Uses(amsCacheContainer, "Publishes notifications for AMS", "Redis Pub/Sub");
            hangfireServiceContainer.Uses(amsDatabaseContainer, "Reads from and writes to", "ADO.Net");
            hangfireServiceContainer.Uses(modelOutputsFileSharesContainer, "Registers created model output files");
            hangfireServiceContainer.Uses(modelOutputsFileSharesContainer, "Removes old model output files");
            hangfireServiceContainer.Uses(modelOutputsKpiDatabaseContainer, "Removes old model outputs");
            hangfireServiceContainer.Uses(modelOutputsTimeSeriesDatabaseContainer, "Removes old model outputs");

            hangfireDashboardContainer.Uses(hangfireServiceContainer, "Provides user interface for");

            amsApiApplicationContainer.Uses(hangfireMessageQueuesContainer, "Enqueues and schedules jobs using");

            var failoverRelationship = clusterNode2.Uses(clusterNode1, "Failover to", "Windows Server Failover Cluster");

            rcDistributorContainer.Uses(rcDatabaseContainer, "Reads from and writes to", "ADO.Net");
            rcClientContainer.Uses(rcDistributorContainer, "Requests packages to execute; send package reports and changes in package status", "WCF, http");
            rcPortalContainer.Uses(rcDistributorContainer, "Reads data to display from and sends requests to manage data to", "WCF, http");
            scpUser.Uses(rcPortalContainer, "Manages submissions and packages");

            hangfireServiceContainer.Uses(rcDistributorContainer, "Creates AnyLogic packages and submissions, creates post-processing packages and submissions", "WCF, http");
            hangfireServiceContainer.Uses(rcReportingAPIServiceContainer, "Reads submission details to be cached in AMS and displayed in AMS", "http");

            spotfireSoftwareSystem.Uses(rcReportingAPIServiceContainer, "Reads submission details to be displayed on the environment performance dashboard", "http");

            amsApiApplicationContainer.Uses(rcDistributorContainer, "Updates Customer and Project details", "WCF, http");

            rcCommandLineToolContainer.Uses(rcDistributorContainer, "Creates Arena packages and submissions, updates customer and project", "WCF, http");

            rcSubmissionToolContainer.Uses(rcCommandLineToolContainer, "Uses it to creates Arena packages and submissions an to update customer and project");

            rcNotificationClientContainer.Uses(rcDistributorContainer, "Subscribes to user's submission changes", "WCF, http");

            rcReportingAPIServiceContainer.Uses(rcDatabaseContainer, "Reads persisted submission data", "ADO.Net");
            rcReportingAPIServiceContainer.Uses(rcDistributorContainer, "Reads queue details", "WCF, http");

            scpUser.Uses(rcNotificationClientContainer, "Gets notified when his submission is finished");
            scpUser.Uses(rcSubmissionToolContainer, "Creates Arena submissions using");

            rcClientContainer.Uses(modelOutputsFileSharesContainer, "Generates output files in");

            #endregion



            #endregion

            #region Views (diagrams)

            // define some views (the diagrams you would like to see)
            ViewSet views = workspace.Views;

            #region Enterprise context

            EnterpriseContextView enterpriseContextView = views.CreateEnterpriseContextView("EnterpriseContext", "Enterprise Context diagram.");

            enterpriseContextView.PaperSize = PaperSize.A4_Landscape;
            enterpriseContextView.AddAllSoftwareSystems();
            enterpriseContextView.AddAllPeople();

            #endregion

            #region AMS System Context

            SystemContextView amsSystemContextView = views.CreateSystemContextView(
                amsSoftwareSystem,
                "AMSSystemContext",
                "AMS System Context diagram.");
            amsSystemContextView.AddNearestNeighbours(amsSoftwareSystem);
            amsSystemContextView.AddNearestNeighbours(hangfireSoftwareSystem);

            #endregion

            #region Runs Controller System Context

            SystemContextView runsControllerSystemContextView = views.CreateSystemContextView(
                runsControllerSoftwareSystem,
                "RunsControllerSystemContext",
                "RunsController System Context diagram.");
            runsControllerSystemContextView.AddNearestNeighbours(runsControllerSoftwareSystem);

            #endregion

            #region AMS Container Diagram

            var amsContainerView = views.CreateContainerView(amsSoftwareSystem, "AMSContainerDiagram", "AMS container diagram");
            amsContainerView.AddAllContainers();

            amsContainerView.Add(scpUser);
            amsContainerView.Add(adminUser);

            amsContainerView.Add(hangfireServiceContainer);           
            amsContainerView.Add(hangfireMessageQueuesContainer);
            amsContainerView.Add(hangfireServiceContainer);

            #endregion

            #region Runs Controller Container Diagram

            var runsControllerContainerView = views.CreateContainerView(runsControllerSoftwareSystem, "RCContainerDiagram", "Runs Controller container diagram");
            runsControllerContainerView.AddAllContainers();
            runsControllerContainerView.Add(scpUser);

            runsControllerContainerView.Add(hangfireServiceContainer);
            runsControllerContainerView.Add(amsApiApplicationContainer);
            runsControllerContainerView.Add(spotfireSoftwareSystem);
            runsControllerContainerView.Add(modelOutputsFileSharesContainer);

            #endregion

            #region Hangfire Container

            var hangfireContainerView = views.CreateContainerView(hangfireSoftwareSystem, "HangfireContainerDiagram", "Hangfire container diagram");
            hangfireContainerView.AddAllContainers();

            hangfireContainerView.Add(adminUser);

            hangfireContainerView.Add(amsCacheContainer);
            hangfireContainerView.Add(amsDatabaseContainer);
            hangfireContainerView.Add(amsApiApplicationContainer);

            #endregion

            #region AMS Deployment

            var amsDeploymentView = views.CreateDeploymentView("AMSDeploymentDiagram", "AMS Deployment Diagram");

            amsDeploymentView.Add(amsWebServer);
            amsDeploymentView.Add(clusterNode2);
            amsDeploymentView.Add(clusterNode1);
            amsDeploymentView.Add(failoverRelationship);

            #endregion

            #region Runs Controller Deployment

            var runsControllerDeploymentView = views.CreateDeploymentView("RunsControllerDeploymentDiagram", "Runs Controller Deployment Diagram");

            runsControllerDeploymentView.Add(runsControllerDistributorServer);
            runsControllerDeploymentView.Add(gen8processingClients);
            runsControllerDeploymentView.Add(gen8postProcessingClients);
            runsControllerDeploymentView.Add(gen9processingClients);
            runsControllerDeploymentView.Add(gen9postProcessingClients);
            runsControllerDeploymentView.Add(clusterNode2);
            runsControllerDeploymentView.Add(clusterNode2);
            runsControllerDeploymentView.Add(clusterNode1);
            runsControllerDeploymentView.Add(failoverRelationship);

            #endregion

            #region Styles

            // add some styling
            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            // styles.Add(new RelationshipStyle(Tags.Relationship) {Routing = Routing.Orthogonal, Position = 30 });

            #endregion

            #endregion

            #region Documentation

            // add some documentation
            StructurizrDocumentationTemplate template = new StructurizrDocumentationTemplate(workspace);

            var documentationFolderPath = Path.Combine(AppContext.BaseDirectory, "Documentation");

            template.AddContextSection(amsSoftwareSystem, new FileInfo(Path.Combine(documentationFolderPath, "Context.md")));
            template.AddFunctionalOverviewSection(amsSoftwareSystem, new FileInfo(Path.Combine(documentationFolderPath, "FunctionalOverview.md")));
            template.AddDataSection(amsSoftwareSystem, new FileInfo(Path.Combine(documentationFolderPath, "Data.md")));
            template.AddPrinciplesSection(amsSoftwareSystem, new FileInfo(Path.Combine(documentationFolderPath, "Principles.md")));
            template.AddSoftwareArchitectureSection(amsSoftwareSystem, new FileInfo(Path.Combine(documentationFolderPath, "SoftwareArchitecture.md")));
            template.AddDeploymentSection(amsSoftwareSystem, new FileInfo(Path.Combine(documentationFolderPath, "Deployment.md")));
            template.AddOperationAndSupportSection(amsSoftwareSystem, new FileInfo(Path.Combine(documentationFolderPath, "OperationAndSupport.md")));

            #endregion

            #region Upload and generate local DGML

            // upload workspace to Structurizr (https://structurizr.com/)
            UploadWorkspaceToStructurizr(workspace);

            // Convert diagrams to DGML - dgml files can be opened using Visual Studio (extension is needed for VS 2017)
            var dgml = workspace.ToDgml();
            dgml.WriteToFile("c4model.dgml");

            // Convert diagrams to PlantUML format (http://plantuml.com/)
            StringWriter stringWriter = new StringWriter();
            PlantUMLWriter plantUMLWriter = new PlantUMLWriter();
            plantUMLWriter.Write(workspace, stringWriter);
            // content of the generated file can be visualized online (http://www.plantuml.com/plantuml/uml/)
            // or converted to image locally (using local PlantUML jar + Graphwiz or one of available VS Code extensions)
            File.WriteAllText("c4model_plant_UML.txt", stringWriter.ToString());

            #endregion
        }

        private static void UploadWorkspaceToStructurizr(Workspace workspace)
        {
            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}