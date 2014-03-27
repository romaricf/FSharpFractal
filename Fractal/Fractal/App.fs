module MainApp

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Media
open FSharpx
open System.Threading

type MainWindow = XAML<"MainWindow.xaml">



let loadWindow() =

   let window = MainWindow()

   let canvas = window.GetChild("Canvas") :?> Canvas

   let drawSquare(rect:Tree.Rect, level, ctx) = 
        let uiContext = ctx
        Async.Start (
            async{
                do! Async.SwitchToContext(uiContext)
                let polygon = new Polygon()
                let points = new PointCollection()

                polygon.Stroke <- System.Windows.Media.Brushes.Black
                polygon.StrokeThickness <- 0.0

                let mp3_5 = Tree.squareLeftPoint(rect.p3,rect.p4)

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
        
   
   let rec drawPythagorasTree(rects, level, ctx) =
        match level with
        | 14 -> 0
        | _ ->
            List.iter (fun x -> drawSquare(x,level,ctx)) rects
            let newRects = List.fold (fun l rect -> l @ (Tree.generateChildren rect)) [] rects
            Thread.Sleep(200)
            drawPythagorasTree(newRects,level+1,ctx)

    
    // drawing of the main tree
   let drawTree(ctx) =
        let size = 70.0
        let oX = 270.0
        let oY = 450.0
        let rect1:Tree.Rect = { p1={x=oX;y=oY}; p3={x=oX-size;y=oY-size} }
        let rects = [rect1]
        drawPythagorasTree(rects,0,ctx)


    // sto catch the gui thread context and pass it to the drawing methods
   window.Root.Loaded.Add(fun _ -> 
    System.Threading.SynchronizationContext.SetSynchronizationContext(System.Threading.SynchronizationContext.Current) // this doesn't seems to work
    let ctx = System.Threading.SynchronizationContext.Current
    Async.Start (
        async{
            drawTree(ctx) |> ignore
        })
    )

   window.Root

[<STAThread>]
(new Application()).Run(loadWindow()) |> ignore