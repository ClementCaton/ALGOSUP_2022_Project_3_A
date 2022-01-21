namespace Synthesizer

module Filter =
    open System

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
        Utility.makeOverdrive 1. output

    let makeOverdrive multiplicator (x:List<float>) =
        [for i in x do 
            if i < (-1. * multiplicator) then (-1. * multiplicator) else
            if i > (1. * multiplicator) then (1. * multiplicator) else
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

    


    let createDelay (start:float) (ending:float) (delay:float) sampleRate (data:List<float>) =
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

    let createFlanger (start:float) (ending:float) (delay:float) (rate:float) repNumber sampleRate (data:List<float>) = 
        let mutable dela = delay
        let mutable rep = repNumber
        let mutable actualData = data
        while rep > 0 do
            actualData <- createDelay start ending dela sampleRate data
            dela <- dela + dela/rate
            rep <- rep - 1
        actualData

    // enveloppe stuff
    let pinchAmp (dataPoints0: List<float * float>) (sampleRate:float) (data:List<float>) =
        let dataPoints = if (fst dataPoints0[0] <> 0.) then (0., 0.) :: dataPoints0 else dataPoints0

    let rec reverb (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (wetData:List<float>) (dryData:List<float>) =    // This is also echo
        let calcSegment (fromTime:float) (toTime:float) fromAmp toAmp =
            let step = (toAmp - fromAmp) / (toTime - fromTime)
            List.mapi(fun i flatPoint -> (flatPoint * (fromAmp + (float step * float i)))) data[int fromTime .. int toTime]
            reverb (nbEcho-1) decay delay sampleRate updatedWetData dryData

        let output = List.map2(fun fromT toT -> calcSegment (sampleRate * (fst fromT)) (sampleRate * (fst toT)) (snd fromT) (snd toT)) dataPoints[ .. dataPoints.Length-2] dataPoints[1 ..]

        output |> List.concat

//! wut
//    let rec reverb (wetData:List<float>) (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (dryData:List<float>)=    // This is also echo
//        if nbEcho=0 then
//            Utility.add [dryData; wetData]
//        else
//            let silence = createSoundData(frequency0 = 0, duration0 = (Seconds (delay * float nbEcho)), bpm0 = 114).create(Silence)
//            let updatedWetData = Utility.add [wetData; List.concat [silence ; changeAmplitude decay dryData]]
//            reverb updatedWetData (nbEcho-1) decay delay sampleRate dryData


    let enveloppe (sampleRate:float) sustain attack hold0 decay0 release0 (data:List<float>) = //release substracts from hold because I don't have the data for the release periode
        let hold = hold0 + attack
        let decay = hold + decay0
        let release = (float data.Length/float sampleRate) - release0

        pinchAmp ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); (release, sustain); ((float data.Length/float sampleRate), 0.)]) sampleRate data //error here
        //pinchAmp data ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); ((float data.Length/float sampleRate), sustain); (release, 0.)]) sampleRate  //error here


    let lowPass sampleRate cutoffFreq (data:List<float>) =
        let RC = 1. / (2. * Math.PI * cutoffFreq)
        let dt = 1. / sampleRate
        let alpha = dt / (RC + dt)
        let alpha2 = 1. - alpha

        // TODO: Refactorize and make faster
        let mutable y = [alpha * data.[0]]
        let mutable y' = [alpha * data.[0]]
        for x in List.tail data do
            y' <- y' @ [ alpha * x + alpha2 * (List.last y') ]
            if (List.length y') = 10000 then
                y <- y @ y'[1..]
                y' <- [List.last y']
        y @ y'[1..]

    let highPass sampleRate cutoffFreq (data:List<float>) =
        let RC = 1. / (2. * Math.PI * cutoffFreq)
        let dt = 1. / sampleRate
        let alpha = dt / (RC + dt)

        // TODO: Refactorize and make faster
        let mutable y = [data.[0]]
        let mutable y' = [data.[0]]
        for i in 1..(List.length data - 1) do
            y' <- y' @ [ alpha * (List.last y' + data.[i] - data.[i-1]) ]
            if (List.length y') = 10000 then
                y <- y @ y'[1..]
                y' <- [List.last y']
        y @ y'[1..]

