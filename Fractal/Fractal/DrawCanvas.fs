module DrawCanvas

open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Media

open PythagorasTree


let drawSquareCanvas (canvas:Canvas) uiContext (rect:Rect) level = 
        Async.Start (
            async{
                do! Async.SwitchToContext(uiContext)
                let polygon = new Polygon()
                let points = new PointCollection()

                polygon.Stroke <- System.Windows.Media.Brushes.Black
                polygon.StrokeThickness <- 0.0

                let mp3_5 = squareLeftPoint(rect.p3,rect.p4)

                let p1 = new Point(rect.p1.x, rect.p1.y)
                let p2 = new Point(rect.p2.x, rect.p2.y)
                let p3 = new Point(rect.p3.x, rect.p3.y)
                let p4 = new Point(rect.p4.x, rect.p4.y)
                let p3_5 = new Point(mp3_5.x, mp3_5.y)

                points.Add(p1)
                points.Add(p2)
                points.Add(p3)
                points.Add(p3_5)
                points.Add(p4)

                polygon.Points <- points
        
                match level with
                    | x when x<7 -> polygon.Fill <- System.Windows.Media.Brushes.Chocolate
                    | x when x<13 -> polygon.Fill <- System.Windows.Media.Brushes.DarkGreen
                    | _ -> polygon.Fill <- System.Windows.Media.Brushes.White

                canvas.Children.Add(polygon) |> ignore
            })