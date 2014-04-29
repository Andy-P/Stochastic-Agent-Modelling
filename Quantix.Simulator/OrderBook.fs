namespace Quantix.Simulator

open System
open Quantix.Simulator.TickData
 
module OrderBook = 

    [<CustomEquality; CustomComparison>]
    type Order =
        {created:DateTime; isBuy:bool; price:float; size:int; }

        override x.ToString () =
            let sideStr = if x.isBuy then "Buy" else "Sell"
            sprintf "%s: %s %i @ %.2f" (x.created.ToUniversalTime().ToString("O")) sideStr x.size x.price

        override x.Equals(yobj) =
            match yobj with
            | :? Order as y -> x.created=y.created && x.price=y.price && x.isBuy = y.isBuy && x.size=y.size 
            | _ -> false
                
        override x.GetHashCode() = hash (x.isBuy, x.created, x.price)

        interface System.IComparable with
          member x.CompareTo yobj =
              match yobj with
              | :? Order as y -> 
                    if (x.isBuy <> y.isBuy) then (if x.isBuy then -1 else 1) // buy orders first
                    elif (x.created <> y.created) then x.created.CompareTo(y.created) // then oldest first
                    else x.price.CompareTo(y.price) // then by lowest price
              | _ -> invalidArg "yobj" "cannot compare values of different types"


    type PriceSide = Bid | Offered | Empty
    type QueuePos = int

    [<CustomEquality; CustomComparison>]
    type PriceNode = 
        {updated:DateTime; 
         price:float; 
         bid:int; ask:int; 
         lastSize:int; 
         buyOrders: List<Tuple<QueuePos,Order>>;
         sellOrders:List<Tuple<QueuePos,Order>>;
         depth:int; side:PriceSide}

        override x.ToString () =
            sprintf "%s,%.2f,%d,%d,%d,%d,%s" (x.updated.ToUniversalTime().ToString("O")) x.price x.bid x.ask x.lastSize x.depth (x.side.ToString())

        member x.ToStringNoDate () =
            sprintf "%.2f,%d,%d,%d,%d,%s" x.price x.bid x.ask x.lastSize x.depth (x.side.ToString())

        override x.Equals(yobj) =
            match yobj with
            | :? PriceNode as y -> ((x.price = y.price) && (x.bid = y.bid) && (x.ask = y.ask) &&
                                    (x.lastSize = y.lastSize) && (x.depth = y.depth) && (x.side = y.side))
            | _ -> false

        override x.GetHashCode() = hash x.price

        interface System.IComparable with
          member x.CompareTo yobj =
              match yobj with
              | :? PriceNode as y -> 
                    if (x.updated <> y.updated) then x.updated.CompareTo(y.updated)
                    elif (x.price <> y.price) then x.price.CompareTo(y.price)
                    else if x.side = y.side then 0 elif x.side = PriceSide.Bid then 1 else -1
              | _ -> invalidArg "yobj" "cannot compare values of different types"


    type Market = { 
                updated:DateTime;
                bestBid:float; 
                bestAsk:float; 
                prices:Map<float,PriceNode>; 
                tickSize:float;
                isInit:bool;
                isOpen:bool }

    let initialMkt tickSize = { updated  = System.DateTime.MinValue;
                                bestBid  = Double.MinValue; 
                                bestAsk  = Double.MaxValue; 
                                prices   = Map.empty<float,PriceNode>; 
                                tickSize = tickSize;
                                isInit   = false;
                                isOpen   = false }

    let emptyPriceNode = 
            {updated = System.DateTime.MinValue; 
             price = 0.; 
             bid = 0; ask = 0; 
             lastSize = 0; 
             buyOrders = List.empty<Tuple<QueuePos,Order>>;
             sellOrders = List.empty<Tuple<QueuePos,Order>>;
             depth = 0; side = PriceSide.Empty}

