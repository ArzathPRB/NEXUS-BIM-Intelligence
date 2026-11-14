
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NEXUS
{
    public class NexusDashboardPane : IDockablePaneProvider
    {
        public static readonly DockablePaneId PaneId =
            new DockablePaneId(
                new Guid("A7D23B51-91E4-4C62-AB38-5F7D9E210641")
            );

        public static NexusDashboardPane? Instance { get; private set; }

        public DashboardPaneControl Control { get; private set; }
            = new DashboardPaneControl();

        public NexusDashboardPane()
        {
            Instance = this;
        }

        public void SetupDockablePane(
            DockablePaneProviderData data)
        {
            data.FrameworkElement = Control;

            data.InitialState = new DockablePaneState
            {
                DockPosition = DockPosition.Right
            };
        }
    }

    public class DashboardPaneControl : UserControl
    {
        private readonly StackPanel content;

        public DashboardPaneControl()
        {
            Background = new SolidColorBrush(
                System.Windows.Media.Color.FromRgb(25, 32, 35));

            content = new StackPanel
            {
                Margin = new Thickness(20)
            };

            Content = new ScrollViewer
            {
                Content = content,
                VerticalScrollBarVisibility =
                    ScrollBarVisibility.Auto
            };

            ShowWelcome();
        }

        private void ShowWelcome()
        {
            content.Children.Clear();

            content.Children.Add(new TextBlock
            {
                Text = "NEXUS",
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LightGreen
            });

            content.Children.Add(new TextBlock
            {
                Text = "BIM INTELLIGENCE",
                Foreground = Brushes.White,
                Margin = new Thickness(0, 5, 0, 20)
            });

            content.Children.Add(new TextBlock
            {
                Text = "Open a Revit project and select Project Inspector.",
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            });
        }

        public void Refresh(Document doc)
        {
            content.Children.Clear();

            AddHeading("NEXUS | BIM INTELLIGENCE");

            AddMetric("PROJECT", doc.Title);

            AddMetric("REVIT VERSION",
                doc.Application.VersionNumber);

            AddMetric("LEVELS",
                new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .GetElementCount().ToString());

            AddMetric("VIEWS",
                new FilteredElementCollector(doc)
                    .OfClass(typeof(View))
                    .Cast<View>()
                    .Count(v => !v.IsTemplate).ToString());

            AddMetric("SHEETS",
                new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSheet))
                    .GetElementCount().ToString());

            AddMetric("SCHEDULES",
                new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSchedule))
                    .Cast<ViewSchedule>()
                    .Count(s => !s.IsTemplate).ToString());

            AddMetric("LOADED FAMILIES",
                new FilteredElementCollector(doc)
                    .OfClass(typeof(Family))
                    .GetElementCount().ToString());

            AddMetric("REVIT WARNINGS",
                doc.GetWarnings().Count.ToString());
        }

        private void AddHeading(string text)
        {
            content.Children.Add(new TextBlock
            {
                Text = text,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LightGreen,
                Margin = new Thickness(0, 0, 0, 20)
            });
        }

        private void AddMetric(string label, string value)
        {
            Border card = new Border
            {
                Background = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(42, 55, 53)),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 10),
                CornerRadius = new CornerRadius(6)
            };

            StackPanel panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = label,
                Foreground = Brushes.LightGreen,
                FontSize = 11
            });

            panel.Children.Add(new TextBlock
            {
                Text = value,
                Foreground = Brushes.White,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap
            });

            card.Child = panel;
            content.Children.Add(card);
        }
    }
}
