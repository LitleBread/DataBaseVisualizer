using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace ERDiagramBuilder
{
    public class Graph
    {
        private bool[,] adjacencyMatrix;
        List<List<int>> adjacency;
        private UIElement[] uiElements;
        private Canvas canvas;

        public static bool IsStraightLined { get; set; }
        public Graph(bool[,] adjacencyMatrix, UIElement[] uiElements, Canvas canvas)
        {
            IsStraightLined = true;
            this.adjacencyMatrix = adjacencyMatrix;
            this.uiElements = uiElements;
            this.canvas = canvas;

            adjacency = new List<List<int>>();
            for (int i = 0; i < uiElements.Length; i++)
            {
                adjacency.Add(new List<int>());
                for (int j = 0; j < uiElements.Length; j++)
                {
                    if (adjacencyMatrix[i, j]) adjacency[i].Add(j);
                }
            }
            
            canvas.MouseMove += OnMouseMoveMoveLines;
        }

        private void OnMouseMoveMoveLines(object sender, MouseEventArgs e)
        {
            var lineList = canvas.Children.OfType<Line>().ToList();
            var polylineList = canvas.Children.OfType<Polyline>().ToList();
            foreach (var item in lineList)
            {
                canvas.Children.Remove(item);
            }
            foreach (var item in polylineList)
            {
                canvas.Children.Remove(item);
            }
            for (int i = 0; i < uiElements.Length; i++)
            {
                UIElement element1 = uiElements[i];
                for (int j = 0; j < adjacency[i].Count; j++)
                {
                    UIElement element2 = uiElements[adjacency[i][j]];
                    Line line = new Line();
                    Polyline polyline = new Polyline();
                    var pos1 = canvas.TranslatePoint(new Point(0, 0), element1);
                    var pos2 = canvas.TranslatePoint(new Point(0, 0), element2);
                    pos1.X = -pos1.X;
                    pos2.X = -pos2.X;
                    pos1.Y = -pos1.Y;
                    pos2.Y = -pos2.Y;
                    if (pos1.X > pos2.X + (element2 as Control).ActualWidth / 2)
                    {
                        line.X1 = pos1.X;
                        line.X2 = pos2.X + (element2 as Control).ActualWidth;
                    }
                    else if (pos1.X < pos2.X + (element2 as Control).ActualWidth / 2)
                    {
                        line.X1 = pos1.X + (element1 as Control).ActualWidth;
                        line.X2 = pos2.X;
                    }

                    line.Y1 = pos1.Y + (element1 as Control).ActualHeight / 2;
                    line.Y2 = pos2.Y + (element2 as Control).ActualHeight / 2;
                    line.Stroke = Brushes.Black; 
                    line.StrokeThickness = 1;

                    polyline.Points.Clear();
                    polyline.Points.Add(new Point(line.X1, line.Y1));
                    polyline.Points.Add(new Point((line.X1 + line.X2) / 2, line.Y1));
                    polyline.Points.Add(new Point((line.X1 + line.X2) / 2, line.Y2));
                    polyline.Points.Add(new Point(line.X2, line.Y2));
                    polyline.Stroke= Brushes.Black;
                    polyline.StrokeThickness = 1;
                    //Canvas.SetZIndex(line, 1);
                    //Canvas.SetZIndex(polyline, 1);
                    if (IsStraightLined)
                    {
                        canvas.Children.Add(line);
                    }
                    else
                    {
                        canvas.Children.Add(polyline);
                    }
                }

            }
        }
    }
}
