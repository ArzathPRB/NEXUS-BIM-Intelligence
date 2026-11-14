
using Autodesk.Revit.DB;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NEXUS
{
    public class DashboardWindow : Window
    {
        public DashboardWindow(Document doc)
        {
            Title = "NEXUS | BIM Intelligence Dashboard";
            Width = 950;
            Height = 650;

            WindowStartupLocation =
                WindowStartupLocation.CenterScreen;

            Background = new SolidColorBrush(
                System.Windows.Media.Color.FromRgb(25, 32, 35));

            int levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .GetElementCount();

            int views = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Count(v => !v.IsTemplate);

            int sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .GetElementCount();

            int families = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .GetElementCount();

            int warnings = doc.GetWarnings().Count;

            StackPanel main = new StackPanel
            {
                Margin = new Thickness(30)
            };

            main.Children.Add(new TextBlock
            {
                Text = "NEXUS",
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LightGreen
            });

            main.Children.Add(new TextBlock
            {
                Text = "BIM INTELLIGENCE DASHBOARD",
                FontSize = 13,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 5, 0, 25)
            });

            main.Children.Add(new TextBlock
            {
                Text = "PROJECT: " + doc.Title +
                       " | REVIT: " +
                       doc.Application.VersionNumber,
                FontSize = 15,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 25)
            });

            AddHeading(main, "MODEL STATISTICS");

            AddMetric(main, "LEVELS", levels);
            AddMetric(main, "VIEWS", views);
            AddMetric(main, "SHEETS", sheets);
            AddMetric(main, "LOADED FAMILIES", families);
            AddMetric(main, "REVIT WARNINGS", warnings);

            Content = new ScrollViewer
            {
                Content = main,
                VerticalScrollBarVisibility =
                    ScrollBarVisibility.Auto
            };
        }

        private static void AddHeading(
            StackPanel panel, string heading)
        {
            panel.Children.Add(new TextBlock
            {
                Text = heading,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LightGreen,
                Margin = new Thickness(0, 0, 0, 15)
            });
        }

        private static void AddMetric(
            StackPanel panel, string label, int value)
        {
            Border card = new Border
            {
                Background = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(42, 55, 53)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(18),
                Margin = new Thickness(0, 0, 0, 10)
            };

            StackPanel content = new StackPanel();

            content.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 12,
                Foreground = Brushes.LightGreen
            });

            content.Children.Add(new TextBlock
            {
                Text = value.ToString(),
                FontSize = 26,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            });

            card.Child = content;
            panel.Children.Add(card);
        }
    }
}
