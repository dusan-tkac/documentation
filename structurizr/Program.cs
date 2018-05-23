using System;
using System.IO;
using OpenSoftware.DgmlTools.Model;
using OpenSoftware.Structurizr.Dgml;
using Structurizr;
using Structurizr.Api;
using Structurizr.Documentation;

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


            Model model = workspace.Model;

            #region Users
            Person amsUser = model.AddPerson("AMS User", "Modeller, Data Analyst or other users.");
            #endregion

            #region Software systems

            SoftwareSystem amsSoftwareSystem = model.AddSoftwareSystem("AMS", "Assumption Management System\n\n" +
            "Stores and allows user to manage model versions and model inputs (assumptions)");

            SoftwareSystem runsControllerSoftwareSystem = model.AddSoftwareSystem("Runs Controller", "Executes model submissions and post-processing tasks and allows user to monitor and manage submisions");
            amsUser.Uses(runsControllerSoftwareSystem, "Monitors and manages submissions");

            SoftwareSystem modelOutputsStorageSoftwareSystem = model.AddSoftwareSystem("Model Outputs Storage", "Databases and shared file system that stores outputs of model runs");

            SoftwareSystem hangfireSoftwareSystem = model.AddSoftwareSystem("Hangfire", "Recurring and background jobs processing\n\n" +
            "Executes long-running background tasks (like submission creation in Runs Controller) " +
            "and recurring tasks (like model outputs registration and retention policy)");

            #endregion

            #region Relationships (what uses what)

            amsUser.Uses(amsSoftwareSystem, "Manages model versions, model inputs and creates submissions");

            amsSoftwareSystem.Uses(runsControllerSoftwareSystem, "Fetches submission status");
            amsSoftwareSystem.Uses(hangfireSoftwareSystem, "Triggers submission creation in Runs Controller");

            runsControllerSoftwareSystem.Uses(modelOutputsStorageSoftwareSystem, "Generates model outputs");

            hangfireSoftwareSystem.Uses(modelOutputsStorageSoftwareSystem, "Registers created model outputs, applies retention policy");
            hangfireSoftwareSystem.Uses(runsControllerSoftwareSystem, "Creates model submissions and post-processing submissions");

            #endregion

            #region Views (diagrams)

            // define some views (the diagrams you would like to see)
            ViewSet views = workspace.Views;

            SystemContextView amsSystemContextView = views.CreateSystemContextView(amsSoftwareSystem, "AMSSystemContext",
                "AMS System Context diagram.");
            amsSystemContextView.PaperSize = PaperSize.A4_Landscape;
            amsSystemContextView.AddAllSoftwareSystems();
            amsSystemContextView.AddAllPeople();

            // add some styling
            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });

            #endregion

            #region Documentation

            // add some documentation
            StructurizrDocumentationTemplate template = new StructurizrDocumentationTemplate(workspace);

            var documentationFolderPath = Path.Combine(AppContext.BaseDirectory, "Documentation");
            var amsDocPath = Path.Combine(documentationFolderPath, "AMSSoftwareSystem.md");
            var amsDocFi = new FileInfo(amsDocPath);

            template.AddContextSection(amsSoftwareSystem, amsDocFi);

            #endregion

            #region Upload and generate local DGML

            UploadWorkspaceToStructurizr(workspace);

            // Convert to DGML
            var dgml = workspace.ToDgml();
            // Write to file
            dgml.WriteToFile("c4model.dgml");

            #endregion
        }

        private static void UploadWorkspaceToStructurizr(Workspace workspace)
        {
            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
    }
}