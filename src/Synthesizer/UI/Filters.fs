namespace Synthesizer

module Filter =
    open System

    let ChangeAmplitude multiplicator (x:List<float>) =
        x |> List.map (( * ) multiplicator)


    let AddTwoWaves ratio (x:List<float>) (y:List<float>) =
        let mutable Output = List.empty
        if not (x.Length = y.Length) then
            let Diff = Math.Abs(x.Length - y.Length)
            let EndArray = [for i in [0 .. Diff] do 0.0]
            if x.Length > y.Length then
                let NewY = List.append y EndArray
                Output <- List.init x.Length (fun i -> (x[i] * Ratio) + (NewY[i] * (1.-Ratio)))
            else 
                let NewX = List.append x EndArray
                Output <- List.init y.Length (fun i -> (NewX[i] * Ratio) + (y[i] * (1.-Ratio)))
        else 
            Output <- List.init x.Length (fun i -> (x[i] * Ratio) + (y[i] * (1.-Ratio)))
        Output

    let AddModulation ratio (x:List<float>) (y:List<float>) = 
        let mutable OldMax = x |> List.max
        if y |> List.max > OldMax then OldMax <- y |> List.max
        let mutable Output = (AddTwoWaves Ratio x y)
        Output <- ChangeAmplitude (1./(Output|>List.max)) Output
        Output <- ChangeAmplitude OldMax Output
        Utility.Overdrive 1. Output

    let Reverb (NbEcho:int) (Decay:float) (Delay:float) (SampleRate:float) (DryData:List<float>) = 
        let rec RevebInner (NbEcho:int) (Decay:float) (Delay:float) (SampleRate:float) (WetData:List<float>) (DryData:List<float>) =   // This is also echo
            if NbEcho=0 then
                Utility.add [DryData; WetData]
            else
                let Silence = SoundData(Frequency0 = 0, Duration0 = (Seconds (Delay * float NbEcho)), Bpm0 = 114).create(Silence)
                let UpdatedWetData = Utility.add [WetData; List.concat [Silence ; ChangeAmplitude Decay DryData]]
                r=RevebInner (NbEcho-1) Decay Delay SampleRate UpdatedWetData DryData

        RevebInner NbEcho Decay Delay SampleRate [] DryData



    //! WIP
    let Flanger (Delay:float) (Speed:float) (SampleRate:float) (DryData:List<float>) =
        let Step = Speed/1000.*SampleRate
        let Silence = SoundData(Frequency0 = 0, SampleRate0 = SampleRate,  Duration0 = (Seconds (Delay/1000.)), Bpm0 = 114).create(Silence)


        let rec FlangerInner (Step:float) (Rate:int) (InitialRate:int)  current (Dry:List<float>) (Wet:List<float>) =
            if Wet.Length >= Dry.Length then Wet

            elif Math.Floor(float Current%step) = 0 then
                printfn $"{float wet.Length / float dry.Length}"

                let Addition = [for i in 0 .. (Rate) -> Dry[Current]]
                FlangerInner Step (Rate+InitialRate) InitialRate (Current+1) Dry (Wet @ Addition)

            else FlangerInner Step Rate initialRate (Current+1) Dry (Wet @ [Dry[Current]])
        
        let WetData = FlangerInner Step 1 1 0 DryData[Silence.Length..] []

        Utility.add [DryData; (Silence @ WetData)]

    let CustomEnvelope (DataPoints0: List<float * float>) (SampleRate:float) (Data:List<float>) =
            let DataPoints = if (fst DataPoints0[0] <> 0.) then (0., 0.) :: DataPoints0 else DataPoints0
    
            let CalcSegment (FromTime:float) (ToTime:float) FromAmp ToAmp =
                let Step = (ToAmp - FromAmp) / (ToTime - FromTime)
                List.mapi(fun i FlatPoint -> (FlatPoint * (FromAmp + (float Step * float i)))) data[int FromTime .. int ToTime]
    
            let Output = List.map2(fun FromT toT -> CalcSegment (SampleRate * (fst FromT)) (SampleRate * (fst toT)) (snd FromT) (snd toT)) DataPoints[ .. DataPoints.Length-2] DataPoints[1 ..]
    
            Output |> List.concat
    
    let Envelope Sustain Attack Hold0 Decay0 Release0 (SampleRate:float) (Data:List<float>) = //release substracts from hold because I don't have the data for the release periode
        let Hold = Hold0 + Attack
        let Decay = Hold + Decay0
        let Release = (float Data.Length/float SampleRate) - Release0
        
        

        CustomEnvelope ([(0., 0.); (Attack, 1.); (Hold, 1.); (Decay, Sustain); (Release, Sustain); ((float Data.Length/float SampleRate), 0.)]) SampleRate Data //error here
        //PinchAmp data ([(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); ((float data.Length/float sampleRate), sustain); (release, 0.)]) sampleRate  //error here


    let LFO_AM Frequency MinAmplitude MaxAmplitude SampleRate Data =
        let Oscillator = FourWaves.SinWave
        let Amplitude = (maxAmplitude - MinAmplitude) / 2.
        let VerticalShift = (MaxAmplitude + MinAmplitude) / 2.
        Data
        |> List.mapi (fun i x ->
            let T = float i / float SampleRate
            x * (Oscillator Frequency Amplitude VerticalShift 0. T)
        )

    let LFO_FM Frequency DeltaFreq SampleRate (Data:List<float>) =
        failwith "Not working yet"
        let Ac = 1. // Carrier's amplitude
        let Fc = Frequency // Carrier's frequency
        let Fd = DeltaFreq // Frequency deviation = frequency modulator's sensitivity * data's amplitude

        let Integrate N (Xs:List<float>) =
            Xs
            |> List.take (N+1)
            |> List.sum
            |> ( * ) (float N) // (float N / float sampleRate)

        List.init (List.length data) (fun i ->
            let T = float i / SampleRate
            Ac * Cos ( 2. * Math.PI * (Fc * T + Fd * Integrate i Data))
            // https://en.wikipedia.org/wiki/Frequency_modulation#Theory
        )

    let LowPass SampleRate CutoffFreq (Data:List<float>) =
        let RC = 1. / (2. * Math.PI * CutoffFreq)
        let Dt = 1. / SampleRate
        let Alpha = Dt / (RC + Dt)
        let Alpha2 = 1. - Alpha

        let mutable Last = Alpha * Data.[0]
        [Last] @ (
            Data
            |> List.tail
            |> List.map (fun x ->
                Last <- Alpha * x + Alpha2 * Last
                Last
            )
        )

    let HighPass SampleRate CutoffFreq (Data:List<float>) =
        let RC = 1. / (2. * Math.PI * CutoffFreq)
        let Dt = 1. / SampleRate
        let Alpha = RC / (RC + Dt)

        let mutable Last = Data.[0]
        [Last] @ (
            (Data.[1..(List.length data - 1)], Data.[0..(List.length Data - 2)])
            ||> List.map2 ( - )
            |> List.map (fun x ->
                Last <- Alpha * (Last + X)
                Last
            )
        )

    let BandPass SampleRate LowFreq HighFreq (Data:List<float>) = 
        Data |> LowPass SampleRate LowFreq |> HighPass SampleRate HighFreq

    let RejectBand SampleRate LowFreq HighFreq (Data:List<float>) = 
        let LowPassData = HighPass SampleRate LowFreq Data
        let HighPassData = LowPass SampleRate HighFreq Data
        Utility.Add [LowPassData; HighPassData]
