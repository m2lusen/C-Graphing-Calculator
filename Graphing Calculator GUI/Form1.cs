using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using NCalc;
using System.IO;

namespace Graphing_Calculator_GUI
{
    public partial class Form1 : Form
    {
        private class LineInfo
        {
            public Guid ID { get; set; }
            public ComboBox ComboBox { get; set; }
            public TextBox InputField { get; set; }
            public Button DisplayButton { get; set; }
            public Button RemoveButton { get; set; }
            public LineSeries Line { get; set; }
            public bool IsHidden { get; set; }

            public LineInfo()
            {
                ID = Guid.NewGuid();
                IsHidden = false;
            }
        }

        private List<LineInfo> lineInfos = new List<LineInfo>();
        private PlotModel plotModel = new PlotModel { Title = "Graph" };

        private System.Windows.Forms.Timer timer;
        private bool isSidebarVisible;
        private int sidebarWidth;

        private Panel scrollablePanel; // Scrollable container

        public Form1()
        {
            InitializeComponent();
            InitializePlot();
            InitializeSidebar();
        }

        private void InitializePlot()
        {
            graph.Model = plotModel;

            // Set up axes to center the graph with both positive and negative values visible
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = -10,
                Maximum = 10,
                IsPanEnabled = false,
                IsZoomEnabled = false
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -10,
                Maximum = 10,
                IsPanEnabled = false,
                IsZoomEnabled = false
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);
        }

        private void InitializeSidebar()
        {
            sidebarWidth = 400; // Set the desired width of the sidebar
            isSidebarVisible = false;

            // Initialize the Timer
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 10; // Set the interval for the timer
            timer.Tick += Timer_Tick;

            // Set initial sidebar properties
            containerLineEq.Width = 0;
            containerLineEq.Visible = true;

            // Create scrollable container
            scrollablePanel = new Panel
            {
                AutoScroll = true,
                Width = sidebarWidth - 20,
                Height = containerLineEq.Height - 50,
                Location = new Point(0, 50),
            };
            containerLineEq.Controls.Add(scrollablePanel);

            toggleLineEq.Click += ToggleLineEq_Click;

            toggleLineEq.Size = new Size(30, 100);
            toggleLineEq.Location = new Point(containerLineEq.Width, 0);
            toggleLineEq.FlatAppearance.BorderSize = 0;

            // Add addButton inside the sidebar but outside the scrollable container
            Button addButton = new Button
            {
                Text = "Add",
                Width = 70,
                Location = new Point(10, 10),
            };
            addButton.Click += addButton_Click;
            containerLineEq.Controls.Add(addButton);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // Create a new LineInfo instance
            LineInfo lineInfo = new LineInfo();

            // Initialize ComboBox
            lineInfo.ComboBox = new ComboBox
            {
                Items = { "y = f(x)", "x = f(y)" },
                SelectedIndex = 0,
                Width = 70
            };
            lineInfo.ComboBox.SelectedIndexChanged += (s, ev) => EquationChanged(lineInfo);

            // Initialize TextBox
            lineInfo.InputField = new TextBox
            {
                Width = 160
            };
            lineInfo.InputField.TextChanged += (s, ev) => EquationChanged(lineInfo);

            // Initialize Display Button
            lineInfo.DisplayButton = new Button
            {
                Text = "Display",
                Width = 70
            };
            lineInfo.DisplayButton.Click += (s, ev) => ToggleDisplayFunction(lineInfo);

            // Initialize Remove Button
            lineInfo.RemoveButton = new Button
            {
                Text = "X",
                Width = 30
            };
            lineInfo.RemoveButton.Click += (s, ev) => RemoveControls(lineInfo);

            // Add controls to the scrollablePanel
            scrollablePanel.Controls.Add(lineInfo.ComboBox);
            scrollablePanel.Controls.Add(lineInfo.InputField);
            scrollablePanel.Controls.Add(lineInfo.DisplayButton);
            scrollablePanel.Controls.Add(lineInfo.RemoveButton);

            // Add to the list
            lineInfos.Add(lineInfo);

            LayoutControls();
        }

        private void RemoveControls(LineInfo lineInfo)
        {
            // If the line is visible, remove it from the plot
            if (lineInfo.Line != null && !lineInfo.IsHidden)
            {
                plotModel.Series.Remove(lineInfo.Line);
                plotModel.InvalidatePlot(true);
            }

            // Remove controls from panel
            scrollablePanel.Controls.Remove(lineInfo.ComboBox);
            scrollablePanel.Controls.Remove(lineInfo.InputField);
            scrollablePanel.Controls.Remove(lineInfo.DisplayButton);
            scrollablePanel.Controls.Remove(lineInfo.RemoveButton);

            // Remove from the list
            lineInfos.Remove(lineInfo);

            LayoutControls();
        }

        private void LayoutControls()
        {
            int yOffset = 10;
            foreach (var lineInfo in lineInfos)
            {
                lineInfo.ComboBox.Location = new Point(10, yOffset);
                lineInfo.InputField.Location = new Point(90, yOffset);
                lineInfo.DisplayButton.Location = new Point(260, yOffset);
                lineInfo.RemoveButton.Location = new Point(340, yOffset);
                yOffset += 30;
            }
        }

        private void ToggleDisplayFunction(LineInfo lineInfo)
        {
            if (lineInfo.Line != null)
            {
                // Toggle visibility
                lineInfo.Line.IsVisible = !lineInfo.Line.IsVisible;
                lineInfo.IsHidden = !lineInfo.Line.IsVisible;

                plotModel.InvalidatePlot(true);
                lineInfo.DisplayButton.Text = lineInfo.Line.IsVisible ? "Hide" : "Display";
            }
            else
            {
                try
                {
                    // Create and display a new series
                    DisplayFunction(lineInfo);
                    lineInfo.DisplayButton.Text = "Hide";
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DisplayFunction(LineInfo lineInfo)
        {
            string function = lineInfo.InputField.Text;
            string graphType = lineInfo.ComboBox.SelectedItem.ToString();
            var series = new LineSeries { Title = $"{lineInfo.ID}" };

            try
            {
                if (graphType == "y = f(x)")
                {
                    for (double x = -10; x <= 10; x += 0.1)
                    {
                        double y = EvaluateExpression(function, x, "x");
                        series.Points.Add(new DataPoint(x, y));
                    }
                }
                else if (graphType == "x = f(y)")
                {
                    for (double y = -10; y <= 10; y += 0.1)
                    {
                        double x = EvaluateExpression(function, y, "y");
                        series.Points.Add(new DataPoint(x, y));
                    }
                }

                plotModel.Series.Add(series);
                plotModel.InvalidatePlot(true);

                lineInfo.Line = series;
                lineInfo.IsHidden = false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error in function evaluation: " + ex.Message, ex);
            }
        }

        private double EvaluateExpression(string function, double variable, string variableName)
        {
            // Replace only the specified variable
            function = ReplaceVariable(function, variable, variableName);

            var expression = new Expression(function);
            var result = expression.Evaluate();

            return Convert.ToDouble(result);
        }

        private string ReplaceVariable(string expression, double value, string variableName)
        {
            // Replace only standalone instances of the variable
            // To avoid replacing substrings, use a regex
            return System.Text.RegularExpressions.Regex.Replace(
                expression,
                $@"\b{variableName}\b",
                value.ToString(),
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private void EquationChanged(LineInfo lineInfo)
        {
            // Remove the existing line if it exists
            if (lineInfo.Line != null)
            {
                plotModel.Series.Remove(lineInfo.Line);
                lineInfo.Line = null;
                lineInfo.IsHidden = true;
                plotModel.InvalidatePlot(true);
            }

            // Reset the display button text to "Display"
            lineInfo.DisplayButton.Text = "Display";
        }

        private void ToggleLineEq_Click(object sender, EventArgs e)
        {
            isSidebarVisible = !isSidebarVisible;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isSidebarVisible)
            {
                if (containerLineEq.Width < sidebarWidth)
                {
                    containerLineEq.Width += 20;
                    if (containerLineEq.Width >= sidebarWidth)
                    {
                        containerLineEq.Width = sidebarWidth;
                        timer.Stop();
                    }
                }
            }
            else
            {
                if (containerLineEq.Width > 0)
                {
                    containerLineEq.Width -= 20;
                    if (containerLineEq.Width <= 0)
                    {
                        containerLineEq.Width = 0;
                        timer.Stop();
                    }
                }
            }

            toggleLineEq.Location = new Point(containerLineEq.Width, 0);
        }
    }
}
