namespace Synthesizer

module Utility = 
    let CutStart (SampleRate:float) Time (Data:List<float>) = 
        Data[int (SampleRate * Time) .. Data.Length]


    let CutEnd (SampleRate:float) Time (Data:List<float>) = 
        Data[0 .. Data.Length - int (SampleRate * Time)-1] //need to add another time for the end

    let CutCorners Limit (Data:List<float>) =
        let Step = 1. / float Limit
        let StartVals = List.map2(fun x i -> x * Step * I) Sata[..Limit-1] [1. .. float Limit]
        let EndVals = List.map2(fun x i -> x * Step * I) Data[Data.Length-Limit..] [float Limit .. -1. .. 1.]

        List.append (List.append StartVals Data[Limit .. Data.Length-Limit-1]) EndVals

(*    let add (jaggedArray: float[] list) =
        let size = jaggedArray |> List.map Array.length |> List.max
        let nTracks = List.length jaggedArray
        let matrix = jaggedArray |> List.map (fun L -> (List.ofArray L) @ (List.replicate (size - Array.length L) 0.))
        Array.init size (fun j -> Array.init nTracks (fun i -> matrix.[i].[j]) |> Array.sum |> ( / ) (float nTracks))*)
    //TODO: Choose one
    let Add Sounds =
        let Size = Sounds |> List.map List.length |> List.max
        let Mean = 1. / (float (List.length Sounds))
        let Expand Sound =
            List.append Sound (Array.ToList(Array.Replicate (Size - List.length Sound) 0.))
        let rec AddTwo (Sounds: List<float> List) =
            match Sounds with
            | a::b::rest -> AddTwo ((List.map2 ( + ) a b)::rest)
            | [a] -> a
            | [] -> List.empty

        Sounds |> List.map expand |> AddTwo |> List.map (( * ) Mean)

    let Overdrive Multiplicator (x:List<float>) =
        [for i in x do 
            if i < (-1. * Multiplicator * 256.) then (-1. * Multiplicator * 256.) else
            if i > (1. * Multiplicator  * 256.) then (1. * Multiplicator * 256.) else
            i]
