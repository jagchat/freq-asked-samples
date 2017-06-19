using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using ChartDemo.Models;

namespace ChartDemo.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmpGraph()
        {
            var vm = new EmpViewModel();
            vm.LoadData();
            return View(vm);
        }

        public ActionResult EmpGraph2()
        {
            var vm = new EmpViewModel();
            vm.LoadData();
            Bitmap image = new Bitmap(300, 50);
            Graphics g = Graphics.FromImage(image);
            var Chart2 = new System.Web.UI.DataVisualization.Charting.Chart();

            Chart2.Width = 412;
            Chart2.Height = 296;

            Chart2.ChartAreas.Add("Series 1").BackColor = System.Drawing.Color.FromArgb(64, System.Drawing.Color.White);
            // create a couple of series  
            Chart2.Series.Add("Series 1");
            
            // add points to series 1
            Chart2.Series["Series 1"].ChartType = SeriesChartType.Line;
            vm.EmployeeData.ForEach(o =>
            {
                Chart2.Series["Series 1"].Points.AddXY(o.Ename, o.Sal);
            });
            
            //Chart2.Series.Add("Series 2");
            //// add points to series 2  
            //Chart2.Series["Series 2"].Points.AddY(10 + 1);
            //Chart2.Series["Series 2"].Points.AddY(1 + 1);
            //Chart2.Series["Series 2"].Points.AddY(2 + 1);
            //Chart2.Series["Series 2"].Points.AddY(10 + 1);

            Chart2.BackColor = Color.Transparent;
            MemoryStream imageStream = new MemoryStream();
            Chart2.SaveImage(imageStream, ChartImageFormat.Png);
            Chart2.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;
            Response.ContentType = "image/png";
            imageStream.WriteTo(Response.OutputStream);
            g.Dispose();
            image.Dispose();
            return null;
        }

        public ActionResult EmpGraph3()
        {
            var vm = new EmpViewModel();
            vm.LoadData();
            Bitmap image = new Bitmap(300, 50);
            Graphics g = Graphics.FromImage(image);

            var c = new System.Web.UI.DataVisualization.Charting.Chart();
            c.AntiAliasing = AntiAliasingStyles.All;
            c.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            c.Width = 640; //SET HEIGHT
            c.Height = 480; //SET WIDTH

            ChartArea ca = new ChartArea();
            ca.BackColor = Color.FromArgb(248, 248, 248);
            ca.BackSecondaryColor = Color.FromArgb(255, 255, 255);
            ca.BackGradientStyle = GradientStyle.TopBottom;

            ca.AxisY.IsMarksNextToAxis = true;
            ca.AxisY.Title = "Salary";
            ca.AxisY.LineColor = Color.FromArgb(157, 157, 157);
            ca.AxisY.MajorTickMark.Enabled = true;
            ca.AxisY.MinorTickMark.Enabled = true;
            ca.AxisY.MajorTickMark.LineColor = Color.FromArgb(157, 157, 157);
            ca.AxisY.MinorTickMark.LineColor = Color.FromArgb(200, 200, 200);
            ca.AxisY.LabelStyle.ForeColor = Color.FromArgb(89, 89, 89);
            ca.AxisY.LabelStyle.Format = "{0:0.0}";
            ca.AxisY.LabelStyle.IsEndLabelVisible = false;
            ca.AxisY.LabelStyle.Font = new Font("Calibri", 4, FontStyle.Regular);
            ca.AxisY.MajorGrid.LineColor = Color.FromArgb(234, 234, 234);

            ca.AxisX.IsMarksNextToAxis = true;
            ca.AxisX.Title = "Names";
            ca.AxisX.LabelStyle.Enabled = true;
            ca.AxisX.LineColor = Color.FromArgb(157, 157, 157);
            
            ca.AxisX.MajorTickMark.Enabled = true;
            ca.AxisX.MinorTickMark.Enabled = false;
            ca.AxisX.MajorTickMark.LineColor = Color.FromArgb(157, 157, 157);
            //ca.AxisX.MinorTickMark.LineColor = Color.FromArgb(200, 200, 200);
            ca.AxisX.MajorGrid.LineWidth = 0;
            //ca.AxisX.MajorGrid.LineWidth = 1;
            //ca.AxisX.MajorGrid.LineColor = Color.FromArgb(234, 234, 234);


            c.ChartAreas.Add(ca);

            Series s = new Series();
            s.ChartType = SeriesChartType.Line;
            s.Font = new Font("Lucida Sans Unicode", 6f);
            s.Color = Color.DeepSkyBlue;
            s.IsValueShownAsLabel = true;
            //s.Color = Color.FromArgb(215, 47, 6);
            //s.BorderColor = Color.FromArgb(159, 27, 13);
            //s.BackSecondaryColor = Color.FromArgb(173, 32, 11);
            //s.BackGradientStyle = GradientStyle.LeftRight;

            s.BorderWidth = 2;
            s.MarkerStyle = MarkerStyle.Square;
            s.MarkerBorderWidth = 10;
            s.MarkerSize = 10;
            s.MarkerColor = Color.Blue;
            int i = 0;
            vm.EmployeeData.ForEach(o =>
            {
                s.Points.AddXY(o.Ename, o.Sal);
            });

            c.Series.Add(s);

            //c.BackColor = Color.Transparent;
            MemoryStream imageStream = new MemoryStream();
            c.SaveImage(imageStream, ChartImageFormat.Png);
            c.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;
            Response.ContentType = "image/png";
            imageStream.WriteTo(Response.OutputStream);
            g.Dispose();
            image.Dispose();
            return null;
        }
    }
}