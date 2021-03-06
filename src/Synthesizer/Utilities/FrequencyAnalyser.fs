namespace Synthesizer

open System
open System.Numerics

module FrequencyAnalysis =

    (*
        Waveform can be described by adding sinusoidal with some frequencies magnitudes and phase angles

        We need to put 2 boxes to display the waveform and another to display the frequency after the fast fourier
        The objectives is to display the sinusoids compiled in a single columns
        a wave with a magnitude of 10 in 60 hertz mean a sinusoid at 60 hertz to a row 10 to describes the form

        We need to be able to compile more waves to mix with the first and display other sinusoid
        The magnitude and the phases of these waves should be changeable
    *)



    /// <summary>
    /// Recursive FFT algorithm using complex numbers
    /// </summary>
    /// <param name="">List of complex numbers</param>
    /// <returns>List of indices and complex amplitudes</returns>
    
    let rec FFT = function
    | []  -> []
    | [x] -> [x] 
    | x ->
        let multiplier = -2. * Math.PI / float x.Length
        x
        |> List.mapi (fun i c -> i % 2 = 0, c)
        |> List.partition fst
        |> fun (even, odd) -> FFT (List.map snd even), FFT (List.map snd odd)
        ||> List.mapi2 (fun i even odd -> 
            let Btf = odd * Complex.FromPolarCoordinates(1., multiplier * float i)
            even + Btf, even - Btf
        )
        |> List.unzip
        ||> List.append



    /// <summary>
    /// Makes a Fourier analysis
    /// </summary>
    /// <param name="sampleRate">Sample rate of the data</param>
    /// <param name="x">Data to analyse</param>
    /// <returns>Map of frequencies and amplitudes</returns>
    
    let Fourier (sampleRate: float) (x: List<float>) =
        let n =
            x
            |> List.length
            |> float
            |> (fun x -> Math.Log(x, 2.))
            |> ceil
            |> (fun x -> 2.**x)
            |> int
        // n is a power of 2 greater or equal to the size of x

        let increment = sampleRate / (float n - 2.) //* Given by Robert, taken from a library he uses

        (n - x.Length, 0.)
        ||> List.replicate
        |> List.append x
        // Now a list with a length that is a power of two
        |> List.map (fun f -> Complex(f, 0))
        |> FFT
        |> List.take (x.Length / 2)
        |> List.mapi (fun i c -> float i * increment, c.Magnitude / float n)
        |> Map.ofList



    /// <summary>
    /// Filters maximum frequencies based on their amplitudes
    /// </summary>
    /// <param name="threshold">Minimum amplitude</param>
    /// <param name="map">Map of frequencies to amplitudes to filter</param>
    /// <returns>Filtered frequencies</returns>
    
    let LocalMaxValuesIndices (threshold: float) (map: Map<float, float>) =
        // The threshold is the percentage between the bottom and the top where all point below are discarded to remove noise
        match Map.count map with
        | 0 -> []
        | _ ->
            if threshold < 0. || threshold > 1. then failwith "threshold must be a float in the range [0, 1]"
            let limit = (1. - threshold) * (Seq.min (Map.values map)) + threshold * (Seq.max (Map.values map))
            //printfn "%f %f %f" (List.min values) (List.max values) (List.average values)
            map
            |> Map.add -infinity -infinity
            |> Map.add infinity -infinity
            |> Map.toList
            |> List.windowed 3
            |> List.filter (fun items ->
                let V = List.map snd items
                V.[1] > limit && V.[0] <= V.[1] && V.[1] >= V.[2]
            )
            |> List.map (fun k -> k.[1])
            |> List.map fst
