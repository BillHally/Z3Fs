﻿module Microsoft.Z3.FSharp.Real

open System
open Microsoft.Z3
open Microsoft.Z3.FSharp.Bool

module Z3 =  Microsoft.Z3.FSharp.Context

type RealArith = RealExpr of RealExpr

[<AutoOpen>]
module internal RealUtils =
    let inline mkReal (x: float) = Z3.getContext().MkReal(string x)
    let inline add (x: RealExpr) (y: RealExpr) = Z3.getContext().MkAdd(x, y) :?> RealExpr |> RealExpr
    let inline subtract (x: RealExpr) (y: RealExpr) = Z3.getContext().MkSub(x, y) :?> RealExpr |> RealExpr
    let inline multiply (x: RealExpr) (y: RealExpr) = Z3.getContext().MkMul(x, y) :?> RealExpr |> RealExpr
    let inline exp (x: RealExpr) (y: bigint) =
            let rec loop i acc =
                if i = 0I then acc
                else loop (i-1I) (Z3.getContext().MkMul(acc, x))
            if y = 0I then (mkReal 0.) :> RealExpr |> RealExpr
            elif y > 0I then loop (y-1I) x :?> RealExpr |> RealExpr
            else failwith "Coefficient should be a non-negative integer"
    let inline gt (x: RealExpr) (y: RealExpr) = Z3.getContext().MkGt(x, y) |> BoolExpr
    let inline eq (x: RealExpr) (y: RealExpr) = Z3.getContext().MkEq(x, y) |> BoolExpr
    let inline ge (x: RealExpr) (y: RealExpr) = Z3.getContext().MkGe(x, y) |> BoolExpr
    let inline lt (x: RealExpr) (y: RealExpr) = Z3.getContext().MkLt(x, y) |> BoolExpr
    let inline ueq (x: RealExpr) (y: RealExpr) = Z3.getContext().MkDistinct(x, y) |> BoolExpr
    let inline le (x: RealExpr) (y: RealExpr) = Z3.getContext().MkLe(x, y) |> BoolExpr

type RealArith with
    static member (+)(RealExpr x, RealExpr y) = add x y
    static member (+)(RealExpr x, y: float) = add x (mkReal y)
    static member (+)(x: float, RealExpr y) = add (mkReal x) y
    static member (+)(x: float, y: float) = add (mkReal x) (mkReal y)
    static member (-)(RealExpr x, RealExpr y) = subtract x y
    static member (-)(RealExpr x, y: float) = subtract x (mkReal y)
    static member (-)(x: float, RealExpr y) = subtract (mkReal x) y
    static member (-)(x: float, y: float) = subtract (mkReal x) (mkReal y)
    static member (*)(RealExpr x, RealExpr y) = multiply x y
    static member (*)(RealExpr x, y: float) = multiply x (mkReal y)
    static member (*)(x: float, RealExpr y) = multiply (mkReal x) y
    static member (*)(x: float, y: float) = multiply (mkReal x) (mkReal y)  
    static member Pow(RealExpr x, y: bigint) = exp x y
    static member Pow(x: float, y: bigint) = exp (mkReal x) y
    static member (>.)(RealExpr x, RealExpr y) = gt x y
    static member (>.)(RealExpr x, y: float) = gt x (mkReal y)
    static member (>.)(x: float, RealExpr y) = gt (mkReal x) y
    static member (>.)(x: float, y: float) = gt (mkReal x) (mkReal y)  
    static member (=.)(RealExpr x, RealExpr y) = eq x y
    static member (=.)(RealExpr x, y: float) = eq x (mkReal y)
    static member (=.)(x: float, RealExpr y) = eq (mkReal x) y
    static member (=.)(x: float, y: float) = eq (mkReal x) (mkReal y)  
    static member (>=.)(RealExpr x, RealExpr y) = ge x y
    static member (>=.)(RealExpr x, y: float) = ge x (mkReal y)
    static member (>=.)(x: float, RealExpr y) = ge (mkReal x) y
    static member (>=.)(x: float, y: float) = ge (mkReal x) (mkReal y)  
    static member (<.)(RealExpr x, RealExpr y) = lt x y
    static member (<.)(RealExpr x, y: float) = lt x (mkReal y)
    static member (<.)(x: float, RealExpr y) = lt (mkReal x) y
    static member (<.)(x: float, y: float) = lt (mkReal x) (mkReal y)  
    static member (<>.)(RealExpr x, RealExpr y) = ueq x y
    static member (<>.)(RealExpr x, y: float) = ueq x (mkReal y)
    static member (<>.)(x: float, RealExpr y) = ueq (mkReal x) y
    static member (<>.)(x: float, y: float) = ueq (mkReal x) (mkReal y)  
    static member (<=.)(RealExpr x, RealExpr y) = le x y
    static member (<=.)(RealExpr x, y: float) = le x (mkReal y)
    static member (<=.)(x: float, RealExpr y) = le (mkReal x) y
    static member (<=.)(x: float, y: float) = le (mkReal x) (mkReal y)

let internal mkRealVar =
    let context = Z3.getContext()
    fun (s: string) -> context.MkRealConst s 

/// Return an int const with supplied name
let Real = mkRealVar >> RealExpr

type Microsoft.Z3.FSharp.Bool.Z3 with
    static member Simplify(RealExpr f, [<ParamArray>] options: (string * _) []) = simplify(f, options)