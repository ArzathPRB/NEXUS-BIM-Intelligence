using System;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace NEXUS
{
    [Transaction(TransactionMode.Manual)]
    public class NexusHelloCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uiDoc =
                commandData.Application.ActiveUIDocument;

            if (uiDoc == null)
            {
                message = "Open a Revit project first.";
                return Result.Cancelled;
            }

            Document doc = uiDoc.Document;

            TaskDialog selector = new TaskDialog(
                "NEXUS | Select Viewer");

            selector.MainInstruction =
                "Choose your BIM viewing environment";

            selector.MainContent =
                "A — Live Revit Viewer\n" +
                "Inspect the active Revit model inside Revit.\n\n" +
                "B — Exported Model Viewer\n" +
                "Inspect an exported model independently in a browser.";

            selector.AddCommandLink(
                TaskDialogCommandLinkId.CommandLink1,
                "A | LIVE REVIT VIEWER");

            selector.AddCommandLink(
                TaskDialogCommandLinkId.CommandLink2,
                "B | EXPORTED MODEL VIEWER");

            selector.CommonButtons = TaskDialogCommonButtons.Cancel;

            TaskDialogResult result = selector.Show();

            if (result == TaskDialogResult.CommandLink1)
            {
                NexusDashboardPane.Instance?.Control.Refresh(doc);

                commandData.Application
                    .GetDockablePane(NexusDashboardPane.PaneId)
                    .Show();

                return Result.Succeeded;
            }

            
if (result == TaskDialogResult.CommandLink2)
{
    string exportFolder = Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments),
        "NEXUS",
        "Exports"
    );

    Directory.CreateDirectory(exportFolder);

    string fileName =
        Path.GetFileNameWithoutExtension(doc.Title);

    if (string.IsNullOrWhiteSpace(fileName))
        fileName = "NEXUS_Model";

    try
    {
        IFCExportOptions options = new IFCExportOptions();

        bool exported;

using (Transaction tx =
    new Transaction(doc, "NEXUS IFC Export"))
{
    tx.Start();

    exported = doc.Export(
        exportFolder,
        fileName,
        options
    );

    tx.Commit();
}

        string exportPath = Path.Combine(
            exportFolder,
            fileName + ".ifc"
        );

        TaskDialog.Show(
            "NEXUS | IFC Export",
            exported
                ? "IFC export successful!\n\nLocation:\n" + exportPath
                : "IFC export was unsuccessful."
        );
    }
    catch (Exception ex)
    {
        TaskDialog.Show(
            "NEXUS | Export Error",
            ex.Message
        );
    }

    return Result.Succeeded;
}

            return Result.Cancelled;
        }
    }
}
