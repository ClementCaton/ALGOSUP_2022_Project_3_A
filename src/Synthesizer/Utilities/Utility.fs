namespace Synthesizer
open System

module Utility =
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    
    let CutStart (sampleRate:float) time (data:List<float>) = 
        data[int (sampleRate * time) .. data.Length]



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    
    let CutEnd (sampleRate:float) time (data:List<float>) = 
        data[0 .. data.Length - int (sampleRate * time)-1] //need to add another time for the end



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    
    let CutCorners (limit0:int) (data:List<float>) =
        let limit = if limit0>data.Length/2 then data.Length/2 else limit0
        let step = 1. / float limit
        let startVals = List.map2(fun x i -> x * step * i) data[..limit-1] [1. .. float limit]
        let endVals = List.map2(fun x i -> x * step * i) data[data.Length-limit..] [float limit .. -1. .. 1.]

        List.append (List.append startVals data[limit .. data.Length-limit-1]) endVals



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
        
    let Maximize data =
        let factor = data |> List.map abs |> List.max |> ( / ) 1.
        data |> List.map (( * ) factor)
        
        


    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>

    let AddFactor (map:List<Tuple<List<float>, float>>) =
        let size = map |> List.map fst |> List.map List.length |> List.max
        let expand sound = List.append sound (List.replicate (size - List.length sound) 0.)
        map
        |> List.unzip
        ||> List.map2 (fun data factor -> data |> List.map (( * ) factor) )
        |> List.map expand
        |> List.transpose
        |> List.map List.sum



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    
    let AddMean sounds =
        let mean = 1. / (float (List.length sounds))
        sounds |> List.map (fun sound -> sound, mean) |> AddFactor



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    
    let AddMaximize = AddMean >> Maximize


    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    
    let AddSimple (sounds:list<list<float>>) = 
        let size = sounds |> List.map List.length |> List.max
        let expand sound = List.append sound (List.replicate (size - List.length sound) 0.)
        let add (values:List<float>) = 
            let sum = List.sum values
            if sum>1. then 1.
            else sum

        sounds |> List.map expand |> List.transpose |>  List.map add



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    
    let Overdrive multiplicator (x:List<float>) =
        x |> List.map (
            min multiplicator
            >> max (multiplicator * -1.)
        )