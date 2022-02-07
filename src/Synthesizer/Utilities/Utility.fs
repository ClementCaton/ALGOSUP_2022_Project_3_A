namespace Synthesizer

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
    
    let CutCorners limit (data:List<float>) =
        let step = 1. / float limit
        let startVals = List.map2(fun x i -> x * step * i) data[..limit-1] [1. .. float limit]
        let endVals = List.map2(fun x i -> x * step * i) data[data.Length-limit..] [float limit .. -1. .. 1.]

        List.append (List.append startVals data[limit .. data.Length-limit-1]) endVals



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    
    let Add sounds =
        let size = sounds |> List.map List.length |> List.max
        let mean = 1. / (float (List.length sounds))
        let expand sound =
            List.append sound (Array.toList(Array.replicate (size - List.length sound) 0.))
        let rec AddTwo (sounds: List<float> list) =
            match sounds with
            | a::b::rest -> AddTwo ((List.map2 ( + ) a b)::rest)
            | [a] -> a
            | [] -> List.empty

        sounds |> List.map expand |> AddTwo |> List.map (( * ) mean)



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    
    let Overdrive multiplicator (x:List<float>) =
        [for i in x do 
            if i < (-1. * multiplicator * 256.) then (-1. * multiplicator * 256.) else
            if i > (1. * multiplicator  * 256.) then (1. * multiplicator * 256.) else
            i]