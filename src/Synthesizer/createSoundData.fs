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
    let duration = getDuration (defaultArg duration0 Whole) (defaultArg bpm0 90.) // In seconds
    let sampleRate = defaultArg sampleRate0 44100.
    let arraySize = int ((defaultArg arraySize0 44100.) * duration * 2.) 
    let amplitude = defaultArg amplitude0 1.
    let phaseShift = (defaultArg phaseShift0 0.) * Math.PI
    let verticalShift = defaultArg verticalShift0 0.
    let frequency = (defaultArg frequency0 440.) /2.

    let toByte x = x/2. * 255. |> byte
    //https://www.geogebra.org/m/NS9DJf4S
    
    member x.create waveType =
        let waveFunc = 
            match waveType with
            | Sin -> fourWaves.sinWave
            | Square -> fourWaves.sawWave
            | Triangular -> fourWaves.triangleWave
            | Saw -> fourWaves.sawWave
            | Silence -> (fun freq amp vShift phaseShift t -> 0)
        
        Array.init arraySize (fun i -> Filter.makeOverdrive overdrive (waveFunc frequency amplitude verticalShift phaseShift (float i/sampleRate)))
