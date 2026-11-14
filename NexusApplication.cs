
using Autodesk.Revit.UI;
using System;
using System.Reflection;

namespace NEXUS
{
    public class NexusApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "NEXUS";

            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                // Tab already exists
            }

            // Register NEXUS Dockable Dashboard
            application.RegisterDockablePane(
                NexusDashboardPane.PaneId,
                "NEXUS | BIM Intelligence",
                new NexusDashboardPane()
            );

            RibbonPanel panel =
                application.CreateRibbonPanel(tabName, "Project");

            string assemblyPath =
                Assembly.GetExecutingAssembly().Location;

            PushButtonData buttonData = new PushButtonData(
                "NexusProjectInspector",
                "Project\nInspector",
                assemblyPath,
                "NEXUS.NexusHelloCommand"
            );

            PushButton button =
                panel.AddItem(buttonData) as PushButton;

            if (button != null)
            {
                button.ToolTip =
                    "Open the NEXUS BIM Intelligence dashboard.";
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
