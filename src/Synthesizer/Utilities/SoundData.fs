namespace Synthesizer
open System


    
type Duration =
    | Whole
    | Half
    | Quarter
    | Eighth
    | Sixteenth
    | Custom of float
    | Seconds of float
    
type BaseWaves =
    | Sin
    | Square
    | Triangular
    | Saw
    | Silence

type SoundData(
        ?OverDrive0:float,
        ?Duration0:Duration, // In seconds
        ?ArraySize0:float,
        ?Amplitude0:float,
        ?VerticalShift0:float,
        ?PhaseShift0:float,
        ?Frequency0:float,
        ?SampleRate0:float,
        ?Bpm0:float) =

    let GetDuration DurationType Bpm =
        match DurationType with
        | Whole ->  4.                  * 60./bpm
        | Half -> 2.                    * 60./bpm
        | Quarter -> 1.                 * 60./bpm
        | Eighth -> 0.5                 * 60./bpm
        | Sixteenth -> 0.25             * 60./bpm
        | Custom value -> value * 4.    * 60./bpm
        | Seconds value -> value

    let Duration = GetDuration (DefaultArg Duration0 Quarter) (DefaultArg Bpm0 90.) // In seconds
    let SampleRate = defaultArg SampleRate0 44100.
    let ArraySize = int ((defaultArg ArraySize0 44100.) * Duration) 
    let Amplitude = defaultArg Amplitude0 1.
    let PhaseShift = (defaultArg PhaseShift0 0.) * Math.PI
    let VerticalShift = defaultArg VerticalShift0 0.
    let Frequency = defaultArg Frequency0 440.
    let OverDrive = defaultArg OverDrive0 1.

    let ToByte x = x/2. * 255. |> byte
    //https://www.geogebra.org/m/NS9DJf4S

    let SecTobyte (Sec:float) =
        Sec * SampleRate
    
    let WaveFunc WaveType = 
            match WaveType with
            | Sin -> FourWaves.SinWave
            | Square -> FourWaves.SquareWave
            | Triangular -> FourWaves.TriangleWave
            | Saw -> FourWaves.SawWave
            | Silence -> (fun Freq Amp VShift PhaseShift T -> 0)

    member x.Create WaveType =
        let A = List.init ArraySize (fun I -> ((WaveFunc WaveType) Frequency Amplitude VerticalShift PhaseShift (float i/SampleRate)))
        Utility.Overdrive OverDrive A

    member x.CreateFromDataPoints WaveType (DataPoints0: List<float * float>) = // (time, amp)
        let DataPoints = if (fst DataPoints0[0] <> 0.) then (0., 0.) :: DataPoints0 else DataPoints0
        let FlatSoundData = Utility.Overdrive 1. (List.init (int (SecTobyte (fst DataPoints[DataPoints.Length-1])))  (fun i -> ((waveFunc waveType) frequency amplitude verticalShift phaseShift (float i/sampleRate))))

        let CalcSegment (FromTime:float) (ToTime:float) FromAmp ToAmp =
            let Step = (toAmp - fromAmp) / (toTime - fromTime)
            List.mapi(fun I FlatPoint -> (FlatPoint * (FromAmp + (float Step * float I)))) FlatSoundData[int FromTime .. int ToTime]

        let Output = List.map2(fun FromT ToT -> CalcSegment (SecTobyte (fst FromT)) (SecTobyte (fst ToT)) (snd fromT) (snd ToT)) DataPoints[ .. DataPoints.Length-2] DataPoints[1 ..]

        output |> List.concat

    member x.CreateWithEnvelope WaveType Sustain Attack Hold0 Decay0 Release0 =  // time, time, time, amp, time
        let Hold = Hold0 + Attack
        let Decay = Hold + Decay0
        let Release = Duration + Release0

        x.CreateFromDataPoints WaveType [(0., 0.); (Attack, 1.); (Hold, 1.); (Decay, Sustain); (Duration, Sustain); (Release, 0.)]




    // let cutCorners (data:List<float>) limit =
        // let step = 1. / float limit
        // let startVals = List.map2(fun x i -> x * step * i) data[..limit-1] [1. .. float limit]
        // let endVals = List.map2(fun x i -> x * step * i) data[data.Length-limit..] [float limit .. -1. .. 1.]

        // List.append (List.append startVals data[limit .. data.Length-limit-1]) endVals
