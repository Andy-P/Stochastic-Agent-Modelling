// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "TickData.fs"
#load "OrderBook.fs"

open Quantix.Simulator.TickData
open Quantix.Simulator.OrderBook
open System

// sort tests
let os = [{created = DateTime.MinValue.AddDays(3.); isBuy=true;  size=1; price=10. }
          {created = DateTime.MinValue.AddDays(1.); isBuy=false; size=1; price=10. }
          {created = DateTime.MinValue.AddDays(2.); isBuy=true;  size=1; price=10. }
          {created = DateTime.MinValue.AddDays(3.); isBuy=true;  size=1; price=10. }]
let sortedOs = os |> List.sort

let notEqual = (os.[0] = os.[1]) // should be false
let equal    = (os.[0] = os.[3]) // should be true


let mkt = initialMkt 5.
let pNode = emptyPriceNode