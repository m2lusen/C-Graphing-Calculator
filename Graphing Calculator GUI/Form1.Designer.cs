namespace Graphing_Calculator_GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            graph = new OxyPlot.WindowsForms.PlotView();
            containerLineEq = new Panel();
            toggleLineEq = new Button();
            SuspendLayout();
            // 
            // graph
            // 
            graph.Location = new Point(387, 12);
            graph.Name = "graph";
            graph.PanCursor = Cursors.Hand;
            graph.Size = new Size(401, 426);
            graph.TabIndex = 0;
            graph.Text = "plotView1";
            graph.ZoomHorizontalCursor = Cursors.SizeWE;
            graph.ZoomRectangleCursor = Cursors.SizeNWSE;
            graph.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // containerLineEq
            // 
            containerLineEq.Location = new Point(-2, 0);
            containerLineEq.Name = "containerLineEq";
            containerLineEq.Size = new Size(359, 450);
            containerLineEq.TabIndex = 2;
            // 
            // toggleLineEq
            // 
            toggleLineEq.Location = new Point(363, 188);
            toggleLineEq.Name = "toggleLineEq";
            toggleLineEq.Size = new Size(75, 23);
            toggleLineEq.TabIndex = 3;
            toggleLineEq.Text = "Lines";
            toggleLineEq.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(toggleLineEq);
            Controls.Add(containerLineEq);
            Controls.Add(graph);
            Name = "Form1";
            Text = "Graphing Calculator";
            ResumeLayout(false);
        }

        #endregion

        private OxyPlot.WindowsForms.PlotView graph;
        private Panel containerLineEq;
        private Button toggleLineEq;
    }
}
