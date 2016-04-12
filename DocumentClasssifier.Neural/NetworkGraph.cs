using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;

namespace DocumentClasssifier.Neural
{
    public partial class NetworkGraph : Form
    {
        PlotView Plot;
        public int CX = 0;
        public int Offset = 1;
        public LineSeries ValidationError;
        public LineSeries TrainingError;
        public LinearAxis xAxis;
        public LogarithmicAxis yAxis;

        public NetworkGraph()
        {
            InitializeComponent();

            Plot = new PlotView();
            Plot.Model = new PlotModel();
            Plot.Dock = DockStyle.Fill;
            this.Controls.Add(Plot);

            Plot.Model.PlotType = PlotType.XY;
            Plot.Model.Background = OxyColor.FromRgb(255, 255, 255);
            Plot.Model.TextColor = OxyColor.FromRgb(0, 0, 0);

            // Create Line series
            TrainingError = new LineSeries { Title = "Training Error", StrokeThickness = 1 };
            ValidationError = new LineSeries { Title = "Validation Error", StrokeThickness = 1 };

            // add Series and Axis to plot model
            Plot.Model.Series.Add(TrainingError);
            Plot.Model.Series.Add(ValidationError);
            
            xAxis = new LinearAxis(AxisPosition.Bottom, 0.0, 10.0);
            Plot.Model.Axes.Add(xAxis);
            yAxis = new LogarithmicAxis(AxisPosition.Left, "Error", .01, 10);
            Plot.Model.Axes.Add(yAxis);
        }

        public void AddPoint(double trainingVal, double validationVal, TimeSpan elapsed)
        {
            TrainingError.Points.Add(new DataPoint(elapsed.TotalSeconds, trainingVal));
            ValidationError.Points.Add(new DataPoint(elapsed.TotalSeconds, validationVal));
            xAxis.Maximum = elapsed.TotalSeconds + Offset;
            yAxis.Minimum = Math.Min(TrainingError.Points.Min(x => x.Y) / 1.1, ValidationError.Points.Min(x => x.Y) / 1.1);
            yAxis.Maximum = Math.Max(TrainingError.Points.Max(x=>x.Y)*1.1,ValidationError.Points.Max(x => x.Y) * 1.1);
            Plot.Model.ResetAllAxes();
            Plot.Invalidate();
        }

        public void AddTitle(string title)
        {
            Plot.Model.Title = title;
        }

        public void ResetData()
        {
            TrainingError.Points.Clear();
            ValidationError.Points.Clear();
            Plot.Model.ResetAllAxes();
            Plot.Invalidate();
        }
    }
}
