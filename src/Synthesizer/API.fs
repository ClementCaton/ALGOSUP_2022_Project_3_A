namespace Synthesizer

open System
open System.IO

module API =
    
    let getNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    let getNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output


    let createSound freq duration waveType =
        createSoundData(frequency0 = freq, duration0 = duration, bpm0 = 114).create(waveType) // TEMP: Remove bpm

    let writeToWav path music =
        writeWav().Write (File.Create(path)) (music)

    let readFromWav path =
        readWav().Read (File.Open(path, FileMode.Open))

    let note duration note octave =
        let freq = getNoteFreq note octave
        createSound freq duration Sin
    
    let silence duration =
        createSound 0 duration Silence
    
    let compose sounds =
        //this is to be revisited
        sounds |> List.map(fun x -> Filter.cutCorners x 3500) |> Array.concat
    
(*    let add (jaggedArray: float[] list) =
        let size = jaggedArray |> List.map Array.length |> List.max
        let nTracks = List.length jaggedArray
        let matrix = jaggedArray |> List.map (fun L -> (List.ofArray L) @ (List.replicate (size - Array.length L) 0.))
        Array.init size (fun j -> Array.init nTracks (fun i -> matrix.[i].[j]) |> Array.sum |> (/) (float nTracks))*)
    
    let add sounds =
        let size = sounds |> List.map Array.length |> List.max
        let mean = 1. / (float (List.length sounds))
        let expand sound =
            Array.append sound (Array.replicate (size - Array.length sound) 0.)
        let rec addTwo (sounds: float[] list) =
            match sounds with
            | a::b::rest -> addTwo ((Array.map2 (+) a b)::rest)
            | [a] -> a
            | [] -> Array.empty

        sounds |> List.map expand |> addTwo |> Array.map ((*) mean)

    let preview sound =
        previewarr.chart sound
        sound