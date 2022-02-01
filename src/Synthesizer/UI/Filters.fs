namespace Synthesizer

module Filter =
    open System

    let changeAmplitude multiplicator (x:List<float>) =
        x |> List.map (( * ) multiplicator)


    let addTwoWaves ratio (x:List<float>) (y:List<float>) =
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

    let addModulation ratio (x:List<float>) (y:List<float>) = 
        let mutable oldMax = x |> List.max
        if y |> List.max > oldMax then oldMax <- y |> List.max
        let mutable output = (addTwoWaves ratio x y)
        output <- changeAmplitude (1./(output|>List.max)) output
        output <- changeAmplitude oldMax output
        Utility.Overdrive 1. output


    let Repeater (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (dryData:List<float>) = 
        let rec RepeaterInner (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (wetData:List<float>) (dryData:List<float>) =   // This is also echo
            if nbEcho=0 then
                Utility.add [dryData; wetData]
            else
                let silence = SoundData(frequency0 = 0, duration0 = (Seconds (delay*float nbEcho)), bpm0 = 114).create(Silence)
                let updatedWetData = Utility.add [wetData; List.concat [silence ; changeAmplitude decay dryData]]
                RepeaterInner (nbEcho-1) decay delay sampleRate updatedWetData dryData

        RepeaterInner nbEcho decay delay sampleRate [] dryData

    let Reverb (delayRatio:float) (minAmpRatio:float) (decay:float) (sampleRate:float) (dryData:List<float>) =
        let delay = (float dryData.Length * delayRatio) / sampleRate
        let rec calcSteps minAmp decay current step =
            if minAmp >= current then
                step
            else
                calcSteps minAmp decay (current*decay) (step+1)

        let nbEcho = calcSteps minAmpRatio decay 1. 0

        Repeater nbEcho decay delay sampleRate dryData
    
    let Echo (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (dryData:List<float>) =
        Repeater nbEcho decay (float dryData.Length / sampleRate + delay) sampleRate dryData

    //TODO: let chorus


    let flanger (delay:float) (speed:float) (sampleRate:float) (dryData:List<float>) =
        let step = speed/1000.*sampleRate
        let silence = SoundData(frequency0 = 0, sampleRate0 = sampleRate,  duration0 = (Seconds (delay/1000.)), bpm0 = 114).create(Silence)


        let rec flangerInner (step:float) (rate:int) (initialRate:int)  current (dry:List<float>) (wet:List<float>) =
            if wet.Length >= dry.Length then wet

            elif Math.Floor(float current%step) = 0 then
                let addition = [for i in 0 .. (rate) -> dry[current]]
                flangerInner step (rate+initialRate) initialRate (current+1) dry (wet @ addition)

            else flangerInner step rate initialRate (current+1) dry (wet @ [dry[current]])
        
        let wetData = flangerInner step 1 1 0 dryData[silence.Length..] []

        Utility.add [dryData; (silence @ wetData)]

    let CustomEnvelope (dataPoints0: List<float * float>) (sampleRate:float) (data:List<float>) =
            let dataPoints = if (fst dataPoints0[0] <> 0.) then (0., 0.) :: dataPoints0 else dataPoints0
    
            let calcSegment (fromTime:float) (toTime:float) fromAmp toAmp =
                let step = (toAmp - fromAmp) / (toTime - fromTime)
                List.mapi(fun i flatPoint -> (flatPoint * (fromAmp + (float step * float i)))) data[int fromTime .. int toTime]
    
            let output = List.map2(fun fromT toT -> calcSegment (sampleRate * (fst fromT)) (sampleRate * (fst toT)) (snd fromT) (snd toT)) dataPoints[ .. dataPoints.Length-2] dataPoints[1 ..]
    
            output |> List.concat
    
    let envelope sustain attack hold0 decay0 release0 (sampleRate:float) (data:List<float>) = //release substracts from hold because I don't have the data for the release periode
        let hold = hold0 + attack
        let decay = hold + decay0
        let release = (float data.Length/float sampleRate) - release0
        
        

        CustomEnvelope ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); (release, sustain); ((float data.Length/float sampleRate), 0.)]) sampleRate data //error here
        //pinchAmp data ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); ((float data.Length/float sampleRate), sustain); (release, 0.)]) sampleRate  //error here


    let LFO_AM frequency minAmplitude maxAmplitude sampleRate data =
        let oscillator = fourWaves.sinWave
        let amplitude = (maxAmplitude - minAmplitude) / 2.
        let verticalShift = (maxAmplitude + minAmplitude) / 2.
        data
        |> List.mapi (fun i x ->
            let t = float i / float sampleRate
            x * (oscillator frequency amplitude verticalShift 0. t)
        )

    let LFO_FM frequency deltaFreq sampleRate (data:List<float>) =
        failwith "Not working yet"
        let Ac = 1. // Carrier's amplitude
        let fc = frequency // Carrier's frequency
        let fd = deltaFreq // Frequency deviation = frequency modulator's sensitivity * data's amplitude

        let integrate N (Xs:List<float>) =
            Xs
            |> List.take (N+1)
            |> List.sum
            |> ( * ) (float N) // (float N / float sampleRate)

        List.init (List.length data) (fun i ->
            let t = float i / sampleRate
            Ac * cos ( 2. * Math.PI * (fc * t + fd * integrate i data))
            // https://en.wikipedia.org/wiki/Frequency_modulation#Theory
        )

    let lowPass sampleRate cutoffFreq (data:List<float>) =
        let RC = 1. / (2. * Math.PI * cutoffFreq)
        let dt = 1. / sampleRate
        let alpha = dt / (RC + dt)
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

    let highPass sampleRate cutoffFreq (data:List<float>) =
        let RC = 1. / (2. * Math.PI * cutoffFreq)
        let dt = 1. / sampleRate
        let alpha = RC / (RC + dt)

        let mutable last = data.[0]
        [last] @ (
            (data.[1..(List.length data - 1)], data.[0..(List.length data - 2)])
            ||> List.map2 ( - )
            |> List.map (fun x ->
                last <- alpha * (last + x)
                last
            )
        )

    let bandPass sampleRate lowFreq highFreq (data:List<float>) = 
        data |> lowPass sampleRate lowFreq |> highPass sampleRate highFreq

    let rejectBand sampleRate lowFreq highFreq (data:List<float>) = 
        let lowPassData = highPass sampleRate lowFreq data
        let highPassData = lowPass sampleRate highFreq data
        Utility.add [lowPassData; highPassData]

    let ApplyFilters filterList data =
        let mutable output = List.empty
        filterList |> List.map (fun func -> 
            output <- func data
        ) |> ignore
        output