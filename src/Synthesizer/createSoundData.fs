namespace Synthesizer
open System

module createSoundData =
    let toByte x = (x + 1.)/2. * 255. |> byte

    type createSinData(
            ?arraySize0:int,
            ?amplitude0:float,
            ?varticalShift0:float,
            ?phaseShift0:float,
            ?angularFrequency0:float,
            ?fromAngularFreqency0:bool,
            ?frequency0:int) =

        let arraySize = (defaultArg arraySize0 10000)
        let amplitude = (defaultArg amplitude0 1.)
        let phaseShift = (defaultArg phaseShift0 0.) * Math.PI
        let varticalShift = (defaultArg varticalShift0 0.)
        let angularFrequency = match (defaultArg fromAngularFreqency0 false) with
        | true -> (defaultArg angularFrequency0 (880. * Math.PI))
        | false -> float (defaultArg frequency0 440) * 2. * Math.PI

             

        //https://www.geogebra.org/m/NS9DJf4S
        let output = Array.init arraySize (fun i -> amplitude * sin (angularFrequency * float i - phaseShift) + varticalShift |> toByte)

        member x.Output = output
