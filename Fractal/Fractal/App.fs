module MainApp

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes
open System.Windows.Media
open System.Windows.Media.Imaging

open FSharpx

type MainWindow = XAML<"MainWindow.xaml">

let loadWindow() =

   let window = MainWindow()
   let canvas = window.GetChild("Canvas") :?> Canvas

   let gridContainer = window.GetChild("ViewPortContainer") :?> Grid
   let writeableBmp = BitmapFactory.New((int)gridContainer.Width, (int)gridContainer.Height)
   
   let image = window.GetChild("ImageViewport") :?> Image
   image.Source <- writeableBmp

   // low level drawing
   let bitmapDrawSquare (rect:PythagorasTree.Rect) level =
      writeableBmp.DrawQuad(int rect.p1.x, int rect.p1.y, int rect.p2.x, int rect.p2.y, int rect.p3.x, int rect.p3.y, int rect.p4.x, int rect.p4.y, Colors.Black)

   let bitmapDrawSquares uiContext (rects) level =
      Async.Start (
            async{
              do! Async.SwitchToContext(uiContext)
              use bctx = writeableBmp.GetBitmapContext()
              List.iter(fun x -> bitmapDrawSquare x level) rects
              })

    // drawing of the main tree
   let drawForest ctx =

       // Initialize drawing system
//       let drawSquare = DrawCanvas.drawSquareCanvas canvas
       let drawSquares = bitmapDrawSquares
       let drawTree rects level = PythagorasTree.drawPythagorasTree ctx drawSquares rects level

       let size = 70.0
       let x1 = 350.0
       let y1 = 450.0
       let rect1:PythagorasTree.Rect = { p1={x=x1;y=y1}; p3={x=x1-size;y=y1-size} }
       let rect2:PythagorasTree.Rect = { p1={x=x1-300.0;y=y1-100.0}; p3={x=x1-size/2.0-300.0;y=y1-size/2.0-100.0} }
       drawTree [rect2] 0 |> ignore
       drawTree [rect1] 0

   let draw =
    use bctx = writeableBmp.GetBitmapContext()
    writeableBmp.Clear()
    List.iter (fun x -> writeableBmp.DrawRectangle(x*10, x*10, x*10+5, x*10+5, Colors.Blue)) [ 0..1 ]

    // catch the gui thread context and pass it to the drawing methods
   window.Root.Loaded.Add(fun _ -> 
    let ctx = System.Threading.SynchronizationContext.Current
    
    CompositionTarget.Rendering.Add(fun evenArgs -> draw |> ignore) 
    Async.Start (
        async{
            drawForest ctx |> ignore
        })
    )
   window.Root

[<STAThread>]
(new Application()).Run(loadWindow()) |> ignore