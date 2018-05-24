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

            #endregion

            #region Relationships (what uses what)

            scpUser.Uses(amsSoftwareSystem, "Manages model versions, model inputs and creates submissions");
            scpUser.Uses(runsControllerSoftwareSystem, "Monitors and manages submissions");
            scpUser.Uses(spotfireSoftwareSystem, "Uses dashboards to visualize and analyze data");

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
            // amsSystemContextView.CopyLayoutInformationFrom(enterpriseContextView);
            // amsSystemContextView.PaperSize = PaperSize.A4_Landscape;
            amsSystemContextView.AddNearestNeighbours(amsSoftwareSystem);
            amsSystemContextView.AddNearestNeighbours(hangfireSoftwareSystem);
           
            #endregion

            #region Runs Controller System Context

            SystemContextView runsControllerSystemContextView = views.CreateSystemContextView(
                runsControllerSoftwareSystem, 
                "RunsControllerSystemContext",
                "RunsController System Context diagram.");
            runsControllerSystemContextView.AddNearestNeighbours(runsControllerSoftwareSystem);
            // runsControllerSystemContextView.CopyLayoutInformationFrom(enterpriseContextView);
           
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

            var amsDocPath = Path.Combine(documentationFolderPath, "Context.md");
            var amsDocFi = new FileInfo(amsDocPath);

            template.AddContextSection(amsSoftwareSystem, amsDocFi);

            var rcDocPath = Path.Combine(documentationFolderPath, "RunsControllerContext.md");
            var rcDocFi = new FileInfo(rcDocPath);

            // template.AddContextSection(runsControllerSoftwareSystem, rcDocFi);

            
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