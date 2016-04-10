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
        public LineSeries Series;
        public LinearAxis xAxis;
        public LinearAxis yAxis;

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
            Series = new LineSeries { Title = "Error", StrokeThickness = 1 };

            // add Series and Axis to plot model
            Plot.Model.Series.Add(Series);
            xAxis = new LinearAxis(AxisPosition.Bottom, 0.0, 10.0);
            Plot.Model.Axes.Add(xAxis);
            yAxis = new LinearAxis(AxisPosition.Left, 0.0, 5.0);
            Plot.Model.Axes.Add(yAxis);
        }

        public void AddPoint(double val, TimeSpan elapsed)
        {
            Series.Points.Add(new DataPoint(elapsed.TotalSeconds, val));
            xAxis.Maximum = elapsed.TotalSeconds + Offset;
            yAxis.Maximum = Series.Points.Max(x=>x.Y)*1.1;
            Plot.Model.ResetAllAxes();
            Plot.Invalidate();
        }
    }
}
