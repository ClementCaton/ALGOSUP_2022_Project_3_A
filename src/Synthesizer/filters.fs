namespace Synthesizer

module Filter =

    let makeOverdrive multiplicator x =
        if x < (-1. * multiplicator) then (-1. * multiplicator) else
        if x > 1. * multiplicator then 1. * multiplicator else
        x

    let changeAmplitude multiplicator (x:List<float>) =
        x |> List.map ((*) multiplicator)

    let createEcho (startIndex:int) (endIndex:int) (delay:float) (nbEcho:int) (x:List<float>) = //takes the whole sound and echoes it
        //TODO: REPLACE THIS BY THE REAL SILENCE
        let silence = [for i in 1. .. delay -> 0.]
        let mutable output = List.empty
        let mutable echo = x[startIndex..endIndex]
        for i in 1..nbEcho do
            echo <- changeAmplitude (float (nbEcho-1)/ float nbEcho) echo
            output <- List.append output silence
            output <- List.append output (echo)
            //output <- List.append output [SILENCE FOR DELAY]
        output


    let cutCorners (data:float[]) limit =
        let step = 1. / float limit
        let startVals = Array.map2(fun x i -> x * step * i) data[..limit-1] [|1. .. float limit|]
        let endVals = Array.map2(fun x i -> x * step * i) data[data.Length-limit..] [|float limit .. -1. .. 1.|]

        Array.append (Array.append startVals data[limit .. data.Length-limit-1]) endVals