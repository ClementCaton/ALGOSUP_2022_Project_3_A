namespace Synthesizer

module Filter =
    open System



    /// <summary>
    /// Changes the amplitude of the inputed audio data
    /// </summary>
    /// <param name="multiplicator">Ratio at which the amplitude needs to be changed at</param>
    /// <param name="data">Audio data</param>
    /// <returns>Audio data with the new amplitude</returns>
    
    let ChangeAmplitude (multiplicator:float) (data:List<float>) =
        data |> List.map (( * ) multiplicator)



    /// <summary>
    /// Additions two waves with ratio between them
    /// </summary>
    /// <param name="ratio">The ratio between the two waves</param>
    /// <param name="dataX">First audio input</param>
    /// <param name="dataY">Second audio input</param>
    /// <returns>The sum of the two inputed audios</returns>
    
    let AddTwoWaves (ratio:float) (dataX:List<float>) (dataY:List<float>) =
        let mutable output = List.empty
        if not (dataX.Length = dataY.Length) then
            let diff = Math.Abs(dataX.Length - dataY.Length)
            let endArray = [for i in [0 .. diff] do 0.0]
            if dataX.Length > dataY.Length then
                let newY = List.append dataY endArray
                output <- List.init dataX.Length (fun i -> (dataX[i] * ratio) + (newY[i] * (1.-ratio)))
            else 
                let newX = List.append dataX endArray
                output <- List.init dataY.Length (fun i -> (newX[i] * ratio) + (dataY[i] * (1.-ratio)))
        else 
            output <- List.init dataX.Length (fun i -> (dataX[i] * ratio) + (dataY[i] * (1.-ratio)))
        output



    /// <summary>
    /// Repeats the inputed audio data with a preset delay and changes the amplitude every new repetition
    /// </summary>
    /// <param name="nbEcho">Number of times the audio should be repeated</param>
    /// <param name="decay">How much the amplitude of the echos should be changed (multiplied)</param>
    /// <param name="delay">The delay for each new repetition (starts at 0, NOT at the end of the audio)</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="dryData">The original audio</param>
    /// <returns>Repeated audio</returns>
    
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
    /// Creates a reverberating effect
    /// </summary>
    /// <param name="delayRatio">At which point the sound should be repeated (1s sound with 0.6 delay = 0.6s delay)</param>
    /// <param name="minAmpRatio">Amplitude at which the sound should not be repeated anymore</param>
    /// <param name="decay">How much the amplitude of the echos should be changed (multiplied)</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="dryData">The original audio</param>
    /// <returns>Filtered audio</returns>
    
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
    /// Chreats an echo sound effect
    /// </summary>
    /// <param name="nbEcho">Number of times the audio should be repeated</param>
    /// <param name="decay">How much the amplitude of the echos should be changed (multiplied)</param>
    /// <param name="delay">The delay for each new repetition (starts at the end of the audio)</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="dryData">The original audio</param>
    /// <returns>Filtered audio</returns>
    
    let Echo (nbEcho:int) (decay:float) (delay:float) (sampleRate:float) (dryData:List<float>) =
        Repeater nbEcho decay (float dryData.Length / sampleRate + delay) sampleRate dryData

    //TODO: let chorus



    /// <summary>
    /// Adds a sweeping sound effect to the audio
    /// </summary>
    /// <param name="Delay at which the effect should start at (in ms)"></param>
    /// <param name="speed">How fast should the effect get stronger</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="bpm">Beats per minute</param>
    /// <param name="dryData">The original audio</param>
    /// <returns>Filtered audio</returns>
    
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
    /// Creates a custom pattern for the inputed audios amplitude to follow
    /// </summary>
    /// <param name="dataPoints0">Coordinates for the envelope to follow [(time1 en s, amp1); (time2 en s, amp2)]</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="data">Audio data</param>
    /// <returns>Edited sound</returns>
    
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
    
    let Envelope (sustain:float) (attack:float) (hold0:float) (decay0:float) (release0:float) (sampleRate:float) (data:List<float>) = //release substracts from hold because I don't have the data for the release periode
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
    
    let LFO_AM (frequency:float) (minAmplitude:float) (maxAmplitude:float) (sampleRate:float) (data:List<float>) =
        let oscillator = FourWaves.SinWave
        let amplitude = (maxAmplitude - minAmplitude) / 2.
        let verticalShift = (maxAmplitude + minAmplitude) / 2.
        data
        |> List.mapi (fun i x ->
            let t = float i / float sampleRate
            x * (oscillator frequency amplitude verticalShift 0. t)
        )

    let LFO_FM (modWave:List<float>) (multiplicator:float) (data:List<float>) =
        
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
    
    let LowPass (sampleRate:float) (cutoffFreq:float) (data:List<float>) =
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
    
    let HighPass (sampleRate:float) (cutoffFreq:float) (data:List<float>) =
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
    
    let BandPass (sampleRate:float) (lowFreq:float) (highFreq:float) (data:List<float>) = 
        data |> LowPass sampleRate lowFreq |> HighPass sampleRate highFreq



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    let RejectBand (sampleRate:float) (lowFreq:float) (highFreq:float) (data:List<float>) = 
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
        let mutable output = data
        filterList |> List.map (fun func -> 
            output <- func output
        ) |> ignore
        output
