namespace Synthesizer
open System


    
type Duration =
    | Whole
    | Half
    | Quarter
    | Eighth
    | Sixteenth
    | Custom of float
    
type BaseWaves =
    | Sin
    | Square
    | Triangular
    | Saw

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

    let getDuration durationType =
        60. / (defaultArg bpm0 90.) * 
        match durationType with
        | Whole -> 4.
        | Half -> 2.
        | Quarter -> 1.
        | Eighth -> 0.5
        | Sixteenth -> 0.25
        | Custom value -> value * 4.

    let overdrive = defaultArg overdrive0 1.
    let duration = getDuration (defaultArg duration0 Whole) // In seconds
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
        
        Array.init arraySize (fun i -> Filter.makeOverdrive overdrive (waveFunc frequency amplitude verticalShift phaseShift (float i/sampleRate)) |> toByte)
