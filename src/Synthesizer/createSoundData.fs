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

type createSoundData(
        ?overdrive0:float,
        ?duration0:Duration, // In seconds
        ?arraySize0:float,
        ?amplitude0:float,
        ?verticalShift0:float,
        ?phaseShift0:float,
        ?frequency0:float,
        ?sampleRate0:float,
        ?bpm0:float) =

    let getDuration durationType bpm =
        match durationType with
        | Whole ->  4.                  * 60./bpm
        | Half -> 2.                    * 60./bpm
        | Quarter -> 1.                 * 60./bpm
        | Eighth -> 0.5                 * 60./bpm
        | Sixteenth -> 0.25             * 60./bpm
        | Custom value -> value * 4.    * 60./bpm
        | Seconds value -> value

    let overdrive = defaultArg overdrive0 1.
    let duration = getDuration (defaultArg duration0 Quarter) (defaultArg bpm0 90.) // In seconds
    let sampleRate = defaultArg sampleRate0 44100.
    let arraySize = int ((defaultArg arraySize0 44100.) * duration) 
    let amplitude = defaultArg amplitude0 1.
    let phaseShift = (defaultArg phaseShift0 0.) * Math.PI
    let verticalShift = defaultArg verticalShift0 0.
    let frequency = defaultArg frequency0 440.

    let toByte x = x/2. * 255. |> byte
    //https://www.geogebra.org/m/NS9DJf4S

    let secTobyte (sec:float) =
        sec * sampleRate
    
    let waveFunc waveType = 
            match waveType with
            | Sin -> fourWaves.sinWave
            | Square -> fourWaves.squareWave
            | Triangular -> fourWaves.triangleWave
            | Saw -> fourWaves.sawWave
            | Silence -> (fun freq amp vShift phaseShift t -> 0)


    member x.create waveType =
        let a = List.init arraySize (fun i -> ((waveFunc waveType) frequency amplitude verticalShift phaseShift (float i/sampleRate)))
        Filter.makeOverdrive overdrive a

    member x.createFromDataPoints waveType (dataPoints0: List<float * float>) = // (time, amp)
        let dataPoints = if (fst dataPoints0[0] <> 0.) then (0., 0.) :: dataPoints0 else dataPoints0
        let flatSoundData = Filter.makeOverdrive 1. (List.init (int (secTobyte (fst dataPoints[dataPoints.Length-1])))  (fun i -> ((waveFunc waveType) frequency amplitude verticalShift phaseShift (float i/sampleRate))))

        let calcSegment (fromTime:float) (toTime:float) fromAmp toAmp =
            let step = (toAmp - fromAmp) / (toTime - fromTime)
            List.mapi(fun i flatPoint -> (flatPoint * (fromAmp + (float step * float i)))) flatSoundData[int fromTime .. int toTime]

        let output = List.map2(fun fromT toT -> calcSegment (secTobyte (fst fromT)) (secTobyte (fst toT)) (snd fromT) (snd toT)) dataPoints[ .. dataPoints.Length-2] dataPoints[1 ..]

        output |> List.concat

    member x.creteWithEnvelope waveType sustain attack hold0 decay0 release0 =  // time, time, time, amp, time
        let hold = hold0 + attack
        let decay = hold + decay0
        let release = duration + release0

        x.createFromDataPoints waveType [(0., 0.); (attack, 1.); (hold, 1.); (decay, sustain); (duration, sustain); (release, 0.)]




    // let cutCorners (data:List<float>) limit =
        // let step = 1. / float limit
        // let startVals = List.map2(fun x i -> x * step * i) data[..limit-1] [1. .. float limit]
        // let endVals = List.map2(fun x i -> x * step * i) data[data.Length-limit..] [float limit .. -1. .. 1.]

        // List.append (List.append startVals data[limit .. data.Length-limit-1]) endVals