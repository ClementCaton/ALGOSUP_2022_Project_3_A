namespace Synthesizer

module Filter =
    open System

    let makeOverdrive multiplicator (x:List<float>) =
        [for i in x do 
            if i < (-1. * multiplicator * 256.) then (-1. * multiplicator * 256.) else
            if i > (1. * multiplicator  * 256.) then (1. * multiplicator * 256.) else
            i]

    let changeAmplitude multiplicator (x:List<float>) =
        x |> List.map (( * ) multiplicator)

    let addTwoWaves ratio (y:List<float>) (x:List<float>) = 
        let mutable output = List.empty
        let mutable oldMax = x |> List.max
        if (oldMax < (y |> List.max)) then (oldMax <- (y |> List.max))

        if not (x.Length = y.Length) then
            let diff = Math.Abs(x.Length - y.Length)
            let endArray = [for i in [0 .. diff] do 0.0]
            if x.Length > y.Length then
                let newY = List.append y endArray
                output <- List.init x.Length (fun i -> (x[i] * ratio) + (newY[i] * (1.-ratio)))
            else 
                let newX = List.append x endArray
                output <- List.init y.Length (fun i -> (newX[i] * ratio) + (y[i] * (1.-ratio)))
        else 
            output <- List.init x.Length (fun i -> (x[i] * ratio) + (y[i] * (1.-ratio)))
        output <- changeAmplitude (1./(output|>List.max)) output
        output <- changeAmplitude oldMax output
        makeOverdrive 1. output

    let createEcho (startIndex:int) (endIndex:int) (delay:float) (nbEcho:int) (x:List<float>) = //takes the whole sound and echoes it
        let silenceDelay = [for i in 0. .. delay do 0.]
        //let silenceEcho = [for i in 0 .. ( endIndex - startIndex ) do 0.]
        let echoSample = x[startIndex..endIndex]

        let mutable (output:List<List<float>>) = List.empty
        let mutable buffer = List.empty

        for i in [0 .. nbEcho] do
            buffer <- List.empty
            for a in [0 .. i] do
                buffer <- buffer |> List.append silenceDelay
                //buffer <- buffer |> List.append silenceEcho
            buffer <- List.append buffer echoSample
            output <- output @ [buffer]

        let mutable returnValue = output[0]
        for i in [(output.Length - 1).. -1 ..1] do 
            returnValue <- addTwoWaves 0.66 output[i] returnValue
        let silence = [for i in 0 .. (startIndex - 1) do 0.]
        returnValue <- List.append silence returnValue
        addTwoWaves 0.66 returnValue x
