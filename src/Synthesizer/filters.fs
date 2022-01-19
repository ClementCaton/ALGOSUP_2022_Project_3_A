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

    // let createEcho (startIndex:int) (endIndex:int) (delay:float) (nbEcho:int) (x:List<float>) = //takes the whole sound and echoes it
    //     let silenceDelay = [for i in 0. .. delay do 0.]
    //     //let silenceEcho = [for i in 0 .. ( endIndex - startIndex ) do 0.]
    //     let echoSample = x[startIndex..endIndex]

    //     let mutable (output:List<List<float>>) = List.empty
    //     let mutable buffer = List.empty

    //     for i in [0 .. nbEcho] do
    //         buffer <- List.empty
    //         for a in [0 .. i] do
    //             buffer <- buffer |> List.append silenceDelay
    //             //buffer <- buffer |> List.append silenceEcho
    //         buffer <- List.append buffer echoSample
    //         output <- output @ [buffer]

    //     let mutable returnValue = output[0]
    //     for i in [(output.Length - 1).. -1 ..1] do 
    //         returnValue <- addTwoWaves returnValue output[i] 0.66
    //     let silence = [for i in 0 .. (startIndex - 1) do 0.]
    //     returnValue <- List.append silence returnValue
    //     addTwoWaves x returnValue

    let cutCorners (data:List<float>) limit =
        let step = 1. / float limit
        let startVals = List.map2(fun x i -> x * step * i) data[..limit-1] [1. .. float limit]
        let endVals = List.map2(fun x i -> x * step * i) data[data.Length-limit..] [float limit .. -1. .. 1.]

        List.append (List.append startVals data[limit .. data.Length-limit-1]) endVals

    let createDelay (data:List<float>) (start:float) (ending:float) (delay:float) sampleRate=
        let (newData) = [
            for i in (int (start*float sampleRate)) .. (int(ending*float sampleRate)) do 
                if i < data.Length then
                    yield data.[i]
        ]
        // printfn "%A %A %A" (int (delay * float sampleRate)) delay sampleRate
        let mutable inc = 0
        let fData = [
            for i in 0 .. data.Length-1 do
                if i > (int (start * float sampleRate)+(int (delay * float sampleRate))) && i < (int (ending*float sampleRate)+(int (delay * float sampleRate))) then
                    // printfn "%A %A %A %A" newData.Length inc  i data.Length
                    yield (newData.[inc] + data.[i])/2.
                    inc <- inc + 1
                else
                    yield data.[i]
        ]
        fData

    
    let createFlanger (data:List<float>) (start:float) (ending:float) (delay:float) (rate:float) repNumber sampleRate = 
        let mutable dela = delay
        let mutable rep = repNumber
        let mutable actualData = data
        while rep > 0 do
            actualData <- createDelay data start ending dela sampleRate
            dela <- dela + dela/rate
            rep <- rep - 1
        actualData


    let reverb (dryData:List<float>) (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) = 
        let rec revebInner (dryData:List<float>) (wetData:List<float>) (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) =   // This is also echo
            if nbEcho=0 then
                Utility.add [dryData; wetData]
            else
                let silence = createSoundData(frequency0 = 0, duration0 = (Seconds (delay * float nbEcho)), bpm0 = 114).create(Silence)
                let updatedWetData = Utility.add [wetData; List.concat [silence ; changeAmplitude decay dryData]]
                revebInner dryData updatedWetData (nbEcho-1) decay delay sampleRate

        revebInner dryData [] nbEcho decay delay sampleRate


    