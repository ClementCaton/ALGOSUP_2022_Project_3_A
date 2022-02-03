namespace Synthesizer

open System

module Utility = 

    let CutStart (sampleRate:float) time (data:List<float>) = 
        data[int (sampleRate * time) .. data.Length]

    let CutEnd (sampleRate:float) time (data:List<float>) = 
        data[0 .. data.Length - int (sampleRate * time)-1] //need to add another time for the end

    let CutCorners limit (data:List<float>) =
        let step = 1. / float limit
        let startVals = List.map2(fun x i -> x * step * i) data[..limit-1] [1. .. float limit]
        let endVals = List.map2(fun x i -> x * step * i) data[data.Length-limit..] [float limit .. -1. .. 1.]

        List.append (List.append startVals data[limit .. data.Length-limit-1]) endVals
        
    let Maximize data =
        let factor = data |> List.map abs |> List.max |> ( / ) 1.
        data |> List.map (( * ) factor)

    let AddFactor (map:List<Tuple<List<float>, float>>) =
        let size = map |> List.map fst |> List.map List.length |> List.max
        let expand sound = List.append sound (List.replicate (size - List.length sound) 0.)
        map
        |> List.unzip
        ||> List.map2 (fun data factor -> data |> List.map (( * ) factor) )
        |> List.map expand
        |> List.transpose
        |> List.map List.sum

    let AddMean sounds =
        let mean = 1. / (float (List.length sounds))
        sounds |> List.map (fun sound -> sound, mean) |> AddFactor

    let AddMaximize = AddMean >> Maximize

    let Overdrive multiplicator (x:List<float>) =
        [for i in x do 
            if i < (-1. * multiplicator * 256.) then (-1. * multiplicator * 256.) else
            if i > (1. * multiplicator  * 256.) then (1. * multiplicator * 256.) else
            i]