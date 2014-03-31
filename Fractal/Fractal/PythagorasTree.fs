module PythagorasTree

open System
open System.Threading

let sqrt2 = Math.Sqrt(2.0)

type MyPoint = { x: float; y: float }

let squareLeftPoint(p1,p3) =
    let xc = (p1.x + p3.x)/2.0
    let yc = (p1.y + p3.y)/2.0    // Center point
    let xd = (p1.x - p3.x)/2.0
    let yd = (p1.y - p3.y)/2.0    // Half-diagonal

    let x2 = xc - yd
    let y2 = yc + xd  
    { x=x2;y=y2 }

let squareRightPoint(p1,p3) =
    let xc = (p1.x + p3.x)/2.0
    let yc = (p1.y + p3.y)/2.0    // Center point
    let xd = (p1.x - p3.x)/2.0
    let yd = (p1.y - p3.y)/2.0    // Half-diagonal

    let x4 = xc + yd
    let y4 = yc - xd  
    { x=x4;y=y4 }


type Rect = {
    p1: MyPoint; p3: MyPoint
} with
    member x.p2 = squareLeftPoint(x.p1,x.p3)
    member x.p4 = squareRightPoint(x.p1,x.p3)

let slideFactor = 1.0

let generateLeftRec(rect:Rect) =
    {p1 = rect.p4; p3 = {x=rect.p4.x+(rect.p4.x - rect.p1.x)*slideFactor ; y=rect.p4.y+(rect.p4.y - rect.p1.y)*slideFactor}}

let generateRightRec(rect:Rect) =
    let p1 = rect.p3
    let p3 = {x=rect.p3.x+(rect.p4.x - rect.p1.x)/slideFactor;y=rect.p3.y+(rect.p4.y - rect.p1.y)/slideFactor}
    {p1 = squareRightPoint(p1,p3); p3 = squareLeftPoint(p1,p3) }

let generateChildren(rect) =
        let leftRect = generateLeftRec(rect)
        let rightRect = generateRightRec(rect)
        [leftRect;rightRect]

let rec drawPythagorasTree ctx drawSquare rects level =
        match level with
        | 16 -> 0
        | _ ->
//            List.iter (fun x -> drawSquare ctx x level) rects
            drawSquare ctx rects level
            let newRects = List.fold (fun l rect -> 
                let tmp = (generateChildren rect)
                tmp.Head :: tmp.Tail.Head :: l) [] rects
            Thread.Sleep(100)
            drawPythagorasTree ctx drawSquare newRects (level+1)