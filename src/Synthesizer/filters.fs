namespace Synthesizer

module Filter =
    open System

    let addTwoWaves (x:List<float>) (y:List<float>) ratio = 
        let mutable output = List.empty
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
        output

    let makeOverdrive multiplicator (x:List<float>) =
        [for i in x do 
            if i < (-1. * multiplicator * 256.) then (-1. * multiplicator * 256.) else
            if i > (1. * multiplicator  * 256.) then (1. * multiplicator * 256.) else
            i]

    let changeAmplitude multiplicator (x:List<float>) =
        x |> List.map (( * ) multiplicator)

    let createEcho (x:List<float>) (startIndex:int) (endIndex:int) (delay:float) (nbEcho:int) = //takes the whole sound and echoes it
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
            returnValue <- addTwoWaves returnValue output[i] 0.66
        let silence = [for i in 0 .. (startIndex - 1) do 0.]
        returnValue <- List.append silence returnValue
        addTwoWaves x returnValue 0.66

    let createFlanger (data:List<float>) start ending delay (rate:float) repNumber = 
        let mutable dela = delay
        let mutable rep = repNumber
        let mutable actualData = data
        while rep > 0 do
            actualData <- createEcho actualData start ending dela 1
            dela <- dela + delay/rate
            rep <- rep - 1
        actualData

    let cutCorners (data:List<float>) limit =
        let step = 1. / float limit
        let startVals = List.map2(fun x i -> x * step * i) data[..limit-1] [1. .. float limit]
        let endVals = List.map2(fun x i -> x * step * i) data[data.Length-limit..] [float limit .. -1. .. 1.]

        List.append (List.append startVals data[limit .. data.Length-limit-1]) endVals

    let cutStart (data:float[]) (sampleRate:float) time = 
        data[int (sampleRate * time) .. data.Length]


    let cutEnd (data:float[]) (sampleRate:float) time = 
        data[0 .. data.Length - int (sampleRate * time)-1] //need to add another time for the end