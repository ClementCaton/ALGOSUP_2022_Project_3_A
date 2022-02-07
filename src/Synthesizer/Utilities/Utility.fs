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
    /// Changes the amplitude to have the maximum be 1
    /// </summary>
    /// <param name="data">Data to amplify</param>
    /// <returns>Maximized data</returns>
        
    let Maximize data =
        let factor = data |> List.map abs |> List.max |> ( / ) 1.
        data |> List.map (( * ) factor)
        
        


    /// <summary>
    /// Superpose different data with a weight
    /// </summary>
    /// <param name="map">List map of data and weights to superpose</param>
    /// <returns>Superposed data</returns>

    let AddFactor (map:List<Tuple<List<float>, float>>) =
        let size = map |> List.map fst |> List.map List.length |> List.max
        let expand data = List.append data (List.replicate (size - List.length data) 0.)
        map
        |> List.unzip
        ||> List.map2 (fun data factor -> data |> List.map (( * ) factor) )
        |> List.map expand
        |> List.transpose
        |> List.map List.sum



    /// <summary>
    /// Superpose different data
    /// </summary>
    /// <param name="data">List of data to superpose</param>
    /// <returns>Superposed data</returns>
    
    let AddMean (data:List<List<float>>) =
        let mean = 1. / (float (List.length data))
        data |> List.map (fun d -> d, mean) |> AddFactor



    /// <summary>
    /// Superpose different data with maximum amplitude
    /// </summary>
    /// <param name="data">List of data to superpose</param>
    /// <returns>Superposed data</returns>
    
    let AddMaximize (data:List<List<float>>) = data |> AddMean |> Maximize


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
    /// Clamps the values between +/- the given limit
    /// </summary>
    /// <param name="multiplier">Limit of the overdrive</param>
    /// <param name="data">Data to clamp</param>
    /// <returns>Data with overdrive effect</returns>
    
    let Overdrive (multiplier:float) (x:List<float>) =
        x |> List.map (
            min multiplier
            >> max (multiplier * -1.)
        )
