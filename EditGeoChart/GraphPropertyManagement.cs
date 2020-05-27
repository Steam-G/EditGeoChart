using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace EditGeoChart
{
    class GraphPropertyManagement
    {
        private GraphPane _pane;
        private Double _xZoom;
        private ZedGraphControl _zgc;
        private bool _verticalLock = true;
        private bool _horizontalLock = true;
        private bool _rulerDisplay = true;
        private bool _gridDisplay = false;
        private bool _markDisplay = true;
        private bool _dotDisplay = false;
        private bool _isSmooth = false;
        private bool _chartOrientation = false;

        public GraphPane Pane { get => _pane; set => _pane = value; }
        public ZedGraphControl Zgc { get => _zgc; set => _zgc = value; }
        public double XZoom1 { get => _xZoom; set => _xZoom = value; }

        PointF centrePoint = new System.Drawing.PointF();

        public void ZoomIn()
        {
            GraphPane myPane = _zgc.GraphPane;
            _zgc.ZoomPane(myPane, 0.1, centrePoint, false);
        }

        public void ZoomOut()
        {
            GraphPane myPane = _zgc.GraphPane;
            _zgc.ZoomPane(myPane, 1.1, centrePoint, false);
        }

        public void Reset()
        {
            _zgc.ZoomOutAll(_zgc.GraphPane);
        }

        public bool VerticalLock
        {
            get => _verticalLock;
            set
            {
                if (_zgc.IsEnableVZoom != value)
                {
                    _zgc.IsEnableVZoom = value;
                    _verticalLock = value;
                }
            }
        }

        public bool HorizontalLock
        {
            get => _horizontalLock;
            set
            {
                if (_zgc.IsEnableHZoom != value)
                {
                    _zgc.IsEnableHZoom = value;
                    _horizontalLock = value;
                }
            }
        }

        public void RulerDisplay()
        {
            GraphPane myPane = _zgc.GraphPane;
            //myPane.XAxis.Scale.MajorStep = 0.1;
            //myPane.XAxis.Scale.MinorStep = 0.1;
            if (_rulerDisplay)
            {
                myPane.XAxis.IsVisible = false;
                myPane.YAxis.IsVisible = false;
                _rulerDisplay = false;
            }
            else
            {
                myPane.XAxis.IsVisible = true;
                myPane.YAxis.IsVisible = true;
                _rulerDisplay = true;
            }
            _zgc.AxisChange();
            _zgc.Invalidate();
        }
        public void GridDisplay()
        {
            GraphPane myPane = _zgc.GraphPane;
            if (_gridDisplay)
            {
                myPane.XAxis.MajorGrid.IsVisible = false;
                myPane.XAxis.MinorGrid.IsVisible = false;
                myPane.YAxis.MajorGrid.IsVisible = false;
                myPane.YAxis.MinorGrid.IsVisible = false;
                _gridDisplay = false;
            }
            else
            {
                myPane.XAxis.MajorGrid.IsVisible = true;
                myPane.XAxis.MajorGrid.DashOn = 10;
                myPane.XAxis.MajorGrid.DashOff = 5;
                myPane.XAxis.MinorGrid.IsVisible = true;
                myPane.XAxis.MinorGrid.DashOn = 1;
                myPane.XAxis.MinorGrid.DashOff = 2;
                myPane.YAxis.MajorGrid.IsVisible = true;
                myPane.YAxis.MajorGrid.DashOn = 10;
                myPane.YAxis.MajorGrid.DashOff = 5;
                myPane.YAxis.MinorGrid.IsVisible = true;
                myPane.YAxis.MinorGrid.DashOn = 1;
                myPane.YAxis.MinorGrid.DashOff = 2;
                _gridDisplay = true;
            }
            _zgc.AxisChange();
            _zgc.Invalidate();
        }
        public void MarkDisplay(bool state) { }
        public void DotDisplay()
        {
            GraphPane myPane = _zgc.GraphPane;

            if (_dotDisplay)
            {
                foreach (CurveItem ci in myPane.CurveList)
                {
                    LineItem line = ci as LineItem;
                    line.Symbol.Fill.IsVisible = false;
                    line.Symbol.IsVisible = false;
                }
                _dotDisplay = false;
            }
            else
            {
                foreach (CurveItem ci in myPane.CurveList)
                {
                    //LineItem line = myPane.CurveList[0] as LineItem;
                    LineItem line = ci as LineItem;
                    line.Symbol.Type = SymbolType.Circle;
                    line.Symbol.Fill.Color = Color.White;
                    line.Symbol.Fill.IsVisible = true;
                    line.Symbol.Size = 4;
                    line.Symbol.IsVisible = true;
                }
                _dotDisplay = true;
            }
            _zgc.AxisChange();
            _zgc.Invalidate();
        }

        public void IsSmooth()
        {
            GraphPane myPane = _zgc.GraphPane;

            if(_isSmooth)
            {
                foreach (CurveItem ci in myPane.CurveList)
                {
                    LineItem line = ci as LineItem;
                    line.Line.IsSmooth = false;
                }
                _isSmooth = false;
            }
            else
            {
                foreach (CurveItem ci in myPane.CurveList)
                {
                    LineItem line = ci as LineItem;
                    line.Line.IsSmooth = true;
                }
                _isSmooth = true;
            }
            _zgc.AxisChange();
            _zgc.Invalidate();
        }

        public void AddThikness()
        {
            GraphPane myPane = _zgc.GraphPane;
            foreach (CurveItem ci in myPane.CurveList)
            {
                LineItem line = ci as LineItem;
                line.Line.Width += 1;
            }
            _zgc.AxisChange();
            _zgc.Invalidate();
        }

        public void ReduceThikness()
        {
            GraphPane myPane = _zgc.GraphPane;
            foreach (CurveItem ci in myPane.CurveList)
            {
                LineItem line = ci as LineItem;
                line.Line.Width -= 1;
            }
            _zgc.AxisChange();
            _zgc.Invalidate();
        }
        public void LineColor() { }
        public void BackgroundColor()
        {
            GraphPane myPane = _zgc.GraphPane;
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Закрасим область графика (его фон) в черный цвет
                myPane.Chart.Fill.Type = FillType.Solid;
                myPane.Chart.Fill.Color = colorDialog.Color;
            }
            _zgc.AxisChange();
            _zgc.Invalidate();
        }
        public void ChartOrientation()
        {
            GraphPane myPane = _zgc.GraphPane;

            foreach (CurveItem ci in myPane.CurveList)
            {
                LineItem line = ci as LineItem;
                for (int i = 0; i < line.Points.Count; i++)
                {
                    var X = line.Points[i].X;
                    var Y = line.Points[i].Y;
                    line.Points[i].X = Y;
                    line.Points[i].Y = X;

                }
            }

            if (_chartOrientation)  _chartOrientation = false;
                else _chartOrientation = true;

            _zgc.AxisChange();
            _zgc.Invalidate();
        }
    }
}
