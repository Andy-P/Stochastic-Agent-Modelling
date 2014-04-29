namespace Quantix.Simulator

open System

module TickData =

    type Trade = { time:DateTime; price:float; size:int; }

    [<CustomEquality; CustomComparison>]
    type Quote =
        { time:DateTime; price:float; size:int; depth:int }

        override x.Equals(yobj) =
            match yobj with
            | :? Quote as y -> (x.price = y.price) &&  (x.size = y.size) && (x.depth = y.depth)
            | _ -> false

        override x.GetHashCode() = hash x.price

        interface System.IComparable with
          member x.CompareTo yobj =
              match yobj with
              | :? Quote as y -> 
                    if   x.time  <> y.time  then x.time.CompareTo(y.time)
                    elif x.price <> y.price then x.price.CompareTo(y.price)
                    elif x.size  <> y.size  then x.size.CompareTo(y.size)
                    else                         x.depth.CompareTo(y.depth)
              | _ -> invalidArg "yobj" "cannot compare values of different types"

    type Tick = 
        | Bid   of Quote
        | Ask   of Quote
        | Empty of Quote
        | Trade of Trade

        member x.price () = 
             match x with
             | Bid x | Ask x | Empty x -> x.price
             | Trade x -> x.price

        member x.size () = 
             match x with
             | Bid x | Ask x | Empty x -> x.size
             | Trade x -> x.size

        member x.depth () = 
             match x with
             | Bid x | Ask x | Empty x -> x.depth
             | Trade x -> 0

        member x.time () = 
             match x with
             | Bid x | Ask x | Empty x -> x.time
             | Trade x -> x.time

        override x.ToString () =
            match x with
            | Bid x   -> sprintf " %s Bid-%i %i @ %.0f"    (x.time.ToUniversalTime().ToString("o")) x.depth x.size x.price
            | Ask x   -> sprintf " %s Ask-%i %i @ %.0f"    (x.time.ToUniversalTime().ToString("o")) x.depth x.size x.price
            | Empty x -> sprintf " %s Empty-%i %i @ %.0f"  (x.time.ToUniversalTime().ToString("o")) x.depth x.size x.price
            | Trade x -> sprintf " %s Trade %i @ %.0f"     (x.time.ToUniversalTime().ToString("o")) x.size         x.price  
