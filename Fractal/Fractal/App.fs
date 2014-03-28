module MainApp

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Media
open FSharpx

type MainWindow = XAML<"MainWindow.xaml">

let loadWindow() =

   let window = MainWindow()
   let canvas = window.GetChild("Canvas") :?> Canvas

    // drawing of the main tree
   let drawForest ctx =

       // Initialize drawing system
       let drawSquare = DrawCanvas.drawSquareCanvas canvas
   
       let drawTree rects level = PythagorasTree.drawPythagorasTree ctx drawSquare rects level
       let size = 70.0
       let x1 = 270.0
       let y1 = 450.0
       let rect1:PythagorasTree.Rect = { p1={x=x1;y=y1}; p3={x=x1-size;y=y1-size} }
       let rects = [rect1]
       drawTree rects 0


    // catch the gui thread context and pass it to the drawing methods
   window.Root.Loaded.Add(fun _ -> 
    System.Threading.SynchronizationContext.SetSynchronizationContext(System.Threading.SynchronizationContext.Current) // this doesn't seems to work
    let ctx = System.Threading.SynchronizationContext.Current
    Async.Start (
        async{
            drawForest ctx |> ignore
        })
    )

   window.Root

[<STAThread>]
(new Application()).Run(loadWindow()) |> ignore