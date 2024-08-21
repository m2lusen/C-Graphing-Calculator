using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using NCalc;

namespace Graphing_Calculator_GUI
{
    // bug where the y and x are intechangeable

    public partial class Form1 : Form
    {
        private List<ComboBox> comboBoxes = new List<ComboBox>();
        private List<TextBox> inputFields = new List<TextBox>();
        private List<Button> displayButtons = new List<Button>();
        private List<Button> removeButtons = new List<Button>(); // List to hold remove buttons
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
            // Create new ComboBox
            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add("y = f(x)");
            comboBox.Items.Add("x = f(y)");
            comboBox.SelectedIndex = 0;
            comboBox.Width = 70;
            comboBoxes.Add(comboBox);

            // Create new TextBox
            TextBox inputField = new TextBox();
            inputField.Width = 160;
            inputFields.Add(inputField);

            // Create new Display Button
            Button displayButton = new Button();
            displayButton.Text = "Display";
            displayButton.Click += (s, ev) => ToggleDisplayFunction(comboBox, inputField, displayButton);
            displayButton.Width = 70;
            displayButtons.Add(displayButton);

            // Create new Remove Button
            Button removeButton = new Button();
            removeButton.Text = "X";
            removeButton.Click += (s, ev) => RemoveControls(comboBox, inputField, displayButton, removeButton);
            removeButton.Width = 30;
            removeButtons.Add(removeButton);

            // Add controls to scrollablePanel
            scrollablePanel.Controls.Add(comboBox);
            scrollablePanel.Controls.Add(inputField);
            scrollablePanel.Controls.Add(displayButton);
            scrollablePanel.Controls.Add(removeButton);

            LayoutControls();
        }

        private void RemoveControls(ComboBox comboBox, TextBox inputField, Button displayButton, Button removeButton)
        {
            ToggleDisplayFunction(comboBox, inputField, displayButton);

            // Remove controls from panel and lists
            scrollablePanel.Controls.Remove(comboBox);
            scrollablePanel.Controls.Remove(inputField);
            scrollablePanel.Controls.Remove(displayButton);
            scrollablePanel.Controls.Remove(removeButton);

            comboBoxes.Remove(comboBox);
            inputFields.Remove(inputField);
            displayButtons.Remove(displayButton);
            removeButtons.Remove(removeButton);

            LayoutControls();
        }

        private void LayoutControls()
        {
            int yOffset = 10;
            for (int i = 0; i < comboBoxes.Count; i++)
            {
                comboBoxes[i].Location = new System.Drawing.Point(10, yOffset);
                inputFields[i].Location = new System.Drawing.Point(90, yOffset);
                displayButtons[i].Location = new System.Drawing.Point(260, yOffset);
                removeButtons[i].Location = new System.Drawing.Point(340, yOffset);
                yOffset += 30;
            }
        }

        private void ToggleDisplayFunction(ComboBox comboBox, TextBox inputField, Button displayButton)
        {
            string seriesTitle = $"{comboBox.SelectedItem} - {inputField.Text}";
            var existingSeries = plotModel.Series.FirstOrDefault(series => series.Title == seriesTitle) as LineSeries;

            if (existingSeries != null)
            {
                // Toggle visibility
                existingSeries.IsVisible = !existingSeries.IsVisible;
                plotModel.InvalidatePlot(true);

                // Update button text based on the visibility status
                displayButton.Text = existingSeries.IsVisible ? "Hide" : "Display";
            }
            else
            {
                // Create and display a new series
                DisplayFunction(comboBox, inputField, seriesTitle);
                displayButton.Text = "Hide";
            }
        }

        private void DisplayFunction(ComboBox comboBox, TextBox inputField, string seriesTitle)
        {
            string function = inputField.Text;
            string graphType = comboBox.SelectedItem.ToString();
            var series = new LineSeries { Title = seriesTitle };

            try
            {
                if (graphType == "y = f(x)")
                {
                    for (double x = -10; x <= 10; x += 0.1)
                    {
                        double y = EvaluateExpression(function, x);
                        series.Points.Add(new DataPoint(x, y));
                    }
                }
                else if (graphType == "x = f(y)")
                {
                    for (double y = -10; y <= 10; y += 0.1)
                    {
                        double x = EvaluateExpression(function, y);
                        series.Points.Add(new DataPoint(x, y));
                    }
                }

                plotModel.Series.Add(series);
                plotModel.InvalidatePlot(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in function evaluation: " + ex.Message);
            }
        }

        private double EvaluateExpression(string function, double variable)
        {
            function = function.Replace("x", variable.ToString());
            function = function.Replace("y", variable.ToString());

            var expression = new Expression(function);
            var result = expression.Evaluate();

            return Convert.ToDouble(result);
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
