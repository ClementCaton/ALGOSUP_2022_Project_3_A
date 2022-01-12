namespace Synthesizer
open System



type createSoundData(
        ?overdrive0:float,
        ?duration0:float, // In seconds
        ?arraySize0:float,
        ?amplitude0:float,
        ?verticalShift0:float,
        ?phaseShift0:float,
        ?frequency0:float,
        ?sampleRate0:float) =

    let overdrive = defaultArg overdrive0 1.
    let duration = defaultArg duration0 1. // In secondss
    let sampleRate = defaultArg sampleRate0 44100.
    let arraySize = int ((defaultArg arraySize0 44100.) * duration * 2.) 
    let amplitude = defaultArg amplitude0 1.
    let phaseShift = (defaultArg phaseShift0 0.) * Math.PI
    let verticalShift = defaultArg verticalShift0 0.
    let frequency = (defaultArg frequency0 440.) /2.

    let toByte x = x/2. * 255. |> byte
    //https://www.geogebra.org/m/NS9DJf4S

    member x.sinWave = Array.init arraySize (fun i ->
        Filter.makeOverdrive overdrive (fourWaves.sinWave frequency amplitude verticalShift phaseShift (float i/sampleRate)) |> toByte)

    member x.sawWave = Array.init arraySize (fun i ->
        Filter.makeOverdrive overdrive (fourWaves.sawWave frequency amplitude verticalShift phaseShift (float i/sampleRate)) |> toByte)

    member x.squareWave = Array.init arraySize (fun i ->
        Filter.makeOverdrive overdrive (fourWaves.squareWave frequency amplitude verticalShift phaseShift (float i/sampleRate)) |> toByte)

    member x.triangleWave = Array.init arraySize (fun i ->
        Filter.makeOverdrive overdrive (fourWaves.triangleWave frequency amplitude verticalShift phaseShift (float i/sampleRate)) |> toByte)
