namespace Synthesizer
open System



type createSoundData(
        ?arraySize0:int,
        ?amplitude0:float,
        ?verticalShift0:float,
        ?phaseShift0:float,
        ?frequency0:float,
        ?sampleRate0:float) =

    let arraySize = (defaultArg arraySize0 10000)
    let amplitude = (defaultArg amplitude0 1.)
    let phaseShift = (defaultArg phaseShift0 0.) * Math.PI
    let verticalShift = (defaultArg verticalShift0 0.)
    let frequency = (defaultArg frequency0 440.)
    let sampleRate = (defaultArg sampleRate0 440.)

    
    let toByte x = (x + 1.)/2. * 255. |> byte

    //https://www.geogebra.org/m/NS9DJf4S

    member x.sin  = Array.init arraySize (fun i -> fourWaves.sinWave frequency amplitude verticalShift phaseShift (float i/sampleRate) |> toByte)

    member x.sawWave = Array.init arraySize (fun i -> fourWaves.sawWave frequency amplitude verticalShift phaseShift (float i/sampleRate) |> toByte)

    member x.squareWave = Array.init arraySize (fun i -> fourWaves.squareWave frequency amplitude verticalShift phaseShift (float i/sampleRate) |> toByte)

    member x.triangleWave = Array.init arraySize (fun i -> fourWaves.triangleWave frequency amplitude verticalShift phaseShift (float i/sampleRate) |> toByte)
