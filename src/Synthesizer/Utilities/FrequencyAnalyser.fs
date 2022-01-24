namespace Synthesizer

open System
open System.Numerics
open MathNet.Numerics //can be remplaced by using the already created waveforms
open MathNet.Numerics.IntegralTransforms

// Used for complex numbers
// open System.Numerics
// used to display charts 
// open System.Windows.Forms.DataVisualization.Charting 

module frequencyAnalysis =

    // Waveform can be described by adding sinusoidal with some frequencies magnitudes and phase angles 

    // We need to put 2 boxes to display the waveform and another to display the frequency after the fast fourier
    // The objectives is to display the sinusoides compiled in a single columns 
    // a wave with a magnitude of 10 in 60 hertz mean a sinusoid at 60 hertz to a row 10 to describes the form

    // We need to be able to compile more waves to mix with the first and display other sinusoid
    // The magnitude and the phases of these waves should be changeable 



    let rec fft = function
    | []  -> []
    | [x] -> [x] 
    | x ->
        x
        |> List.mapi (fun i c -> i % 2 = 0, c)
        |> List.partition fst
        |> fun (even, odd) -> fft (List.map snd even), fft (List.map snd odd)
        ||> List.mapi2 (fun i even odd -> 
            let btf = odd * Complex.FromPolarCoordinates(1., -2. * Math.PI * (float i / float x.Length ))
            even + btf, even - btf)
        |> List.unzip
        ||> List.append


    let fourier (x:List<float>) =
        let samples = Array.init (x.Length-1) (fun v -> new Complex(x[v], 0))
        let mutable magnitude = List<float>.Empty
        MathNet.Numerics.IntegralTransforms.Fourier.Forward(samples, FourierOptions.NoScaling)
        for i in 1..(samples.Length / 5) do
            // = abs[sqrt(r^2 + i^2)]
            // why not "magnitude <- magnitude @ [formula]" ? because it's 0(n) and way longer than that
            magnitude <- (2.0/(float x.Length))*(Math.Abs(Math.Sqrt(Math.Pow(samples.[i].Real,2) + Math.Pow(samples.[i].Imaginary,2)))) :: magnitude
        magnitude |> List.rev
