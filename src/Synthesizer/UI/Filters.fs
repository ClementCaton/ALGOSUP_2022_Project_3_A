namespace Synthesizer

module Filter =
    open System



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let ChangeAmplitude multiplicator (x:List<float>) =
        x |> List.map (( * ) multiplicator)



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
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



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let Repeater (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (dryData:List<float>) = 
        let rec RepeaterInner (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (wetData:List<float>) (dryData:List<float>) =   // This is also echo
            if nbEcho=0 then
                Utility.AddMean [dryData; wetData]
            else
                let silence = SoundData(frequency0 = 0, duration0 = (Seconds (delay*float nbEcho)), bpm0 = 114).Create(Silence)
                let updatedWetData = Utility.AddMean [wetData; List.concat [silence ; ChangeAmplitude decay dryData]]
                RepeaterInner (nbEcho-1) decay delay sampleRate updatedWetData dryData

        RepeaterInner nbEcho decay delay sampleRate [] dryData



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let Reverb (delayRatio:float) (minAmpRatio:float) (decay:float) (sampleRate:float) (dryData:List<float>) =
        let delay = (float dryData.Length * delayRatio) / sampleRate
        let rec calcSteps minAmp decay current step =
            if minAmp >= current then
                step
            else
                calcSteps minAmp (decay*decay) (current*decay) (step+1)

        let nbEcho = calcSteps minAmpRatio decay 1. 0

        Repeater nbEcho decay delay sampleRate dryData
    


    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let Echo (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (dryData:List<float>) =
        Repeater nbEcho decay (float dryData.Length / sampleRate + delay) sampleRate dryData

    //TODO: let chorus



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let Flanger (delay:float) (speed:float) (sampleRate:float) (bpm:float) (dryData:List<float>) =
        let step = speed/1000.*sampleRate
        let silence = SoundData(frequency0 = 0, sampleRate0 = sampleRate,  duration0 = (Seconds (delay/1000.)), bpm0 = bpm).Create(Silence)

        let rec FlangerInner (step:float) (rate:int) (initialRate:int)  current (dry:List<float>) (wet:List<float>) =
            if wet.Length >= dry.Length then wet

            elif Math.Floor(float current%step) = 0 then
                let addition = [for i in 0 .. (rate) -> dry[current]]
                FlangerInner step (rate+initialRate) initialRate (current+1) dry (wet @ addition)

            else FlangerInner step rate initialRate (current+1) dry (wet @ [dry[current]])
        
        let wetData = FlangerInner step 1 1 0 dryData[silence.Length..] []

        Utility.AddMean [dryData; (silence @ wetData)]



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let CustomEnvelope (dataPoints0: List<float * float>) (sampleRate:float) (data:List<float>) =
        let dataPoints = if (fst dataPoints0[0] <> 0.) then (0., 0.) :: dataPoints0 else dataPoints0

        let calcSegment (fromTime:float) (toTime:float) fromAmp toAmp =
            let step = (toAmp - fromAmp) / (toTime - fromTime)
            List.mapi(fun i flatPoint -> (flatPoint * (fromAmp + (float step * float i)))) data[int fromTime .. int toTime]

        let output = List.map2(fun fromT toT -> calcSegment (sampleRate * (fst fromT)) (sampleRate * (fst toT)) (snd fromT) (snd toT)) dataPoints[ .. dataPoints.Length-2] dataPoints[1 ..]

        output |> List.concat
    


    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let Envelope sustain attack hold0 decay0 release0 (sampleRate:float) (data:List<float>) = //release substracts from hold because I don't have the data for the release periode
        let hold = hold0 + attack
        let decay = hold + decay0
        let release = (float data.Length/float sampleRate) - release0
        
        CustomEnvelope ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); (release, sustain); ((float data.Length/float sampleRate), 0.)]) sampleRate data //error here
        //PinchAmp data ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); ((float data.Length/float sampleRate), sustain); (release, 0.)]) sampleRate  //error here



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let LFO_AM frequency minAmplitude maxAmplitude sampleRate data =
        let oscillator = FourWaves.SinWave
        let amplitude = (maxAmplitude - minAmplitude) / 2.
        let verticalShift = (maxAmplitude + minAmplitude) / 2.
        data
        |> List.mapi (fun i x ->
            let t = float i / float sampleRate
            x * (oscillator frequency amplitude verticalShift 0. t)
        )

    let LFO_FM (modWave:List<float>) (data:List<float>) (multiplicator:float) =
        
        let getShift (startAmp:float) (endAmp:float) (nStep:float)=
            let fullRange = endAmp - startAmp
            let step = fullRange / nStep
            [for i in startAmp .. step .. endAmp do yield i]


        let rec LFO_FM_inner (modWave:list<float>) (dryData:list<float>) (wetData0:list<float>) =
            if modWave.Length<dryData.Length then failwith "modWave too short for LFO FM! modWave must have at least the longer of the dryData!"
            if dryData.Length<=2 then wetData0
            else 
                //printfn $"{dryData.Length}"

                let delta = modWave[0] * multiplicator

                let wetData = 
                    match None with 
                    | _ when delta>0. -> wetData0 @ (getShift dryData[0] dryData[1] delta)
                    | _ -> wetData0 @ [dryData[0]]

            
                match None with
                | _ when delta>0. -> LFO_FM_inner modWave[1..] dryData[1..] wetData
                | _ when delta<0. -> LFO_FM_inner modWave[1..] dryData[(int (Math.Abs (Math.Floor delta)))..] wetData
                | _ -> LFO_FM_inner modWave[1..] dryData[1..] wetData 

        LFO_FM_inner modWave data []


    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let LowPass sampleRate cutoffFreq (data:List<float>) =
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



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let HighPass sampleRate cutoffFreq (data:List<float>) =
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



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let BandPass sampleRate lowFreq highFreq (data:List<float>) = 
        data |> LowPass sampleRate lowFreq |> HighPass sampleRate highFreq



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let RejectBand sampleRate lowFreq highFreq (data:List<float>) = 
        let lowPassData = HighPass sampleRate lowFreq data
        let highPassData = LowPass sampleRate highFreq data
        Utility.AddMean [lowPassData; highPassData]


    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let ApplyFilters filterList data =
        let mutable output = List.empty
        filterList |> List.map (fun func -> 
            output <- func data
        ) |> ignore
        output
