namespace Synthesizer
open System

module createSoundData =
    let toByte x = (x + 1.)/2. * 255. |> byte

    type createSinData(
            ?arraySize0:int,
            ?amplitude0:float,
            ?verticalShift0:float,
            ?phaseShift0:float,
            ?angularFrequency0:float,
            ?fromAngularFrequency0:bool,
            ?frequency0:int) =

        let arraySize = (defaultArg arraySize0 10000)
        let amplitude = (defaultArg amplitude0 1.)
        let phaseShift = (defaultArg phaseShift0 0.) * Math.PI
        let verticalShift = (defaultArg verticalShift0 0.)
        let angularFrequency = match (defaultArg fromAngularFrequency0 false) with
            | true -> (defaultArg angularFrequency0 (880. * Math.PI))
            | false -> float (defaultArg frequency0 440) * 2. * Math.PI



        //https://www.geogebra.org/m/NS9DJf4S
        member x.Output = Array.init arraySize (fun i -> amplitude * sin (angularFrequency * float i - phaseShift) + verticalShift |> toByte)

        member x.sin  = Array.init arraySize (fun i -> fourWaves.sinWave angularFrequency amplitude verticalShift phaseShift i |> toByte)

        member x.sawWave = Array.init arraySize (fun i -> fourWaves.sawWave angularFrequency amplitude verticalShift phaseShift i |> toByte)

        member x.squareWave = Array.init arraySize (fun i -> fourWaves.squareWave angularFrequency amplitude verticalShift phaseShift i |> toByte)

        member x.triangleWave = Array.init arraySize (fun i -> fourWaves.triangleWave angularFrequency amplitude verticalShift phaseShift i |> toByte)
