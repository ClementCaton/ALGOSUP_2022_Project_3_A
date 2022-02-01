namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let Input = Synth.Add [Synth.Note Whole Note.A 2; Synth.Note Whole Note.A 3; Synth.Note Whole Note.A 4; Synth.Note Whole Note.A 5]
    printfn "Wanted:   %A" [CalcNoteFreq(Note.A, 2).Output; CalcNoteFreq(Note.A, 3).Output; CalcNoteFreq(Note.A, 4).Output; CalcNoteFreq(Note.A, 5).Output]
    Synth.WriteToWav "A345.wav" [Input]
    let Output = FrequencyAnalysis.Fourier 44100. Input
    let Freq = FrequencyAnalysis.LocalMaxValuesIndices 0.25 Output
    let Amplitudes = Output |> Map.filter (fun F _ -> List.contains F Freq)
    //printfn "%f %f %f %f" (List.average input) (List.average output) (List.sum input) (List.sum output)
    printfn "Obtained: %A" Freq
    printfn "Amplitudes: %A" Amplitudes
    Synth.PreviewMap "A 3,4,5 Analysis" Output |> ignore