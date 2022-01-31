namespace Synthesizer

module Utility = 
    let cutStart (sampleRate:float) time (data:List<float>) = 
        data[int (sampleRate * time) .. data.Length]


    let cutEnd (sampleRate:float) time (data:List<float>) = 
        data[0 .. data.Length - int (sampleRate * time)-1] //need to add another time for the end

    let cutCorners limit (data:List<float>) =
        let step = 1. / float limit
        let startVals = List.map2(fun x i -> x * step * i) data[..limit-1] [1. .. float limit]
        let endVals = List.map2(fun x i -> x * step * i) data[data.Length-limit..] [float limit .. -1. .. 1.]

        List.append (List.append startVals data[limit .. data.Length-limit-1]) endVals

(*    let add (jaggedArray: float[] list) =
        let size = jaggedArray |> List.map Array.length |> List.max
        let nTracks = List.length jaggedArray
        let matrix = jaggedArray |> List.map (fun L -> (List.ofArray L) @ (List.replicate (size - Array.length L) 0.))
        Array.init size (fun j -> Array.init nTracks (fun i -> matrix.[i].[j]) |> Array.sum |> ( / ) (float nTracks))*)
    //TODO: Choose one
    let add sounds =
        let size = sounds |> List.map List.length |> List.max
        let mean = 1. / (float (List.length sounds))
        let expand sound =
            List.append sound (Array.toList(Array.replicate (size - List.length sound) 0.))
        let rec addTwo (sounds: List<float> list) =
            match sounds with
            | a::b::rest -> addTwo ((List.map2 ( + ) a b)::rest)
            | [a] -> a
            | [] -> List.empty

        sounds |> List.map expand |> addTwo |> List.map (( * ) mean)

    let Overdrive multiplicator (x:List<float>) =
        [for i in x do 
            if i < (-1. * multiplicator * 256.) then (-1. * multiplicator * 256.) else
            if i > (1. * multiplicator  * 256.) then (1. * multiplicator * 256.) else
            i]
