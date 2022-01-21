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

    // enveloppe stuff
    let pinchAmp (data:List<float>) (dataPoints0: List<float * float>) (sampleRate:float)=
        let dataPoints = if (fst dataPoints0[0] <> 0.) then (0., 0.) :: dataPoints0 else dataPoints0

        let calcSegment (fromTime:float) (toTime:float) fromAmp toAmp =
            let step = (toAmp - fromAmp) / (toTime - fromTime)
            List.mapi(fun i flatPoint -> (flatPoint * (fromAmp + (float step * float i)))) data[int fromTime .. int toTime]

        let output = List.map2(fun fromT toT -> calcSegment (sampleRate * (fst fromT)) (sampleRate * (fst toT)) (snd fromT) (snd toT)) dataPoints[ .. dataPoints.Length-2] dataPoints[1 ..]

        output |> List.concat


    let enveloppe (data:List<float>) (sampleRate:float) sustain attack hold0 decay0 release0 = //release substracts from hold because I don't have the data for the release periode
        let hold = hold0 + attack
        let decay = hold + decay0
        let release = (float data.Length/float sampleRate) - release0

        pinchAmp data ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); (release, sustain); ((float data.Length/float sampleRate), 0.)]) sampleRate  //error here
        //pinchAmp data ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); ((float data.Length/float sampleRate), sustain); (release, 0.)]) sampleRate  //error here

    let rec reverb (dryData:List<float>) (wetData:List<float>) (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) =    // This is also echo
        if nbEcho=0 then
            Utility.add [dryData; wetData]
        else
            let silence = createSoundData(frequency0 = 0, duration0 = (Seconds (delay * float nbEcho)), bpm0 = 114).create(Silence)
            let updatedWetData = Utility.add [wetData; List.concat [silence ; changeAmplitude decay dryData]]
            reverb dryData updatedWetData (nbEcho-1) decay delay sampleRate
