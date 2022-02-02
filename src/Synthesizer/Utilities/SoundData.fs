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
    | CustomInstrument of (float -> float -> float -> float -> float -> float)

type SoundData(
        ?overDrive0:float,
        ?duration0:Duration, // In seconds
        ?arraySize0:float,
        ?amplitude0:float,
        ?verticalShift0:float,
        ?phaseShift0:float,
        ?frequency0:float,
        ?sampleRate0:float,
        ?bpm0:float) =

    let GetDuration durationType bpm =
        match durationType with
        | Whole ->  4.                  * 60./bpm
        | Half -> 2.                    * 60./bpm
        | Quarter -> 1.                 * 60./bpm
        | Eighth -> 0.5                 * 60./bpm
        | Sixteenth -> 0.25             * 60./bpm
        | Custom value -> value * 4.    * 60./bpm
        | Seconds value -> value

    let duration = GetDuration (defaultArg duration0 Quarter) (defaultArg bpm0 90.) // In seconds
    let sampleRate = defaultArg sampleRate0 44100.
    let arraySize = int ((defaultArg arraySize0 44100.) * duration) 
    let amplitude = defaultArg amplitude0 1.
    let phaseShift = (defaultArg phaseShift0 0.) * Math.PI
    let verticalShift = defaultArg verticalShift0 0.
    let frequency = defaultArg frequency0 440.
    let overDrive = defaultArg overDrive0 1.

    let ToByte x = x/2. * 255. |> byte
    //https://www.geogebra.org/m/NS9DJf4S

    let SecTobyte (sec:float) =
        sec * sampleRate
    
    let WaveFunc waveType = 
        match waveType with
        | Sin -> FourWaves.SinWave
        | Square -> FourWaves.SquareWave
        | Triangular -> FourWaves.TriangleWave
        | Saw -> FourWaves.SawWave
        | Silence -> (fun freq amp vShift phaseShift t -> 0)
        | CustomInstrument func -> func

    member x.Create waveType =
        let a = List.init arraySize (fun i -> ((WaveFunc waveType) frequency amplitude verticalShift phaseShift (float i/sampleRate)))
        Utility.Overdrive overDrive a

    member x.CreateFromDataPoints waveType (dataPoints0: List<float * float>) = // (time, amp)
        let dataPoints = if (fst dataPoints0[0] <> 0.) then (0., 0.) :: dataPoints0 else dataPoints0
        let flatSoundData = Utility.Overdrive 1. (List.init (int (SecTobyte (fst dataPoints[dataPoints.Length-1])))  (fun i -> ((WaveFunc waveType) frequency amplitude verticalShift phaseShift (float i/sampleRate))))

        let CalcSegment (fromTime:float) (toTime:float) fromAmp toAmp =
            let step = (toAmp - fromAmp) / (toTime - fromTime)
            List.mapi(fun i flatPoint -> (flatPoint * (fromAmp + (float step * float i)))) flatSoundData[int fromTime .. int toTime]

        let output = List.map2(fun fromT toT -> CalcSegment (SecTobyte (fst fromT)) (SecTobyte (fst toT)) (snd fromT) (snd toT)) dataPoints[ .. dataPoints.Length-2] dataPoints[1 ..]

        output |> List.concat

    member x.CreateWithEnvelope waveType sustain attack hold0 decay0 release0 =  // time, time, time, amp, time
        let hold = hold0 + attack
        let decay = hold + decay0
        let release = duration + release0

        x.CreateFromDataPoints waveType [(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); (duration, sustain); (release, 0.)]
