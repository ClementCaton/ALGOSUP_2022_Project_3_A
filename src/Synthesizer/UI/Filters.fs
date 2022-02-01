namespace Synthesizer

module Filter =
    open System

    let ChangeAmplitude multiplicator (x:List<float>) =
        x |> List.map (( * ) multiplicator)


    let AddTwoWaves ratio (x:List<float>) (y:List<float>) =
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

    //! Should be modified
    // let Reverb (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (dryData:List<float>) = 
    //     let rec RevebInner (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (wetData:List<float>) (dryData:List<float>) =   // This is also echo
    //         if nbEcho=0 then
    //             Utility.add [dryData; wetData]
    //         else
    //             let silence = SoundData(frequency0 = 0, duration0 = (Seconds (delay * float nbEcho)), bpm0 = 114).create(Silence)
    //             let updatedWetData = Utility.add [wetData; List.concat [silence ; ChangeAmplitude decay dryData]]
    //             r=RevebInner (nbEcho-1) decay delay sampleRate updatedWetData dryData

    //     RevebInner nbEcho decay delay sampleRate [] dryData



    //! WIP
    let Flanger (delay:float) (speed:float) (sampleRate:float) (dryData:List<float>) =
        let step = speed/1000.*sampleRate
        let silence = SoundData(frequency0 = 0, sampleRate0 = sampleRate,  duration0 = (Seconds (delay/1000.)), bpm0 = 114).Create(Silence)


        let rec FlangerInner (step:float) (rate:int) (initialRate:int)  current (dry:List<float>) (wet:List<float>) =
            if wet.Length >= dry.Length then wet

            elif Math.Floor(float current%step) = 0 then
                // printfn $"{float wet.Length / float dry.Length}"

                let Addition = [for i in 0 .. (rate) -> dry[current]]
                FlangerInner step (rate+initialRate) initialRate (current+1) dry (wet @ Addition)

            else FlangerInner step rate initialRate (current+1) dry (wet @ [dry[current]])
        
        let wetData = FlangerInner step 1 1 0 dryData[silence.Length..] []

        Utility.Add [dryData; (silence @ wetData)]

    let CustomEnvelope (dataPoints0: List<float * float>) (sampleRate:float) (data:List<float>) =
            let dataPoints = if (fst dataPoints0[0] <> 0.) then (0., 0.) :: dataPoints0 else dataPoints0
    
            let CalcSegment (fromTime:float) (toTime:float) fromAmp toAmp =
                let step = (toAmp - fromAmp) / (toTime - fromTime)
                List.mapi(fun i flatPoint -> (flatPoint * (fromAmp + (float step * float i)))) data[int fromTime .. int toTime]
    
            let output = List.map2(fun fromT toT -> CalcSegment (sampleRate * (fst fromT)) (sampleRate * (fst toT)) (snd fromT) (snd toT)) dataPoints[ .. dataPoints.Length-2] dataPoints[1 ..]
    
            output |> List.concat
    
    let Envelope sustain attack hold0 decay0 release0 (sampleRate:float) (data:List<float>) = //release substracts from hold because I don't have the data for the release periode
        let hold = hold0 + attack
        let decay = hold + decay0
        let release = (float data.Length/float sampleRate) - release0
        
        

        CustomEnvelope ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); (release, sustain); ((float data.Length/float sampleRate), 0.)]) sampleRate data //error here
        //PinchAmp data ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); ((float data.Length/float sampleRate), sustain); (release, 0.)]) sampleRate  //error here


    let LFO_AM frequency minAmplitude maxAmplitude sampleRate data =
        let oscillator = FourWaves.SinWave
        let amplitude = (maxAmplitude - minAmplitude) / 2.
        let verticalShift = (maxAmplitude + minAmplitude) / 2.
        data
        |> List.mapi (fun i x ->
            let t = float i / float sampleRate
            x * (oscillator frequency amplitude verticalShift 0. t)
        )

    let LFO_FM frequency deltaFreq sampleRate (data:List<float>) =
        failwith "Not working yet"
        let ac = 1. // Carrier's amplitude
        let fc = frequency // Carrier's frequency
        let fd = deltaFreq // frequency deviation = frequency modulator's sensitivity * data's amplitude

        let Integrate n (xs:List<float>) =
            xs
            |> List.take (n+1)
            |> List.sum
            |> ( * ) (float n) // (float n / float sampleRate)

        List.init (List.length data) (fun i ->
            let t = float i / sampleRate
            ac * cos ( 2. * Math.PI * (fc * t + fd * Integrate i data))
            // https://en.wikipedia.org/wiki/Frequency_modulation#Theory
        )

    let LowPass sampleRate cutoffFreq (data:List<float>) =
        let rc = 1. / (2. * Math.PI * cutoffFreq)
        let dt = 1. / sampleRate
        let alpha = dt / (rc + dt)
        let alpha2 = 1. - alpha

        let mutable last = alpha * data.[0]
        [last] @ (
            data
            |> List.tail
            |> List.map (fun x ->
                last <- alpha * x + alpha2 * last
                last
            )
        )

    let HighPass sampleRate cutoffFreq (data:List<float>) =
        let rc = 1. / (2. * Math.PI * cutoffFreq)
        let dt = 1. / sampleRate
        let alpha = rc / (rc + dt)

        let mutable last = data.[0]
        [last] @ (
            (data.[1..(List.length data - 1)], data.[0..(List.length data - 2)])
            ||> List.map2 ( - )
            |> List.map (fun x ->
                last <- alpha * (last + x)
                last
            )
        )

    let BandPass sampleRate lowFreq highFreq (data:List<float>) = 
        data |> LowPass sampleRate lowFreq |> HighPass sampleRate highFreq

    let RejectBand sampleRate lowFreq highFreq (data:List<float>) = 
        let lowPassData = HighPass sampleRate lowFreq data
        let highPassData = LowPass sampleRate highFreq data
        Utility.Add [lowPassData; highPassData]
